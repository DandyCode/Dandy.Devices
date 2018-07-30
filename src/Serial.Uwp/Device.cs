using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Storage.Streams;

namespace Dandy.Devices.Serial.Uwp
{
    /// <summary>
    /// Wrapper around Windows.Devices.SerialCommunication.SerialDevice.
    /// </summary>
    public sealed class Device : Serial.Device
    {
        private readonly SerialDevice device;

        /// <inheritdoc/>
        public override uint BaudRate { get => device.BaudRate; set => device.BaudRate = value; }

        /// <inheritdoc/>
        public override Stream InputStream => lazyInputStream.Value;
        readonly Lazy<Stream> lazyInputStream;

        /// <inheritdoc/>
        public override Stream OutputStream => lazyOutputStream.Value;
        readonly Lazy<Stream> lazyOutputStream;

        internal Device(SerialDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            lazyInputStream = new Lazy<Stream>(() => new SerialInputStream(device));
            lazyOutputStream = new Lazy<Stream>(() => new SerialOutputStream(device));
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            device.Dispose();
        }
    }

    /// <summary>
    /// device.InputStream.AsStreamForRead() doesn't work correctly (throws
    /// NotImplementedException when reading), so have to implement this
    /// ourselves. See WinRtToNetFxStreamAdapter for more insights on how
    /// this could be improved.
    /// https://github.com/dotnet/corefx/blob/e0ba7aa8026280ee3571179cc06431baf1dfaaac/src/System.Runtime.WindowsRuntime/src/System/IO/WinRtToNetFxStreamAdapter.cs
    ///
    /// Also filed a bug about this:
    /// https://github.com/dotnet/corefx/issues/31460
    /// </summary>
    sealed class SerialInputStream : Stream
    {
        private readonly SerialDevice device;
        private readonly DataReader reader;

        public override bool CanRead => true;

        public override bool CanSeek => false;

        public override bool CanWrite => false;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override bool CanTimeout => true;

        public override int ReadTimeout {
            get => (int)device.ReadTimeout.TotalMilliseconds;
            set => device.ReadTimeout = TimeSpan.FromMilliseconds(value);
        }

        public SerialInputStream(SerialDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            reader = new DataReader(device.InputStream) {
                InputStreamOptions = InputStreamOptions.Partial
            };
        }

        protected override void Dispose(bool disposing)
        {
            reader?.Dispose();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            // API docs don't say that we can throw NotSupportedException, so just do nothing
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return Task.Run(() => ReadAsync(buffer, offset, count)).GetAwaiter().GetResult();
        }

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (buffer == null) {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (offset + count > buffer.Length) {
                throw new ArgumentException();
            }
            cancellationToken.ThrowIfCancellationRequested();
            var readCount = await reader.LoadAsync((uint)count).AsTask(cancellationToken);
            var buf = reader.ReadBuffer(readCount);
            buf.CopyTo(0, buffer, offset, (int)readCount);
            return (int)readCount;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }

    sealed class SerialOutputStream : Stream
    {
        private readonly SerialDevice device;
        private readonly DataWriter writer;

        public override bool CanRead => false;

        public override bool CanSeek => false;

        public override bool CanWrite => true;

        public override long Length => throw new NotSupportedException();

        public override long Position { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }

        public override bool CanTimeout => true;

        public override int WriteTimeout {
            get => (int)device.WriteTimeout.TotalMilliseconds;
            set => device.WriteTimeout = TimeSpan.FromMilliseconds(value);
        }

        public SerialOutputStream(SerialDevice device)
        {
            this.device = device ?? throw new ArgumentNullException(nameof(device));
            writer = new DataWriter(device.OutputStream);
        }

        protected override void Dispose(bool disposing)
        {
            writer?.Dispose();
            base.Dispose(disposing);
        }

        public override void Flush()
        {
            Task.Run(() => FlushAsync()).GetAwaiter().GetResult();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            // have to do both? https://stackoverflow.com/questions/36063898/communicate-between-my-uwp-app-and-my-rfxtrx433e?rq=1
            return writer.StoreAsync().AsTask(cancellationToken);
            // FlushAsync throws NotImplmentedException
            //await writer.FlushAsync().AsTask(cancellationToken);;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (buffer == null) {
                throw new ArgumentNullException(nameof(buffer));
            }
            if (offset < 0) {
                throw new ArgumentOutOfRangeException(nameof(buffer));
            }
            if (count < 0) {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            if (offset + count > buffer.Length) {
                throw new ArgumentException();
            }
            var buf = buffer.AsBuffer();
            writer.WriteBuffer(buf, (uint)offset, (uint)count);
        }

        public override void WriteByte(byte value)
        {
            writer.WriteByte(value);
        }
    }
}
