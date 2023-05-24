// <copyright file="SubscriptionNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DxFeed.Graal.Net.Native.ErrorHandling;
using DxFeed.Graal.Net.Native.Events;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Native.Feed.Handles;
using DxFeed.Graal.Net.Native.Graal;
using DxFeed.Graal.Net.Native.Interop;
using DxFeed.Graal.Net.Native.Subscription.Handles;
using DxFeed.Graal.Net.Native.SymbolMappers;
using DxFeed.Graal.Net.Native.Symbols;

namespace DxFeed.Graal.Net.Native.Subscription;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DxFeedSubscription</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_subscription.h"</c>.
/// </summary>
internal sealed class SubscriptionNative : IDisposable
{
    private readonly SubscriptionSafeHandle _subscriptionHandle;

    private SubscriptionNative(SubscriptionSafeHandle subscriptionHandle) =>
        _subscriptionHandle = subscriptionHandle;

    public static implicit operator SubscriptionSafeHandle(SubscriptionNative value) =>
        value._subscriptionHandle;

    public static SubscriptionNative Create(EventCodeNative eventCodes) =>
        new(SubscriptionSafeHandle.Create(eventCodes));

    public static SubscriptionNative Create(IEnumerable<EventCodeNative> eventCodes) =>
        new(SubscriptionSafeHandle.Create(eventCodes));

    public void Close() =>
        _subscriptionHandle.Close();

    public void AddSymbol(object symbol) =>
        _subscriptionHandle.AddSymbol(symbol);

    public void AddSymbols(object[] symbol) =>
        _subscriptionHandle.AddSymbols(symbol);

    public void RemoveSymbol(object symbol) =>
        _subscriptionHandle.RemoveSymbol(symbol);

    public void RemoveSymbols(object[] symbol) =>
        _subscriptionHandle.RemoveSymbols(symbol);

    public void Attach(FeedNative feed)
    {
        //_subscriptionHandle.Attach(feed);
        feed.AttachSubscription(this);
    }

    public void Detach(FeedNative feed) =>
        feed.DetachSubscription(this);

    public void Clear() =>
        _subscriptionHandle.Clear();

    public void Dispose() =>
        Close();
}
