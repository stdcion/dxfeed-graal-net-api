// <copyright file="FeedSafeHandle.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.Endpoint.Handles;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Subscription;
using DxFeed.Graal.Net.Native.Subscription.Handles;

namespace DxFeed.Graal.Net.Native.Feed.Handles;

internal sealed unsafe class FeedSafeHandle : SafeHandleZeroIsInvalid
{
    public FeedSafeHandle(FeedHandle* handle) =>
        SetHandle((nint)handle);

    public static implicit operator FeedHandle*(FeedSafeHandle value) =>
        (FeedHandle*)value.handle;

    public void AttachSubscription(
        SubscriptionSafeHandle subHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeAttachSubscription(thread, this, subHandle));
    }

    public void DetachSubscription(
        SubscriptionSafeHandle subHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeDetachSubscription(thread, this, subHandle));
    }

    public void DetachSubscriptionAndClear(
        SubscriptionSafeHandle subHandle)
    {
        var thread = Isolate.CurrentThread;
        ErrorCheck.NativeCall(thread, NativeDetachSubscriptionAndClear(thread, this, subHandle));
    }

    protected override bool ReleaseHandle()
    {
        try
        {
            var thread = Isolate.CurrentThread;
            ErrorCheck.NativeCall(thread, NativeRelease(thread, (BuilderHandle*)handle));
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
        EntryPoint = "dxfg_JavaObjectHandler_release")]
    private static extern int NativeRelease(
        nint thread,
        BuilderHandle* builderHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeed_attachSubscription")]
    private static extern int NativeAttachSubscription(
        nint thread,
        FeedHandle* feedHandle,
        SubscriptionHandle* subHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeed_detachSubscription")]
    private static extern int NativeDetachSubscription(
        nint thread,
        FeedHandle* feedHandle,
        SubscriptionHandle* subHandle);

    [DllImport(
        ImportInfo.DllName,
        CallingConvention = CallingConvention.Cdecl,
        CharSet = CharSet.Ansi,
        EntryPoint = "dxfg_DXFeed_detachSubscriptionAndClear")]
    private static extern int NativeDetachSubscriptionAndClear(
        nint thread,
        FeedHandle* feedHandle,
        SubscriptionHandle* subHandle);
}
