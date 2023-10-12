// <copyright file="InstrumentProfileReaderSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Ipf.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="InstrumentProfileReaderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_ipf.h"</c>.
/// </summary>
internal sealed unsafe class InstrumentProfileReaderSafeHandle : SafeHandleZeroIsInvalid
{
    private InstrumentProfileReaderSafeHandle(InstrumentProfileReaderHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator InstrumentProfileReaderHandle*(InstrumentProfileReaderSafeHandle value) =>
        (InstrumentProfileReaderHandle*)value.handle;

    public static InstrumentProfileReaderSafeHandle Create()
    {
        var thread = Isolate.CurrentThread;
        return new(ErrorCheck.NativeCall(thread, NativeCreate(thread)));
    }

    public long GetLastModified()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeGetLastModified(thread, this));
    }

    public bool WasComplete()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeWasComplete(thread, this)) != 0;
    }

    public ListNative<InstrumentProfileNative>* ReadFromFile(string address, string? user, string? password)
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeReadFromFile(thread, this, address, user, password));
    }

    public void IpfRelease(ListNative<InstrumentProfileNative>* ipf)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, IpfRelease(thread, ipf));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (InstrumentProfileReaderHandle*)handle));
            handle = (IntPtr)0;
            return true;
        }
        catch (Exception e)
        {
            // ToDo Add a log entry.
            Console.Error.WriteLine($"Exception in {GetType().Name} when releasing resource: {e}");
        }

        return false;
    }

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
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        InstrumentProfileReaderHandle* handle);

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
