// <copyright file="Program.cs" company="Devexperts LLC">
// Copyright © 2022 Devexperts LLC. All rights reserved.
// This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
// If a copy of the MPL was not distributed with this file, You can obtain one at http://mozilla.org/MPL/2.0/.
// </copyright>

using System;
using System.Threading;
using DxFeed.Graal.Net.Api;
using DxFeed.Graal.Net.Events.Market;
using DxFeed.Graal.Net.Tools.Connect;
using DxFeed.Graal.Net.Tools.Dump;
using DxFeed.Graal.Net.Tools.PerfTest;
using DxFeed.Graal.Net.Utils;

namespace DxFeed.Graal.Net.Tools;

public class Sub
{
    public event EventHandler? CloseEvent;

    public void Attach(Feed feed) =>
        feed.OnClose += OnClose;

    public void Detach(Feed feed) =>
        feed.OnClose -= OnClose;

    public void Close()
    {
    }

    public void OnClose(object? sender, EventArgs args)
    {
        if (sender is Feed feed)
        {
            Detach(feed);
        }
    }
}

public class Feed
{
    public event EventHandler? OnClose;

    public void Close() =>
        OnClose?.Invoke(this, EventArgs.Empty);
}

internal abstract class Program
{
    public static void Main(string[] args)
    {
        int aaa = DayUtil.GetYearMonthDayByDayId(19457);
        var feed = DXEndpoint.Create().GetFeed();
        var sub = new DXFeedSubscription(typeof(Quote));

        new Thread(() =>
        {
            while (true)
            {
                feed.AttachSubscription(sub);
                Console.WriteLine("!!!!");
                feed.DetachSubscription(sub);
            }
        }).Start();
        new Thread(() =>
        {
            while (true)
            {
                sub.Attach(feed);
                Console.WriteLine("&&&");
                sub.Detach(feed);
            }
        }).Start();
        while (true)
        {
            Thread.Sleep(1000);
        }
        return;
        var cmdArgs = new ProgramArgs().ParseArgs(args);
        if (cmdArgs == null)
        {
            return;
        }

        switch (cmdArgs.Tool)
        {
            case Tools.Connect:
                ConnectTool.Run(args.AsSpan()[1..].ToArray());
                break;
            case Tools.Dump:
                DumpTool.Run(args.AsSpan()[1..].ToArray());
                break;
            case Tools.PerfTest:
                PerfTestTool.Run(args.AsSpan()[1..].ToArray());
                break;
        }
    }
}
