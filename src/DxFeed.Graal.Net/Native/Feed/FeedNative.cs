// <copyright file="FeedNative.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using DxFeed.Graal.Net.Native.Feed.Handles;
using DxFeed.Graal.Net.Native.Subscription;

namespace DxFeed.Graal.Net.Native.Feed;

/// <summary>
/// Native wrapper over the Java <c>com.dxfeed.api.DXFeed</c> class.
/// The location of the imported functions is in the header files <c>"dxfg_feed.h"</c>.
/// </summary>
internal sealed unsafe class FeedNative : IDisposable
{
    private readonly FeedSafeHandle _feedHandle;

    public FeedNative(FeedHandle* feedHandle) =>
        _feedHandle = new FeedSafeHandle(feedHandle);

    public void AttachSubscription(SubscriptionNative subscriptionNative) =>
        _feedHandle.AttachSubscription(subscriptionNative);

    public void DetachSubscription(SubscriptionNative subscriptionNative) =>
        _feedHandle.DetachSubscription(subscriptionNative);

    public void DetachSubscriptionAndClear(SubscriptionNative subscriptionNative) =>
        _feedHandle.DetachSubscriptionAndClear(subscriptionNative);

    public FeedHandle* GetHandle() =>
        _feedHandle;

    public void Dispose() =>
        _feedHandle.Dispose();
}
