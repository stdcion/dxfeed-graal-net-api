// <copyright file="InstrumentProfileReaderSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="InstrumentProfileReaderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_ipf.h"</c>.
/// </summary>
internal sealed unsafe class InstrumentProfileReaderSafeHandle : JavaSafeHandle
{
    private InstrumentProfileReaderSafeHandle(InstrumentProfileReaderHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator InstrumentProfileReaderHandle*(InstrumentProfileReaderSafeHandle value) =>
        (InstrumentProfileReaderHandle*)value.handle;

    public static InstrumentProfileReaderSafeHandle Create() =>
        new(ErrorCheck.NativeCall(CurrentThread, NativeCreate(CurrentThread)));

    public long GetLastModified() =>
        ErrorCheck.NativeCall(CurrentThread, NativeGetLastModified(CurrentThread, this));

    public bool WasComplete() =>
        ErrorCheck.NativeCall(CurrentThread, NativeWasComplete(CurrentThread, this)) != 0;

    public ListNative<InstrumentProfileNative>* ReadFromFile(string address, string? user, string? password) =>
        ErrorCheck.NativeCall(CurrentThread, NativeReadFromFile(CurrentThread, this, address, user, password));

    public void IpfRelease(ListNative<InstrumentProfileNative>* ipf) =>
        ErrorCheck.NativeCall(CurrentThread, IpfRelease(CurrentThread, ipf));

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_resolveSourceURL")]
    private static extern nint NativeResolveSourceUrl(
        nint thread,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_new")]
    private static extern InstrumentProfileReaderHandle* NativeCreate(nint thread);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_getLastModified")]
    private static extern long NativeGetLastModified(
        nint thread,
        InstrumentProfileReaderHandle* handle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_InstrumentProfileReader_wasComplete")]
    private static extern int NativeWasComplete(
        nint thread,
        InstrumentProfileReaderHandle* handle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_InstrumentProfileReader_readFromFile2")]
    private static extern ListNative<InstrumentProfileNative>* NativeReadFromFile(
        nint thread,
        InstrumentProfileReaderHandle* handle,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string address,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? user,
        [MarshalAs(UnmanagedType.LPUTF8Str)] string? password);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        ExactSpelling = true,
        BestFitMapping = false,
        ThrowOnUnmappableChar = true,
        EntryPoint = "dxfg_CList_InstrumentProfile_release")]
    private static extern int IpfRelease(
        nint thread,
        ListNative<InstrumentProfileNative>* ipf);
}
