// <copyright file="DXFeed.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using DxFeed.Graal.Net.Native.Feed;
using DxFeed.Graal.Net.Utils;

#pragma warning disable CS1591

namespace DxFeed.Graal.Net.Api;

public class DXFeed
{
    private readonly FeedNative _feedNative;
    private readonly ConcurrentSet<DXFeedSubscription> _attached = new();

    internal DXFeed(FeedNative feedNative) =>
        _feedNative = feedNative;

    public static DXFeed GetInstance() =>
        DXEndpoint.GetInstance().GetFeed();

    public DXFeedSubscription CreateSubscription(params Type[] eventTypes)
    {
        DXFeedSubscription subscription = new(eventTypes);
        subscription.Attach(this);
        return subscription;
    }

    public DXFeedSubscription CreateSubscription(IEnumerable<Type> eventTypes) =>
        CreateSubscription(eventTypes.ToArray());

    public void AttachSubscription(DXFeedSubscription subscription)
    {
        if (!_attached.Add(subscription))
        {
            return;
        }

        subscription.Attach(this);
    }

    public void DetachSubscription(DXFeedSubscription subscription)
    {
        if (!_attached.Remove(subscription))
        {
            return;
        }

        subscription.Detach(this);
    }

    internal void AttachInternal(DXFeedSubscription subscription) =>
        _attached.Add(subscription);

    internal void DetachInternal(DXFeedSubscription subscription) =>
        _attached.Remove(subscription);

    internal void Close()
    {
        foreach (var attached in _attached)
        {
            attached.Detach(this);
        }

        _attached.Clear();
    }
}
