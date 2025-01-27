// <copyright file="InstrumentProfileReaderNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Threading;
using DxFeed.Graal.Net.Ipf;
using DxFeed.Graal.Net.Native.Interop;
using Microsoft.Win32.SafeHandles;
using static DxFeed.Graal.Net.Native.ErrorHandling.ErrorCheck;

namespace DxFeed.Graal.Net.Native.Ipf;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "a")]
internal sealed class InstrumentProfileReaderNative : JavaSafeHandle
{
    public static string? ResolveSourceUrl(string address)
    {
        using var str = NativeCall(CurrentThread, NativeResolveSourceUrl(CurrentThread, address));
        return str.ToString();
    }

    public static InstrumentProfileReaderNative Create() =>
        NativeCall(CurrentThread, NativeCreate(CurrentThread));

    public long GetLastModified() =>
        NativeCall(CurrentThread, NativeGetLastModified(CurrentThread, this));

    public bool WasComplete() =>
        NativeCall(CurrentThread, NativeWasComplete(CurrentThread, this)) != 0;

    public List<InstrumentProfile> ReadFromFile(string address, string? user, string? password)
    {
        using var result = NativeCall(CurrentThread, NativeReadFromFile(CurrentThread, this, address, user, password));
        return result.ToList();
    }

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_resolveSourceURL")]
    private static extern JavaStringSafeHandle NativeResolveSourceUrl(
        nint thread,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_new")]
    private static extern InstrumentProfileReaderNative NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_getLastModified")]
    private static extern long NativeGetLastModified(
        nint thread,
        InstrumentProfileReaderNative reader);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_wasComplete")]
    private static extern int NativeWasComplete(
        nint thread,
        InstrumentProfileReaderNative reader);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_readFromFile2")]
    private static extern InstrumentProfileListNative NativeReadFromFile(
        nint thread,
        InstrumentProfileReaderNative reader,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? user,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? password);
}
