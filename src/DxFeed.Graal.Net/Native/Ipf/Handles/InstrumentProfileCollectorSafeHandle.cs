// <copyright file="InstrumentProfileCollectorSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

internal unsafe class InstrumentProfileCollectorSafeHandle : JavaSafeHandle
{
    private InstrumentProfileCollectorSafeHandle(InstrumentProfileCollectorHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator InstrumentProfileCollectorHandle*(InstrumentProfileCollectorSafeHandle value) =>
        (InstrumentProfileCollectorHandle*)value.handle;

    public static InstrumentProfileCollectorSafeHandle Create() =>
        new(ErrorCheck.NativeCall(CurrentThread, NativeCreate(CurrentThread)));

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_new")]
    private static extern InstrumentProfileCollectorHandle* NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileCollector_addUpdateListener")]
    private static extern int NativeAddUpdateListener(
        nint thread,
        InstrumentProfileCollectorHandle* endpointHandle,
        InstrumentProfileUpdateListenerHandle* listenerHandle);
}
