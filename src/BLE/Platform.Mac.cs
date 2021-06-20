// SPDX-License-Identifier: MIT
// Copyright (c) 2021 David Lechner <david@lechnology.com>

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using CoreBluetooth;
using Foundation;
using ObjCRuntime;

namespace Dandy.Devices.BLE
{
    internal static class Platform
    {
        [ModuleInitializer]
        internal static void Init()
        {
            // HACK: can't use NSApplication.Init() because it messes with the synchronization context
            typeof(Runtime).GetMethod("EnsureInitialized", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, null);
            typeof(Runtime).GetMethod("RegisterAssemblies", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, null);
        }

        public static unsafe NSData MemoryToNSData(ReadOnlyMemory<byte> memory)
        {
            var handle = memory.Pin();
            return new NSData((IntPtr)handle.Pointer, (nuint)memory.Length, (_, _) => handle.Dispose());
        }

        public static Guid CBUuidToGuid(CBUUID uuid)
        {
            var str = uuid.Uuid;

            if (str.Length == 4) {
                return Uuid.From16(ushort.Parse(str, NumberStyles.HexNumber));
            }

            return new(str);
        }
    }
}
