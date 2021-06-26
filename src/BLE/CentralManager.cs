// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    /// <summary>
    /// Manages scanning and connecting to peripherals as a central.
    /// </summary>
    public sealed partial class CentralManager : IAsyncDisposable
    {
        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <returns>The new instance.</returns>
        public static partial Task<CentralManager> NewAsync();

        /// <summary>
        /// Starts scanning for peripherals.
        /// </summary>
        /// <param name="observer">
        /// An observer that receives callbacks when advertising data is received.
        /// </param>
        /// <param name="uuids">
        /// An optional list of service UUIDs to filter on.
        /// </param>
        /// <param name="filterDuplicates">
        /// If <c>false</c>, duplicate advertising data will be received.
        /// </param>
        /// <returns>
        /// An object that must be disposed to stop scanning.
        /// </returns>
        public partial Task<IAsyncDisposable> ScanAsync(
            IObserver<AdvertisementData> observer,
            IEnumerable<Guid>? uuids = null,
            bool filterDuplicates = true);

        /// <summary>
        /// Tries to connect to a BLE device.
        /// </summary>
        /// <param name="id">
        /// The OS-specific ID of the device (from <see cref="AdvertisementData"/>>.
        /// </param>
        /// <param name="token">An optional cancellation token.</param>
        /// <returns>The connected peripheral.</returns>
        /// <remarks>
        /// It is highly recommended to supply a cancellation token since this
        /// method will never time out.
        /// </remarks>
        public partial Task<Peripheral> ConnectAsync(string id, CancellationToken token = default);

        /// <summary>
        /// Releases OS resources.
        /// </summary>
        public partial ValueTask DisposeAsync();
    }
}
