using System;
using System.Buffers;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Dandy.Devices.BluetoothLE
{
    class BufferMemoryManager : MemoryManager<byte>
    {
        private readonly IMemoryBufferReference reference;
        private readonly IMemoryBufferByteAccess byteAccess;
        private uint refCount = 1;
        private bool disposed;

        public BufferMemoryManager(IMemoryBuffer buffer)
        {
            reference = buffer?.CreateReference() ?? throw new ArgumentNullException(nameof(buffer));
            if (reference is IMemoryBufferByteAccess) {
                byteAccess = (IMemoryBufferByteAccess)reference;
            }
            else {
                reference.Dispose();
                throw new ArgumentException("buffer does not implment IMemoryBufferByteAccess");
            }
        }

        public override unsafe Span<byte> GetSpan()
        {
            lock (this) {
                if (disposed) {
                    throw new ObjectDisposedException(null);
                }
                byteAccess.GetBuffer(out var pointer, out var capacity);
                return new Span<byte>(pointer, (int)capacity);
            }
        }
        
        public override unsafe MemoryHandle Pin(int elementIndex = 0)
        {
            lock (this) {
                if (disposed) {
                    throw new ObjectDisposedException(null);
                }
                byteAccess.GetBuffer(out var pointer, out var capacity);
                if (elementIndex >= capacity) {
                    throw new ArgumentOutOfRangeException(nameof(elementIndex));
                }
                refCount++;
                return new MemoryHandle(pointer, GCHandle.Alloc(this), this);
            }
        }

        public override void Unpin()
        {
            lock (this) {
                if ((disposed && refCount == 0) || (!disposed && refCount <= 1)) {
                    throw new InvalidOperationException("Unbalanced calls to Pin and Unpin");
                }
                refCount--;
                if (refCount == 0) {
                    reference.Dispose();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (this) {
                if (!disposed) {
                    disposed = true;
                    refCount--;
                    if (refCount == 0) {
                        reference.Dispose();
                    }
                }
            }
        }
    }

    [ComImport]
    [Guid("5b0d3235-4dba-4d44-865e-8f1d0e4fd04d")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    unsafe interface IMemoryBufferByteAccess
    {
        void GetBuffer(out byte* buffer, out uint capacity);
    }

    static class IBufferExtensions
    {
        public static Memory<byte> ToMemory(this IBuffer buffer)
        {
            return new BufferMemoryManager(Windows.Storage.Streams.Buffer.CreateMemoryBufferOverIBuffer(buffer)).Memory;
        }
        public static ReadOnlyMemory<byte> ToReadOnlyMemory(this IBuffer buffer)
        {
            return buffer.ToMemory();
        }
    }
}
