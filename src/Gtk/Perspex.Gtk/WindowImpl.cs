﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Gdk;
using Perspex.Controls;
using Perspex.Input.Raw;
using Perspex.Platform;
using Perspex.Input;
using Action = System.Action;

namespace Perspex.Gtk
{
    using Gtk = global::Gtk;

    public class WindowImpl : Gtk.Window, IWindowImpl
    {
        private TopLevel _owner;

        private IPlatformHandle _windowHandle;

        private Size _clientSize;

        private Gtk.IMContext _imContext;

        private uint _lastKeyEventTimestamp;

        private static readonly Gdk.Cursor DefaultCursor = new Gdk.Cursor(CursorType.LeftPtr);

        public WindowImpl()
            : base(Gtk.WindowType.Toplevel)
        {
            DefaultSize = new Gdk.Size(640, 480);
            Init();
        }

        public WindowImpl(Gtk.WindowType type)
            : base(type)
        {
            Init();
        }

        private void Init()
        {
            Events = EventMask.PointerMotionMask |
              EventMask.ButtonPressMask |
              EventMask.ButtonReleaseMask;
            _windowHandle = new PlatformHandle(Handle, "GtkWindow");
            _imContext = new Gtk.IMMulticontext();
            _imContext.Commit += ImContext_Commit;
        }

        public Size ClientSize
        {
            get;
            set;
        }

        public Size MaxClientSize
        {
            get
            {
                // TODO: This should take into account things such as taskbar and window border
                // thickness etc.
                return new Size(Screen.Width, Screen.Height);
            }
        }

        IPlatformHandle ITopLevelImpl.Handle { get { return  _windowHandle; } }

        public Action Activated { get; set; }

        public Action Closed { get; set; }

        public Action Deactivated { get; set; }

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect, IPlatformHandle> Paint { get; set; }

        public Action<Size> Resized { get; set; }

        public IPopupImpl CreatePopup()
        {
            return new PopupImpl();
        }

        public void Invalidate(Rect rect)
        {
            // base.GdkWindow.InvalidateRect (new Rectangle ((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height), true);

        }

        public Point PointToScreen(Point point)
        {
            int x = 0, y = 0;
            // GdkWindow.GetDeskrelativeOrigin(out x, out y);

            return new Point(point.X + x, point.Y + y);
        }

        public void SetOwner(TopLevel owner)
        {
            _owner = owner;
        }

        public void SetTitle(string title)
        {
            Title = title;
        }


        public void SetCursor(IPlatformHandle cursor)
        {
            // GdkWindow.Cursor = cursor != null ? new Gdk.Cursor(cursor.Handle) : DefaultCursor;
        }

        public IDisposable ShowDialog()
        {
            Modal = true;
            Show();

            return Disposable.Empty;
        }

        void ITopLevelImpl.Activate()
        {
            Activate();
        }

        private static ModifierKeys GetModifierKeys(ModifierType state)
        {
            var rv = ModifierKeys.None;
            if (state.HasFlag(ModifierType.ControlMask))
                rv |= ModifierKeys.Control;
            if (state.HasFlag(ModifierType.ShiftMask))
                rv |= ModifierKeys.Shift;
            if (state.HasFlag(ModifierType.Mod1Mask))
                rv |= ModifierKeys.Control;

            return rv;
        }

        protected override bool OnButtonPressEvent(EventButton evnt)
        {
            var e = new RawMouseEventArgs(
                GtkMouseDevice.Instance,
                evnt.Time,
                _owner,
                RawMouseEventType.LeftButtonDown,
                new Point(evnt.X, evnt.Y), GetModifierKeys(evnt.State));
            Input(e);
            return true;
        }

        protected override bool OnButtonReleaseEvent(EventButton evnt)
        {
            var e = new RawMouseEventArgs(
                GtkMouseDevice.Instance,
                evnt.Time,
                _owner,
                RawMouseEventType.LeftButtonUp,
                new Point(evnt.X, evnt.Y), GetModifierKeys(evnt.State));
            Input(e);
            return true;
        }

        protected override bool OnConfigureEvent(EventConfigure evnt)
        {
            var newSize = new Size(evnt.Width, evnt.Height);

            if (newSize != _clientSize)
            {
                Resized(newSize);
            }

            return true;
        }

        protected override void OnDestroyed()
        {
            Closed();
        }

        private bool ProcessKeyEvent(EventKey evnt)
        {
            _lastKeyEventTimestamp = evnt.Time;
            if (_imContext.FilterKeypress(evnt))
                return true;
            var e = new RawKeyEventArgs(
                GtkKeyboardDevice.Instance,
                evnt.Time,
                evnt.Type == EventType.KeyPress ? RawKeyEventType.KeyDown : RawKeyEventType.KeyUp,
                GtkKeyboardDevice.ConvertKey(evnt.Key), GetModifierKeys(evnt.State));
            Input(e);
            return true;
        }

        protected override bool OnKeyPressEvent(EventKey evnt) {
            return  ProcessKeyEvent (evnt);
        }

        protected override bool OnKeyReleaseEvent(EventKey evnt) { return ProcessKeyEvent(evnt); }

        private void ImContext_Commit(object o, Gtk.CommitArgs args)
        {
            Input(new RawTextInputEventArgs(GtkKeyboardDevice.Instance, _lastKeyEventTimestamp, args.Str));
        }

#if GTK2
        protected override bool OnExposeEvent(EventExpose evnt)
#else
        protected bool OnExposeEvent(EventExpose evnt)
#endif
        {
            Paint(evnt.Area.ToPerspex(), GetHandle(evnt.Window));
            return true;
        }

        protected override void OnFocusActivated()
        {
            Activated();
        }

        protected override bool OnMotionNotifyEvent(EventMotion evnt)
        {
            var position = new Point(evnt.X, evnt.Y);

            GtkMouseDevice.Instance.SetClientPosition(position);

            var e = new RawMouseEventArgs(
                GtkMouseDevice.Instance,
                evnt.Time,
                _owner,
                RawMouseEventType.Move,
                position, GetModifierKeys(evnt.State));
            Input(e);
            return true;
        }

        private IPlatformHandle GetHandle(Gdk.Window window)
        {
            return new PlatformHandle(window.Handle, "GdkWindow");
        }
    }
}