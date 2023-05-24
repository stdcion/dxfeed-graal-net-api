// <copyright file="EventListenerSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Endpoint.Handles;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;

namespace DxFeed.Graal.Net.Native.Subscription.Handles;

/// <summary>
/// This class wraps an unsafe handler <see cref="BuilderHandle"/>.
/// The location of the imported functions is in the header files <c>"dxfg_endpoint.h"</c>.
/// </summary>
internal sealed unsafe class EventListenerSafeHandle : SafeHandleZeroIsInvalid
{
    private EventListenerSafeHandle(EventListenerHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator EventListenerHandle*(EventListenerSafeHandle value) =>
        (EventListenerHandle*)value.handle;

    public static EventListenerSafeHandle Create(
        delegate* unmanaged[Cdecl]<nint, ListNative<EventTypeNative>*, nint, void> listener,
        GCHandle userData)
    {
        var thread = Isolate.CurrentThread;
        return new(ErrorCheck.NativeCall(
            thread,
            NativeCreate(thread, listener, GCHandle.ToIntPtr(userData))));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (EventListenerHandle*)handle));
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
        EntryPoint = "dxfg_DXFeedEventListener_new")]
    private static extern EventListenerHandle* NativeCreate(
        nint thread,
        delegate* unmanaged[Cdecl]<nint, ListNative<EventTypeNative>*, nint, void> listenerFunc,
        nint userData);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        EventListenerHandle* listenerHandle);
}
