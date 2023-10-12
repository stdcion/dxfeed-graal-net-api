// <copyright file="InstrumentProfileUpdateListenerSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Endpoint.Handles;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

internal unsafe class InstrumentProfileUpdateListenerSafeHandle : JavaFinalizeSafeHandle
{
    private InstrumentProfileUpdateListenerSafeHandle(InstrumentProfileUpdateListenerHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator InstrumentProfileUpdateListenerHandle*(InstrumentProfileUpdateListenerSafeHandle value) =>
        (InstrumentProfileUpdateListenerHandle*)value.handle;

    public static InstrumentProfileUpdateListenerSafeHandle Create(
        delegate* unmanaged[Cdecl]<nint, IterableInstrumentProfileHandle*, void> listener,
        GCHandle userData) =>
        new(ErrorCheck.NativeCall(
            CurrentThread,
            NativeCreate(CurrentThread, listener, GCHandle.ToIntPtr(userData))));

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileUpdateListener_new")]
    private static extern InstrumentProfileUpdateListenerHandle* NativeCreate(
        nint thread,
        delegate* unmanaged[Cdecl]<nint, IterableInstrumentProfileHandle*, void> listenerFunc,
        nint userData);
}
