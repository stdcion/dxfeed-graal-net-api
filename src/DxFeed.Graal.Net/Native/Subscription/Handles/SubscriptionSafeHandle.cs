// <copyright file="SubscriptionSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Feed.Handles;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.SymbolMappers;
using DxFeed.Graal.Net.Native.Symbols;

namespace DxFeed.Graal.Net.Native.Subscription.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="SubscriptionHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class SubscriptionSafeHandle : SafeHandleZeroIsInvalid
{
    private SubscriptionSafeHandle(SubscriptionHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator SubscriptionHandle*(SubscriptionSafeHandle value) =>
        (SubscriptionHandle*)value.handle;

    public static SubscriptionSafeHandle Create(EventCodeNative eventCode)
    {
        var thread = Isolate.CurrentThread;
        return new(ErrorCheck.NativeCall(thread, NativeCreate(thread, eventCode)));
    }

    public static SubscriptionSafeHandle Create(IEnumerable<EventCodeNative> eventCodes)
    {
        var thread = Isolate.CurrentThread;
        var codes = (ListNative<EventCodeNative>*)0;
        try
        {
            codes = ListNative<EventCodeNative>.Create(eventCodes);
            return new(ErrorCheck.NativeCall(thread, NativeCreate(thread, codes)));
        }
        finally
        {
            ListNative<EventCodeNative>.Release(codes);
        }
    }

    public new void Close()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeClose(thread, this));
    }

    public void AddEventListener(
        EventListenerSafeHandle eventListenerHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeAddEventListener(thread, this, eventListenerHandle, 0, 0));
    }

    public void RemoveEventListener(
        EventListenerSafeHandle eventListenerHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeRemoveEventListener(thread, this, eventListenerHandle));
    }

    public void AddSymbol(object symbol)
    {
        var thread = Isolate.CurrentThread;
        var symbolNative = (SymbolNative*)0;
        try
        {
            symbolNative = SymbolMapper.CreateNative(symbol);
            ErrorCheck.NativeCall(thread, NativeAddSymbol(thread, this, symbolNative));
        }
        finally
        {
            SymbolMapper.ReleaseNative(symbolNative);
        }
    }

    public void AddSymbols(object[] symbol)
    {
        var thread = Isolate.CurrentThread;
        var symbolNative = SymbolMapper.CreateNative<SymbolNative>(symbol);
        try
        {
            ErrorCheck.NativeCall(thread, NativeAddSymbols(thread, this, symbolNative));
        }
        finally
        {
            // ToDo Add release.
            //// SymbolMapper.ReleaseNative(symbolNative);
        }
    }

    public void RemoveSymbol(object symbol)
    {
        var thread = Isolate.CurrentThread;
        var symbolNative = (SymbolNative*)0;
        try
        {
            symbolNative = SymbolMapper.CreateNative(symbol);
            ErrorCheck.NativeCall(thread, NativeRemoveSymbol(thread, this, symbolNative));
        }
        finally
        {
            SymbolMapper.ReleaseNative(symbolNative);
        }
    }

    public void RemoveSymbols(object[] symbol)
    {
        var thread = Isolate.CurrentThread;
        var symbolNative = SymbolMapper.CreateNative<SymbolNative>(symbol);
        try
        {
            ErrorCheck.NativeCall(thread, NativeRemoveSymbols(thread, this, symbolNative));
        }
        finally
        {
            // ToDo Add release.
            //SymbolMapper.ReleaseNative(symbolNative);
        }
    }

    public void Clear()
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeClear(thread, this));
    }

    public void Attach(FeedSafeHandle feedHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeAttach(thread, this, feedHandle));
    }

    public void Detach(FeedSafeHandle feedHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeDetach(thread, this, feedHandle));
    }

    public bool IsClosed()
    {
        var thread = Isolate.CurrentThread;
        return ErrorCheck.NativeCall(thread, NativeIsClosed(thread, this)) != 0;
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (SubscriptionHandle*)handle));
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
        EntryPoint = "dxfg_DXFeedSubscription_new")]
    private static extern SubscriptionHandle* NativeCreate(
        nint thread,
        EventCodeNative eventCode);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_new2")]
    private static extern SubscriptionHandle* NativeCreate(
        nint thread,
        ListNative<EventCodeNative>* eventCodes);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        SubscriptionHandle* subHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_close")]
    private static extern int NativeClose(
        nint thread,
        SubscriptionHandle* subHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_addEventListener")]
    private static extern int NativeAddEventListener(
        nint thread,
        SubscriptionHandle* subHandle,
        EventListenerHandle* listenerHandle,
        nint endpointFinalize,
        nint userData);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_removeEventListener")]
    private static extern int NativeRemoveEventListener(
        nint thread,
        SubscriptionHandle* subHandle,
        EventListenerHandle* listenerHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_addSymbol")]
    private static extern int NativeAddSymbol(
        nint thread,
        SubscriptionHandle* subHandle,
        SymbolNative* symbol);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_addSymbols")]
    private static extern int NativeAddSymbols(
        nint thread,
        SubscriptionHandle* subHandle,
        ListNative<SymbolNative>* symbols);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_removeSymbol")]
    private static extern int NativeRemoveSymbol(
        nint thread,
        SubscriptionHandle* subHandle,
        SymbolNative* symbol);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_removeSymbols")]
    private static extern int NativeRemoveSymbols(
        nint thread,
        SubscriptionHandle* subHandle,
        ListNative<SymbolNative>* symbols);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_clear")]
    private static extern int NativeClear(
        nint thread,
        SubscriptionHandle* subHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_attach")]
    private static extern int NativeAttach(
        nint thread,
        SubscriptionHandle* subHandle,
        FeedHandle* feedHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_detach")]
    private static extern int NativeDetach(
        nint thread,
        SubscriptionHandle* subHandle,
        FeedHandle* feedHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeedSubscription_isClosed")]
    private static extern int NativeIsClosed(
        nint thread,
        SubscriptionHandle* subHandle);
}
