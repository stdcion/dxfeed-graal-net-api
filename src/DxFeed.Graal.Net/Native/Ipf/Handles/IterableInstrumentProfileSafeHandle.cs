// <copyright file="IterableInstrumentProfileSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

internal unsafe class IterableInstrumentProfileSafeHandle : JavaSafeHandle
{
    public IterableInstrumentProfileSafeHandle(IterableInstrumentProfileHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator IterableInstrumentProfileHandle*(IterableInstrumentProfileSafeHandle value) =>
        (IterableInstrumentProfileHandle*)value.handle;

    public bool HasNext() =>
        ErrorCheck.NativeCall(CurrentThread, NativeHasNext(CurrentThread, this)) != 0;

    public InstrumentProfile? Next()
    {
        var i = NativeNext(CurrentThread, this);
        return i != null ? InstrumentProfileMapper.Convert(i) : null;
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_Iterable_InstrumentProfile_hasNext")]
    private static extern int NativeHasNext(
        nint thread,
        IterableInstrumentProfileHandle* iterable);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_Iterable_InstrumentProfile_hasNext")]
    private static extern InstrumentProfileNative* NativeNext(
        nint thread,
        IterableInstrumentProfileHandle* iterable);
}
