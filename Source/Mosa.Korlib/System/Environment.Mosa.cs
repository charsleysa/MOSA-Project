// Copyright (c) MOSA Project. Licensed under the New BSD License.
#nullable enable

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace System
{
	public static partial class Environment
	{
		// Emscripten VFS mounts at / and is the only drive
		public static string[] GetLogicalDrives() => DriveInfoInternal.GetLogicalDrives();

		public static string MachineName => "mosa";

		public static long WorkingSet => 0;

		public static string UserName => "root";

		private static OperatingSystem GetOSVersion()
		{
			return new OperatingSystem(PlatformID.Other, new Version(2, 2, 0, 0));
		}

		private static int GetProcessId() => 42;

		/// <summary>
		/// Returns the path of the executable that started the currently executing process. Returns null when the path is not available.
		/// </summary>
		/// <returns>Path of the executable that started the currently executing process</returns>
		private static string? GetProcessPath() => null;

		public static bool UserInteractive => true;

		private static string CurrentDirectoryCore
		{
			get => Interop.Sys.GetCwd();
			set => Interop.CheckIo(Interop.Sys.ChDir(value), value, isDirectory: true);
		}

		private static string ExpandEnvironmentVariablesCore(string name)
		{
			var result = new ValueStringBuilder(stackalloc char[128]);

			int lastPos = 0, pos;
			while (lastPos < name.Length && (pos = name.IndexOf('%', lastPos + 1)) >= 0)
			{
				if (name[lastPos] == '%')
				{
					string key = name.Substring(lastPos + 1, pos - lastPos - 1);
					string? value = GetEnvironmentVariable(key);
					if (value != null)
					{
						result.Append(value);
						lastPos = pos + 1;
						continue;
					}
				}
				result.Append(name.AsSpan(lastPos, pos - lastPos));
				lastPos = pos;
			}
			result.Append(name.AsSpan(lastPos));

			return result.ToString();
		}

		private static bool Is64BitOperatingSystemWhen32BitProcess => false;

		internal const string NewLineConst = "\n";

		public static string SystemDirectory => GetFolderPathCore(SpecialFolder.System, SpecialFolderOption.None);

		public static int SystemPageSize => 4096;

		public static string UserDomainName => MachineName;

		private static string GetFolderPathCore(SpecialFolder folder, SpecialFolderOption option)
		{
			// Get the path for the SpecialFolder
			string path = GetFolderPathCoreWithoutValidation(folder);
			Debug.Assert(path != null);

			// If we didn't get one, or if we got one but we're not supposed to verify it,
			// or if we're supposed to verify it and it passes verification, return the path.
			if (path.Length == 0 ||
				option == SpecialFolderOption.DoNotVerify ||
				Interop.Sys.Access(path, Interop.Sys.AccessMode.R_OK) == 0)
			{
				return path;
			}

			// Failed verification.  If None, then we're supposed to return an empty string.
			// If Create, we're supposed to create it and then return the path.
			if (option == SpecialFolderOption.None)
			{
				return string.Empty;
			}

			Debug.Assert(option == SpecialFolderOption.Create);

			Directory.CreateDirectory(path);

			return path;
		}

		private static string GetFolderPathCoreWithoutValidation(SpecialFolder folder)
		{
			// First handle any paths that involve only static paths, avoiding the overheads of getting user-local paths.
			// https://www.freedesktop.org/software/systemd/man/file-hierarchy.html
			switch (folder)
			{
				case SpecialFolder.CommonApplicationData: return "/usr/share";
				case SpecialFolder.CommonTemplates: return "/usr/share/templates";
				case SpecialFolder.ProgramFiles: return "/apps";
				case SpecialFolder.System: return "/system";
			}

			// All other paths are based on the XDG Base Directory Specification:
			// https://specifications.freedesktop.org/basedir-spec/latest/
			string? home = null;
			try
			{
				home = PersistedFiles.GetHomeDirectory();
			}
			catch (Exception exc)
			{
				Debug.Fail($"Unable to get home directory: {exc}");
			}

			// Fall back to '/' when we can't determine the home directory.
			// This location isn't writable by non-root users which provides some safeguard
			// that the application doesn't write data which is meant to be private.
			if (string.IsNullOrEmpty(home))
			{
				home = "/";
			}

			// TODO: Consider caching (or precomputing and caching) all subsequent results.
			// This would significantly improve performance for repeated access, at the expense
			// of not being responsive to changes in the underlying environment variables,
			// configuration files, etc.

			switch (folder)
			{
				case SpecialFolder.UserProfile:
				case SpecialFolder.MyDocuments: // same value as Personal
					return home;

				case SpecialFolder.ApplicationData:
					return GetXdgConfig(home);

				case SpecialFolder.LocalApplicationData:

					// "$XDG_DATA_HOME defines the base directory relative to which user specific data files should be stored."
					// "If $XDG_DATA_HOME is either not set or empty, a default equal to $HOME/.local/share should be used."
					string? data = GetEnvironmentVariable("XDG_DATA_HOME");
					if (string.IsNullOrEmpty(data) || data[0] != '/')
					{
						data = Path.Combine(home, ".local", "share");
					}
					return data;

				case SpecialFolder.Desktop:
				case SpecialFolder.DesktopDirectory:
					return ReadXdgDirectory(home, "XDG_DESKTOP_DIR", "Desktop");

				case SpecialFolder.Templates:
					return ReadXdgDirectory(home, "XDG_TEMPLATES_DIR", "Templates");

				case SpecialFolder.MyVideos:
					return ReadXdgDirectory(home, "XDG_VIDEOS_DIR", "Videos");

#if TARGET_OSX
                case SpecialFolder.MyMusic:
                    return Path.Combine(home, "Music");

                case SpecialFolder.MyPictures:
                    return Path.Combine(home, "Pictures");

                case SpecialFolder.Fonts:
                    return Path.Combine(home, "Library", "Fonts");

                case SpecialFolder.Favorites:
                    return Path.Combine(home, "Library", "Favorites");

                case SpecialFolder.InternetCache:
                    return Path.Combine(home, "Library", "Caches");
#else
				case SpecialFolder.MyMusic:
					return ReadXdgDirectory(home, "XDG_MUSIC_DIR", "Music");

				case SpecialFolder.MyPictures:
					return ReadXdgDirectory(home, "XDG_PICTURES_DIR", "Pictures");

				case SpecialFolder.Fonts:
					return Path.Combine(home, ".fonts");
#endif
			}

			// No known path for the SpecialFolder
			return string.Empty;
		}

		private static string GetXdgConfig(string home)
		{
			// "$XDG_CONFIG_HOME defines the base directory relative to which user specific configuration files should be stored."
			// "If $XDG_CONFIG_HOME is either not set or empty, a default equal to $HOME/.config should be used."
			string? config = GetEnvironmentVariable("XDG_CONFIG_HOME");
			if (string.IsNullOrEmpty(config) || config[0] != '/')
			{
				config = Path.Combine(home, ".config");
			}
			return config;
		}

		private static string ReadXdgDirectory(string homeDir, string key, string fallback)
		{
			Debug.Assert(!string.IsNullOrEmpty(homeDir), $"Expected non-empty homeDir");
			Debug.Assert(!string.IsNullOrEmpty(key), $"Expected non-empty key");
			Debug.Assert(!string.IsNullOrEmpty(fallback), $"Expected non-empty fallback");

			string? envPath = GetEnvironmentVariable(key);
			if (!string.IsNullOrEmpty(envPath) && envPath[0] == '/')
			{
				return envPath;
			}

			// Use the user-dirs.dirs file to look up the right config.
			// Note that the docs also highlight a list of directories in which to look for this file:
			// "$XDG_CONFIG_DIRS defines the preference-ordered set of base directories to search for configuration files in addition
			//  to the $XDG_CONFIG_HOME base directory. The directories in $XDG_CONFIG_DIRS should be separated with a colon ':'. If
			//  $XDG_CONFIG_DIRS is either not set or empty, a value equal to / etc / xdg should be used."
			// For simplicity, we don't currently do that.  We can add it if/when necessary.

			string userDirsPath = Path.Combine(GetXdgConfig(homeDir), "user-dirs.dirs");
			if (Interop.Sys.Access(userDirsPath, Interop.Sys.AccessMode.R_OK) == 0)
			{
				try
				{
					using (var reader = new StreamReader(userDirsPath))
					{
						string? line;
						while ((line = reader.ReadLine()) != null)
						{
							// Example lines:
							// XDG_DESKTOP_DIR="$HOME/Desktop"
							// XDG_PICTURES_DIR = "/absolute/path"

							// Skip past whitespace at beginning of line
							int pos = 0;
							SkipWhitespace(line, ref pos);
							if (pos >= line.Length) continue;

							// Skip past requested key name
							if (string.CompareOrdinal(line, pos, key, 0, key.Length) != 0) continue;
							pos += key.Length;

							// Skip past whitespace and past '='
							SkipWhitespace(line, ref pos);
							if (pos >= line.Length - 4 || line[pos] != '=') continue; // 4 for ="" and at least one char between quotes
							pos++; // skip past '='

							// Skip past whitespace and past first quote
							SkipWhitespace(line, ref pos);
							if (pos >= line.Length - 3 || line[pos] != '"') continue; // 3 for "" and at least one char between quotes
							pos++; // skip past opening '"'

							// Skip past relative prefix if one exists
							bool relativeToHome = false;
							const string RelativeToHomePrefix = "$HOME/";
							if (string.CompareOrdinal(line, pos, RelativeToHomePrefix, 0, RelativeToHomePrefix.Length) == 0)
							{
								relativeToHome = true;
								pos += RelativeToHomePrefix.Length;
							}
							else if (line[pos] != '/') // if not relative to home, must be absolute path
							{
								continue;
							}

							// Find end of path
							int endPos = line.IndexOf('"', pos);
							if (endPos <= pos) continue;

							// Got we need.  Now extract it.
							string path = line.Substring(pos, endPos - pos);
							return relativeToHome ?
								Path.Combine(homeDir, path) :
								path;
						}
					}
				}
				catch (Exception exc)
				{
					// assembly not found, file not found, errors reading file, etc. Just eat everything.
					Debug.Fail($"Failed reading {userDirsPath}: {exc}");
				}
			}

			return Path.Combine(homeDir, fallback);
		}

		private static void SkipWhitespace(string line, ref int pos)
		{
			while (pos < line.Length && char.IsWhiteSpace(line[pos])) pos++;
		}
	}
}
