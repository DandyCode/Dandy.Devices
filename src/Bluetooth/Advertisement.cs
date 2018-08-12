using System;
using System.Collections.Generic;

namespace Dandy.Devices.Bluetooth
{
    /// <summary>
    /// Bluetooth Low Energy advertisement payload
    /// </summary>
    public sealed partial class Advertisement
    {
        Advertisement() => throw new NotSupportedException();

        /// <summary>
        /// The local name contained within the advertisement. This property can be either
        /// the shortened or complete local name defined by the Bluetooth LE specifications.
        /// </summary>
        public string LocalName => _get_LocalName();

        /// <summary>
        /// Gets a list of service UUIds.
        /// </summary>
        public IReadOnlyList<Guid> ServiceUuids => _get_ServiceUuids();
    }
}
