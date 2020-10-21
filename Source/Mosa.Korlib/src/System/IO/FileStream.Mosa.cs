// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32.SafeHandles;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace System.IO
{
    /// <summary>Provides an implementation of a file stream for Unix files.</summary>
    public partial class FileStream : Stream
    {
        /// <summary>File mode.</summary>
        private FileMode _mode;

        /// <summary>Advanced options requested when opening the file.</summary>
        private FileOptions _options;

        /// <summary>If the file was opened with FileMode.Append, the length of the file when opened; otherwise, -1.</summary>
        private long _appendStart = -1;

        /// <summary>
        /// Extra state used by the file stream when _useAsyncIO is true.  This includes
        /// the semaphore used to serialize all operation, the buffer/offset/count provided by the
        /// caller for ReadAsync/WriteAsync operations, and the last successful task returned
        /// synchronously from ReadAsync which can be reused if the count matches the next request.
        /// Only initialized when <see cref="_useAsyncIO"/> is true.
        /// </summary>
        private AsyncState? _asyncState;

        /// <summary>Lazily-initialized value for whether the file supports seeking.</summary>
        private bool? _canSeek;

        private SafeFileHandle OpenHandle(FileMode mode, FileShare share, FileOptions options)
		{
			// FileStream performs most of the general argument validation.  We can assume here that the arguments
			// are all checked and consistent (e.g. non-null-or-empty path; valid enums in mode, access, share, and options; etc.)
			// Store the arguments
			_mode = mode;
			_options = options;

			if (_useAsyncIO)
				_asyncState = new AsyncState();

			throw new NotImplementedException();
        }

        private static bool GetDefaultIsAsync(SafeFileHandle handle) => handle.IsAsync ?? DefaultIsAsync;

        /// <summary>Initializes a stream for reading or writing a Unix file.</summary>
        /// <param name="mode">How the file should be opened.</param>
        /// <param name="share">What other access to the file should be allowed.  This is currently ignored.</param>
        /// <param name="originalPath">The original path specified for the FileStream.</param>
        private void Init(FileMode mode, FileShare share, string originalPath)
        {
			throw new NotImplementedException();
        }

        /// <summary>Initializes a stream from an already open file handle (file descriptor).</summary>
        private void InitFromHandle(SafeFileHandle handle, FileAccess access, bool useAsyncIO)
        {
            if (useAsyncIO)
                _asyncState = new AsyncState();

            if (CanSeekCore(handle)) // use non-virtual CanSeekCore rather than CanSeek to avoid making virtual call during ctor
                SeekCore(handle, 0, SeekOrigin.Current);
        }

        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        public override bool CanSeek => CanSeekCore(_fileHandle);

        /// <summary>Gets a value indicating whether the current stream supports seeking.</summary>
        /// <remarks>
        /// Separated out of CanSeek to enable making non-virtual call to this logic.
        /// We also pass in the file handle to allow the constructor to use this before it stashes the handle.
        /// </remarks>
        private bool CanSeekCore(SafeFileHandle fileHandle)
        {
            if (fileHandle.IsClosed)
            {
                return false;
            }

            if (!_canSeek.HasValue)
            {
				throw new NotImplementedException();
            }

            return _canSeek.GetValueOrDefault();
        }

        private long GetLengthInternal()
        {
            // Get the length of the file as reported by the OS
			throw new NotImplementedException();
            // long length = 0;

            // // But we may have buffered some data to be written that puts our length
            // // beyond what the OS is aware of.  Update accordingly.
            // if (_writePos > 0 && _filePosition + _writePos > length)
            // {
            //     length = _writePos + _filePosition;
            // }

            // return length;
        }

        /// <summary>Releases the unmanaged resources used by the stream.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_fileHandle != null && !_fileHandle.IsClosed)
                {
                    // Flush any remaining data in the file
                    try
                    {
                        FlushWriteBuffer();
                    }
                    catch (Exception e) when (IsIoRelatedException(e) && !disposing)
                    {
                        // On finalization, ignore failures from trying to flush the write buffer,
                        // e.g. if this stream is wrapping a pipe and the pipe is now broken.
                    }

                    // If DeleteOnClose was requested when constructed, delete the file now.
                    // (Unix doesn't directly support DeleteOnClose, so we mimic it here.)
                    if (_path != null && (_options & FileOptions.DeleteOnClose) != 0)
                    {
                        // Since we still have the file open, this will end up deleting
                        // it (assuming we're the only link to it) once it's closed, but the
                        // name will be removed immediately.
						throw new NotImplementedException();
                    }

                    // Closing the file handle can fail, e.g. due to out of disk space
                    // Throw these errors as exceptions when disposing
                    if (_fileHandle != null && !_fileHandle.IsClosed && disposing)
                    {
                        _fileHandle.Dispose();
						throw new NotImplementedException();
                    }
                }
            }
            finally
            {
                if (_fileHandle != null && !_fileHandle.IsClosed)
                {
                    _fileHandle.Dispose();
                }
                base.Dispose(disposing);
            }
        }

        public override ValueTask DisposeAsync()
        {
            // On Unix, we don't have any special support for async I/O, simply queueing writes
            // rather than doing them synchronously.  As such, if we're "using async I/O" and we
            // have something to flush, queue the call to Dispose, so that we end up queueing whatever
            // write work happens to flush the buffer.  Otherwise, just delegate to the base implementation,
            // which will synchronously invoke Dispose.  We don't need to factor in the current type
            // as we're using the virtual Dispose either way, and therefore factoring in whatever
            // override may already exist on a derived type.
            if (_useAsyncIO && _writePos > 0)
            {
                return new ValueTask(Task.Factory.StartNew(static s => ((FileStream)s!).Dispose(), this,
                    CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default));
            }

            return base.DisposeAsync();
        }

        /// <summary>Flushes the OS buffer.  This does not flush the internal read/write buffer.</summary>
        private void FlushOSBuffer()
        {
			throw new NotImplementedException();
        }

        private void FlushWriteBufferForWriteByte()
        {
            _asyncState?.Wait();
            try { FlushWriteBuffer(); }
            finally { _asyncState?.Release(); }
        }

        /// <summary>Writes any data in the write buffer to the underlying stream and resets the buffer.</summary>
        private void FlushWriteBuffer()
        {
            AssertBufferInvariants();
            if (_writePos > 0)
            {
                WriteNative(new ReadOnlySpan<byte>(GetBuffer(), 0, _writePos));
                _writePos = 0;
            }
        }

        /// <summary>Asynchronously clears all buffers for this stream, causing any buffered data to be written to the underlying device.</summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous flush operation.</returns>
        private Task FlushAsyncInternal(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }
            if (_fileHandle.IsClosed)
            {
                throw Error.GetFileNotOpen();
            }

            // As with Win32FileStream, flush the buffers synchronously to avoid race conditions.
            try
            {
                FlushInternalBuffer();
            }
            catch (Exception e)
            {
                return Task.FromException(e);
            }

            // We then separately flush to disk asynchronously.  This is only
            // necessary if we support writing; otherwise, we're done.
            if (CanWrite)
            {
                return Task.Factory.StartNew(
                    static state => ((FileStream)state!).FlushOSBuffer(),
                    this,
                    cancellationToken,
                    TaskCreationOptions.DenyChildAttach,
                    TaskScheduler.Default);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        /// <summary>Sets the length of this stream to the given value.</summary>
        /// <param name="value">The new length of the stream.</param>
        private void SetLengthInternal(long value)
        {
            FlushInternalBuffer();

            if (_appendStart != -1 && value < _appendStart)
            {
                throw new IOException(SR.IO_SetLengthAppendTruncate);
            }

            long origPos = _filePosition;

            VerifyOSHandlePosition();

            if (_filePosition != value)
            {
                SeekCore(_fileHandle, value, SeekOrigin.Begin);
            }

			throw new NotImplementedException();

            // Return file pointer to where it was before setting length
            // if (origPos != value)
            // {
            //     if (origPos < value)
            //     {
            //         SeekCore(_fileHandle, origPos, SeekOrigin.Begin);
            //     }
            //     else
            //     {
            //         SeekCore(_fileHandle, 0, SeekOrigin.End);
            //     }
            // }
        }

        /// <summary>Reads a block of bytes from the stream and writes the data in a given buffer.</summary>
        private int ReadSpan(Span<byte> destination)
        {
            PrepareForReading();

            // Are there any bytes available in the read buffer? If yes,
            // we can just return from the buffer.  If the buffer is empty
            // or has no more available data in it, we can either refill it
            // (and then read from the buffer into the user's buffer) or
            // we can just go directly into the user's buffer, if they asked
            // for more data than we'd otherwise buffer.
            int numBytesAvailable = _readLength - _readPos;
            bool readFromOS = false;
            if (numBytesAvailable == 0)
            {
                // If we're not able to seek, then we're not able to rewind the stream (i.e. flushing
                // a read buffer), in which case we don't want to use a read buffer.  Similarly, if
                // the user has asked for more data than we can buffer, we also want to skip the buffer.
                if (!CanSeek || (destination.Length >= _bufferLength))
                {
                    // Read directly into the user's buffer
                    _readPos = _readLength = 0;
                    return ReadNative(destination);
                }
                else
                {
                    // Read into our buffer.
                    _readLength = numBytesAvailable = ReadNative(GetBuffer());
                    _readPos = 0;
                    if (numBytesAvailable == 0)
                    {
                        return 0;
                    }

                    // Note that we did an OS read as part of this Read, so that later
                    // we don't try to do one again if what's in the buffer doesn't
                    // meet the user's request.
                    readFromOS = true;
                }
            }

            // Now that we know there's data in the buffer, read from it into the user's buffer.
            Debug.Assert(numBytesAvailable > 0, "Data must be in the buffer to be here");
            int bytesRead = Math.Min(numBytesAvailable, destination.Length);
            new Span<byte>(GetBuffer(), _readPos, bytesRead).CopyTo(destination);
            _readPos += bytesRead;

            // We may not have had enough data in the buffer to completely satisfy the user's request.
            // While Read doesn't require that we return as much data as the user requested (any amount
            // up to the requested count is fine), FileStream on Windows tries to do so by doing a
            // subsequent read from the file if we tried to satisfy the request with what was in the
            // buffer but the buffer contained less than the requested count. To be consistent with that
            // behavior, we do the same thing here on Unix.  Note that we may still get less the requested
            // amount, as the OS may give us back fewer than we request, either due to reaching the end of
            // file, or due to its own whims.
            if (!readFromOS && bytesRead < destination.Length)
            {
                Debug.Assert(_readPos == _readLength, "bytesToRead should only be < destination.Length if numBytesAvailable < destination.Length");
                _readPos = _readLength = 0; // no data left in the read buffer
                bytesRead += ReadNative(destination.Slice(bytesRead));
            }

            return bytesRead;
        }

        /// <summary>Unbuffered, reads a block of bytes from the file handle into the given buffer.</summary>
        /// <param name="buffer">The buffer into which data from the file is read.</param>
        /// <returns>
        /// The total number of bytes read into the buffer. This might be less than the number of bytes requested
        /// if that number of bytes are not currently available, or zero if the end of the stream is reached.
        /// </returns>
        private unsafe int ReadNative(Span<byte> buffer)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously reads a sequence of bytes from the current stream and advances
        /// the position within the stream by the number of bytes read.
        /// </summary>
        /// <param name="destination">The buffer to write the data into.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <param name="synchronousResult">If the operation completes synchronously, the number of bytes read.</param>
        /// <returns>A task that represents the asynchronous read operation.</returns>
        private Task<int>? ReadAsyncInternal(Memory<byte> destination, CancellationToken cancellationToken, out int synchronousResult)
        {
            Debug.Assert(_useAsyncIO);
            Debug.Assert(_asyncState != null);

            if (!CanRead) // match Windows behavior; this gets thrown synchronously
            {
                throw Error.GetReadNotSupported();
            }

            // Serialize operations using the semaphore.
            Task waitTask = _asyncState.WaitAsync();

            // If we got ownership immediately, and if there's enough data in our buffer
            // to satisfy the full request of the caller, hand back the buffered data.
            // While it would be a legal implementation of the Read contract, we don't
            // hand back here less than the amount requested so as to match the behavior
            // in ReadCore that will make a native call to try to fulfill the remainder
            // of the request.
            if (waitTask.Status == TaskStatus.RanToCompletion)
            {
                int numBytesAvailable = _readLength - _readPos;
                if (numBytesAvailable >= destination.Length)
                {
                    try
                    {
                        PrepareForReading();

                        new Span<byte>(GetBuffer(), _readPos, destination.Length).CopyTo(destination.Span);
                        _readPos += destination.Length;

                        synchronousResult = destination.Length;
                        return null;
                    }
                    catch (Exception exc)
                    {
                        synchronousResult = 0;
                        return Task.FromException<int>(exc);
                    }
                    finally
                    {
                        _asyncState.Release();
                    }
                }
            }

            // Otherwise, issue the whole request asynchronously.
            synchronousResult = 0;
            _asyncState.Memory = destination;
            return waitTask.ContinueWith(static (t, s) =>
            {
                // The options available on Unix for writing asynchronously to an arbitrary file
                // handle typically amount to just using another thread to do the synchronous write,
                // which is exactly  what this implementation does. This does mean there are subtle
                // differences in certain FileStream behaviors between Windows and Unix when multiple
                // asynchronous operations are issued against the stream to execute concurrently; on
                // Unix the operations will be serialized due to the usage of a semaphore, but the
                // position /length information won't be updated until after the write has completed,
                // whereas on Windows it may happen before the write has completed.

                Debug.Assert(t.Status == TaskStatus.RanToCompletion);
                var thisRef = (FileStream)s!;
                Debug.Assert(thisRef._asyncState != null);
                try
                {
                    Memory<byte> memory = thisRef._asyncState.Memory;
                    thisRef._asyncState.Memory = default;
                    return thisRef.ReadSpan(memory.Span);
                }
                finally { thisRef._asyncState.Release(); }
            }, this, CancellationToken.None, TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default);
        }

        /// <summary>Reads from the file handle into the buffer, overwriting anything in it.</summary>
        private int FillReadBufferForReadByte()
        {
            _asyncState?.Wait();
            try { return ReadNative(_buffer); }
            finally { _asyncState?.Release(); }
        }

        /// <summary>Writes a block of bytes to the file stream.</summary>
        /// <param name="source">The buffer containing data to write to the stream.</param>
        private void WriteSpan(ReadOnlySpan<byte> source)
        {
            PrepareForWriting();

            // If no data is being written, nothing more to do.
            if (source.Length == 0)
            {
                return;
            }

            // If there's already data in our write buffer, then we need to go through
            // our buffer to ensure data isn't corrupted.
            if (_writePos > 0)
            {
                // If there's space remaining in the buffer, then copy as much as
                // we can from the user's buffer into ours.
                int spaceRemaining = _bufferLength - _writePos;
                if (spaceRemaining >= source.Length)
                {
                    source.CopyTo(GetBuffer().AsSpan(_writePos));
                    _writePos += source.Length;
                    return;
                }
                else if (spaceRemaining > 0)
                {
                    source.Slice(0, spaceRemaining).CopyTo(GetBuffer().AsSpan(_writePos));
                    _writePos += spaceRemaining;
                    source = source.Slice(spaceRemaining);
                }

                // At this point, the buffer is full, so flush it out.
                FlushWriteBuffer();
            }

            // Our buffer is now empty.  If using the buffer would slow things down (because
            // the user's looking to write more data than we can store in the buffer),
            // skip the buffer.  Otherwise, put the remaining data into the buffer.
            Debug.Assert(_writePos == 0);
            if (source.Length >= _bufferLength)
            {
                WriteNative(source);
            }
            else
            {
                source.CopyTo(new Span<byte>(GetBuffer()));
                _writePos = source.Length;
            }
        }

        /// <summary>Unbuffered, writes a block of bytes to the file stream.</summary>
        /// <param name="source">The buffer containing data to write to the stream.</param>
        private unsafe void WriteNative(ReadOnlySpan<byte> source)
        {
			throw new NotImplementedException();
        }

        /// <summary>
        /// Asynchronously writes a sequence of bytes to the current stream, advances
        /// the current position within this stream by the number of bytes written, and
        /// monitors cancellation requests.
        /// </summary>
        /// <param name="source">The buffer to write data from.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous write operation.</returns>
        private ValueTask WriteAsyncInternal(ReadOnlyMemory<byte> source, CancellationToken cancellationToken)
        {
            Debug.Assert(_useAsyncIO);
            Debug.Assert(_asyncState != null);

            if (cancellationToken.IsCancellationRequested)
                return ValueTask.FromCanceled(cancellationToken);

            if (_fileHandle.IsClosed)
                throw Error.GetFileNotOpen();

            if (!CanWrite) // match Windows behavior; this gets thrown synchronously
            {
                throw Error.GetWriteNotSupported();
            }

            // Serialize operations using the semaphore.
            Task waitTask = _asyncState.WaitAsync(cancellationToken);

            // If we got ownership immediately, and if there's enough space in our buffer
            // to buffer the entire write request, then do so and we're done.
            if (waitTask.Status == TaskStatus.RanToCompletion)
            {
                int spaceRemaining = _bufferLength - _writePos;
                if (spaceRemaining >= source.Length)
                {
                    try
                    {
                        PrepareForWriting();

                        source.Span.CopyTo(new Span<byte>(GetBuffer(), _writePos, source.Length));
                        _writePos += source.Length;

                        return default;
                    }
                    catch (Exception exc)
                    {
                        return ValueTask.FromException(exc);
                    }
                    finally
                    {
                        _asyncState.Release();
                    }
                }
            }

            // Otherwise, issue the whole request asynchronously.
            _asyncState.ReadOnlyMemory = source;
            return new ValueTask(waitTask.ContinueWith(static (t, s) =>
            {
                // The options available on Unix for writing asynchronously to an arbitrary file
                // handle typically amount to just using another thread to do the synchronous write,
                // which is exactly  what this implementation does. This does mean there are subtle
                // differences in certain FileStream behaviors between Windows and Unix when multiple
                // asynchronous operations are issued against the stream to execute concurrently; on
                // Unix the operations will be serialized due to the usage of a semaphore, but the
                // position/length information won't be updated until after the write has completed,
                // whereas on Windows it may happen before the write has completed.

                Debug.Assert(t.Status == TaskStatus.RanToCompletion);
                var thisRef = (FileStream)s!;
                Debug.Assert(thisRef._asyncState != null);
                try
                {
                    ReadOnlyMemory<byte> readOnlyMemory = thisRef._asyncState.ReadOnlyMemory;
                    thisRef._asyncState.ReadOnlyMemory = default;
                    thisRef.WriteSpan(readOnlyMemory.Span);
                }
                finally { thisRef._asyncState.Release(); }
            }, this, CancellationToken.None, TaskContinuationOptions.DenyChildAttach, TaskScheduler.Default));
        }

        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) =>
            // Windows version overrides this method, so the Unix version does as well, but it doesn't
            // currently have any special optimizations to be done and so just calls to the base.
            base.CopyToAsync(destination, bufferSize, cancellationToken);

        /// <summary>Sets the current position of this stream to the given value.</summary>
        /// <param name="offset">The point relative to origin from which to begin seeking. </param>
        /// <param name="origin">
        /// Specifies the beginning, the end, or the current position as a reference
        /// point for offset, using a value of type SeekOrigin.
        /// </param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
			throw new NotImplementedException();
        }

        /// <summary>Sets the current position of this stream to the given value.</summary>
        /// <param name="fileHandle">The file handle on which to seek.</param>
        /// <param name="offset">The point relative to origin from which to begin seeking. </param>
        /// <param name="origin">
        /// Specifies the beginning, the end, or the current position as a reference
        /// point for offset, using a value of type SeekOrigin.
        /// </param>
        /// <returns>The new position in the stream.</returns>
        private long SeekCore(SafeFileHandle fileHandle, long offset, SeekOrigin origin)
        {
            Debug.Assert(!fileHandle.IsClosed && (GetType() != typeof(FileStream) || CanSeekCore(fileHandle))); // verify that we can seek, but only if CanSeek won't be a virtual call (which could happen in the ctor)
            Debug.Assert(origin >= SeekOrigin.Begin && origin <= SeekOrigin.End);

			throw new NotImplementedException();
        }

        private long CheckFileCall(long result, bool ignoreNotSupported = false)
        {
            if (result < 0)
            {
				throw new NotImplementedException();
            }

            return result;
        }

        private int CheckFileCall(int result, bool ignoreNotSupported = false)
        {
            CheckFileCall((long)result, ignoreNotSupported);

            return result;
        }

        /// <summary>State used when the stream is in async mode.</summary>
        private sealed class AsyncState : SemaphoreSlim
        {
            internal ReadOnlyMemory<byte> ReadOnlyMemory;
            internal Memory<byte> Memory;

            /// <summary>Initialize the AsyncState.</summary>
            internal AsyncState() : base(initialCount: 1, maxCount: 1) { }
        }
    }
}
