// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Dandy.Devices.BLE
{
    public sealed partial class CentralManager : IAsyncDisposable
    {
        public static partial Task<CentralManager> NewAsync();

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

        public partial Task<IAsyncDisposable> ScanAsync(
            IObserver<AdvertisementData> observer,
            IEnumerable<Guid>? uuids = null,
            bool filterDuplicates = true);
    }
}
