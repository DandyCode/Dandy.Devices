using System;
using System.IO;

namespace Dandy.Devices.Serial
{
    public interface IDevice : IDisposable
    {
        Stream InputStream { get; }
        Stream OutputStream { get; }
    }
}
