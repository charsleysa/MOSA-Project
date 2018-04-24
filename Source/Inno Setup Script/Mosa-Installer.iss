; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppName "MOSA-Project"
#define MyAppVersion "1.5"
#define MyAppPublisher "MOSA-Project"
#define MyAppURL "http://www.mosa-project.org"
#define MyAppExeName "Mosa.Tool.Launcher.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{516E4655-F79C-44AC-AA6D-D9A879450A64}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={pf}\{#MyAppName}
DisableDirPage=yes
DefaultGroupName={#MyAppName}
OutputDir=..\..\bin\MOSA Installer
OutputBaseFilename=MOSA-Installer
SolidCompression=yes
MinVersion=0,6.0
AllowUNCPath=False
Compression=lzma2/ultra64
InternalCompressLevel=ultra64

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\bin\{#MyAppExeName}"
Name: "{group}\{cm:ProgramOnTheWeb,{#MyAppName}}"; Filename: "{#MyAppURL}"
Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\bin\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\bin\{#MyAppExeName}"; Tasks: quicklaunchicon
Name: "{group}\{cm:UninstallProgram, {#MyAppName}}"; Filename: "{uninstallexe}"

[Run]
Filename: "{app}\bin\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Dirs]
Name: "{app}\Tools"
Name: "{app}\bin"
Name: "{app}\Lib"

[Files]
Source: "..\..\bin\*.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\*.config"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\Tools\mkisofs\*"; DestDir: "{app}\Tools\mkisofs"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\ndisasm\*"; DestDir: "{app}\Tools\ndisasm"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\qemu\*"; DestDir: "{app}\Tools\qemu"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\rufus\*"; DestDir: "{app}\Tools\rufus"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\syslinux\*"; DestDir: "{app}\Tools\syslinux"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\gdb\*"; DestDir: "{app}\Tools\gdb"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\grub\*"; DestDir: "{app}\Tools\grub"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\Bochs\*"; DestDir: "{app}\Tools\Bochs"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\nuget\*"; DestDir: "{app}\Tools\nuget"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\Bochs\*"; DestDir: "{app}\Tools\Bochs"; Flags: ignoreversion createallsubdirs recursesubdirs
Source: "..\..\Tools\readme.md"; DestDir: "{app}\Tools"; Flags: ignoreversion
Source: "..\..\*.txt"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\*.md"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Compiler.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Debugger.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Disassembler.Intel.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Explorer.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.GDBDebugger.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Launcher.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.Compiler.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Tool.CreateBootImage.exe"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.CodeDomCompiler.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.DebugEngine.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.GUI.Common.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.Launcher.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.RSP.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\SharpDisasm.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\System.Console.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\System.Reflection.TypeExtensions.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\WeifenLuo.WinFormsUI.Docking.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\WeifenLuo.WinFormsUI.Docking.ThemeVS2015.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\CommandLine.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\dnlib.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\MetroFramework.Design.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\MetroFramework.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\MetroFramework.Fonts.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.ClassLib.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.Common.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.Framework.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.Linker.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.MosaTypeSystem.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.Pdb.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Compiler.Trace.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.DeviceSystem.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.FileSystem.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Platform.ARMv6.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Kernel.x86.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Platform.x86.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\Mosa.Utility.BootImage.dll"; DestDir: "{app}\bin"; Flags: ignoreversion
Source: "..\..\bin\mscorlib.dll"; DestDir: "{app}\Lib"; Flags: ignoreversion

[ThirdParty]
UseRelativePaths=True