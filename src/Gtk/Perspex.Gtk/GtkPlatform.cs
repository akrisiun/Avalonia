﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Perspex.Input.Platform;
using Perspex.Input;
using Perspex.Platform;
using Splat;

namespace Perspex.Gtk
{
    using Gtk = global::Gtk;

    public class GtkPlatform : IPlatformThreadingInterface, IPlatformSettings
    {
        private static // readonly 
            GtkPlatform s_instance; // = new GtkPlatform();

        public GtkPlatform()
        {
            Gtk.Application.Init();
        }

        public Size DoubleClickSize { get { return new Size (4, 4); } }

        public TimeSpan DoubleClickTime { get { return TimeSpan.FromMilliseconds (Gtk.Settings.Default.DoubleClickTime); } }

        public static void Initialize()
        {
            var version = Gtk.CoreGtk3.Instance.ToString();
            // var type = GLib.Object.GType;
            s_instance = new GtkPlatform();

            var locator = Locator.CurrentMutable;
            locator.Register(() => new PclPlatformWrapper(), typeof(IPclPlatformWrapper));
            locator.Register(() => new WindowImpl(), typeof(IWindowImpl));
            locator.Register(() => new PopupImpl(), typeof(IPopupImpl));
            locator.Register(() => new ClipboardImpl(), typeof(IClipboard));
            locator.Register(() => CursorFactory.Instance, typeof(IStandardCursorFactory));
            locator.Register(() => GtkKeyboardDevice.Instance, typeof(IKeyboardDevice));
            locator.Register(() => s_instance, typeof(IPlatformSettings));
            locator.Register(() => s_instance, typeof(IPlatformThreadingInterface));
            locator.RegisterConstant(new AssetLoader(), typeof(IAssetLoader));
        }

        public bool HasMessages()
        {
            return Gtk.Application.EventsPending();
        }

        public void ProcessMessage()
        {
            Gtk.Application.RunIteration();
        }

        public IDisposable StartTimer(TimeSpan interval, Action tick)
        {
            var result = true;
            var handle = GLib.Timeout.Add(
                (uint)interval.TotalMilliseconds,
                () =>
                {
                    tick();
                    return result;
                });

            return Disposable.Create(() => result = false);
        }

        public void Wake()
        {
        }
    }
}