﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Perspex.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using Perspex.Controls;
using Perspex.Input.Raw;
using Perspex.Platform;
using Perspex.Win32.Input;
using Perspex.Win32.Interop;

namespace Perspex.Win32
{
    public class WindowImpl : IWindowImpl
    {
        private static readonly List<WindowImpl> s_instances = new List<WindowImpl>();

        private static readonly IntPtr DefaultCursor = UnmanagedMethods.LoadCursor(
            IntPtr.Zero, new IntPtr((int)UnmanagedMethods.Cursor.IDC_ARROW));

        private UnmanagedMethods.WndProc _wndProcDelegate;

        private string _className;

        private IntPtr _hwnd;

        private TopLevel _owner;

        private bool _trackingMouse;

        public WindowImpl()
        {
            CreateWindow();
            s_instances.Add(this);
        }

        public Action Activated { get; set; }

        public Action Closed { get; set; }

        public Action Deactivated { get; set; }

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect, IPlatformHandle> Paint { get; set; }

        public Action<Size> Resized { get; set; }

        public Thickness BorderThickness
        {
            get
            {
                var style = UnmanagedMethods.GetWindowLong(_hwnd, -16);
                var exStyle = UnmanagedMethods.GetWindowLong(_hwnd, -20);
                var padding = new UnmanagedMethods.RECT();

                if (UnmanagedMethods.AdjustWindowRectEx(ref padding, style, false, exStyle))
                {
                    return new Thickness(-padding.left, -padding.top, padding.right, padding.bottom);
                }
                else
                {
                    throw new Win32Exception();
                }
            }
        }

        public Size ClientSize
        {
            get
            {
                UnmanagedMethods.RECT rect;
                UnmanagedMethods.GetClientRect(_hwnd, out rect);
                return new Size(rect.right, rect.bottom);
            }

            set
            {
                if (value != ClientSize)
                {
                    value += BorderThickness;

                    UnmanagedMethods.SetWindowPos(
                        _hwnd,
                        IntPtr.Zero,
                        0,
                        0,
                        (int)value.Width,
                        (int)value.Height,
                        UnmanagedMethods.SetWindowPosFlags.SWP_RESIZE);
                }
            }
        }

        public IPlatformHandle Handle
        {
            get;
            private set;
        }

        public bool IsEnabled
        {
            get { return UnmanagedMethods.IsWindowEnabled(_hwnd); }
            set { UnmanagedMethods.EnableWindow(_hwnd, value); }
        }

        public Size MaxClientSize
        {
            get
            {
                return new Size(
                    UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXMAXTRACK),
                    UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CYMAXTRACK))
                    - BorderThickness;


            }
        }

        public void Activate()
        {
            UnmanagedMethods.SetActiveWindow(_hwnd);
        }

        public IPopupImpl CreatePopup()
        {
            return new PopupImpl();
        }

        public void Dispose()
        {
            s_instances.Remove(this);
            UnmanagedMethods.DestroyWindow(_hwnd);
        }

        public void Hide()
        {
            UnmanagedMethods.ShowWindow(_hwnd, UnmanagedMethods.ShowWindowCommand.Hide);
        }

        public void Invalidate(Rect rect)
        {
            var r = new UnmanagedMethods.RECT
            {
                left = (int)rect.X,
                top = (int)rect.Y,
                right = (int)rect.Right,
                bottom = (int)rect.Bottom,
            };

            UnmanagedMethods.InvalidateRect(_hwnd, ref r, false);
        }

        public Point PointToScreen(Point point)
        {
            var p = new UnmanagedMethods.POINT { X = (int)point.X, Y = (int)point.Y };
            UnmanagedMethods.ClientToScreen(_hwnd, ref p);
            return new Point(p.X, p.Y);
        }

        public void SetOwner(TopLevel owner)
        {
            _owner = owner;
        }

        public void SetTitle(string title)
        {
            UnmanagedMethods.SetWindowText(_hwnd, title);
        }

        public virtual void Show()
        {
            UnmanagedMethods.ShowWindow(_hwnd, UnmanagedMethods.ShowWindowCommand.Normal);
        }

        public virtual IDisposable ShowDialog()
        {
            var disabled = s_instances.Where(x => x != this && x.IsEnabled).ToList();
            TopLevel activated = null;

            foreach (var window in disabled)
            {
                if (window._owner.IsActive)
                {
                    activated = window._owner;
                }

                window.IsEnabled = false;
            }

            Show();

            return Disposable.Create(() =>
            {
                foreach (var window in disabled)
                {
                    window.IsEnabled = true;
                }

                if (activated != null)
                {
                    activated.Activate();
                }
            });
        }

        public void SetCursor(IPlatformHandle cursor)
        {
            UnmanagedMethods.SetClassLong(_hwnd, UnmanagedMethods.ClassLongIndex.GCL_HCURSOR,
                (cursor != null ? cursor.Handle : DefaultCursor));
        }

        protected virtual IntPtr CreateWindowOverride(ushort atom, IntPtr Handle) // = IntPtr.Zero)
        {
            return UnmanagedMethods.CreateWindowEx(
                0,
                atom,
                null,
                (int)UnmanagedMethods.WindowStyles.WS_OVERLAPPEDWINDOW,
                UnmanagedMethods.CW_USEDEFAULT,
                UnmanagedMethods.CW_USEDEFAULT,
                UnmanagedMethods.CW_USEDEFAULT,
                UnmanagedMethods.CW_USEDEFAULT,
                Handle, // IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        protected virtual IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            bool unicode = UnmanagedMethods.IsWindowUnicode(hWnd);

            const double wheelDelta = 120.0;
            uint timestamp = unchecked((uint)UnmanagedMethods.GetMessageTime());

            RawInputEventArgs e = null;

            WindowsMouseDevice.Instance.CurrentWindow = this;

            switch ((UnmanagedMethods.WindowsMessage)msg)
            {
                case UnmanagedMethods.WindowsMessage.WM_ACTIVATE:
                    var wa = (UnmanagedMethods.WindowActivate)((int)wParam & 0xffff);

                    switch (wa)
                    {
                        case UnmanagedMethods.WindowActivate.WA_ACTIVE:
                        case UnmanagedMethods.WindowActivate.WA_CLICKACTIVE:
                            if (Activated != null)
                            {
                                Activated();
                            }

                            break;

                        case UnmanagedMethods.WindowActivate.WA_INACTIVE:
                            if (Deactivated != null)
                            {
                                Deactivated();
                            }

                            break;
                    }

                    return IntPtr.Zero;

                case UnmanagedMethods.WindowsMessage.WM_DESTROY:
                    if (Closed != null)
                    {
                        UnmanagedMethods.UnregisterClass(_className, Marshal.GetHINSTANCE(GetType().Module));
                        Closed();
                    }

                    return IntPtr.Zero;

                case UnmanagedMethods.WindowsMessage.WM_KEYDOWN:
                case UnmanagedMethods.WindowsMessage.WM_SYSKEYDOWN:
                    e = new RawKeyEventArgs(
                            WindowsKeyboardDevice.Instance,
                            timestamp,
                            RawKeyEventType.KeyDown,
                            KeyInterop.KeyFromVirtualKey((int)wParam), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_KEYUP:
                case UnmanagedMethods.WindowsMessage.WM_SYSKEYUP:
                    e = new RawKeyEventArgs(
                            WindowsKeyboardDevice.Instance,
                            timestamp,
                            RawKeyEventType.KeyUp,
                            KeyInterop.KeyFromVirtualKey((int)wParam), WindowsKeyboardDevice.Instance.Modifiers);
                    break;
                case UnmanagedMethods.WindowsMessage.WM_CHAR:
                    // Ignore control chars
                    if (wParam.ToInt32() > 32)
                    {
                        e = new RawTextInputEventArgs(WindowsKeyboardDevice.Instance, timestamp,
                            new string((char)wParam.ToInt32(), 1));
                    }

                    break;
                case UnmanagedMethods.WindowsMessage.WM_LBUTTONDOWN:
                    e = new RawMouseEventArgs(
                        WindowsMouseDevice.Instance,
                        timestamp,
                        _owner,
                        RawMouseEventType.LeftButtonDown,
                        new Point((uint)lParam & 0xffff, (uint)lParam >> 16), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_LBUTTONUP:
                    e = new RawMouseEventArgs(
                        WindowsMouseDevice.Instance,
                        timestamp,
                        _owner,
                        RawMouseEventType.LeftButtonUp,
                        new Point((uint)lParam & 0xffff, (uint)lParam >> 16), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_MOUSEMOVE:
                    if (!_trackingMouse)
                    {
                        var tm = new UnmanagedMethods.TRACKMOUSEEVENT
                        {
                            cbSize = Marshal.SizeOf(typeof(UnmanagedMethods.TRACKMOUSEEVENT)),
                            dwFlags = 2,
                            hwndTrack = _hwnd,
                            dwHoverTime = 0,
                        };

                        UnmanagedMethods.TrackMouseEvent(ref tm);
                    }

                    e = new RawMouseEventArgs(
                        WindowsMouseDevice.Instance,
                        timestamp,
                        _owner,
                        RawMouseEventType.Move,
                        new Point((uint)lParam & 0xffff, (uint)lParam >> 16), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_MOUSEWHEEL:
                    e = new RawMouseWheelEventArgs(
                        WindowsMouseDevice.Instance,
                        timestamp,
                        _owner,
                        ScreenToClient((uint)lParam & 0xffff, (uint)lParam >> 16),
                        new Vector(0, ((int)wParam >> 16) / wheelDelta), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_MOUSELEAVE:
                    _trackingMouse = false;
                    e = new RawMouseEventArgs(
                        WindowsMouseDevice.Instance,
                        timestamp,
                        _owner,
                        RawMouseEventType.LeaveWindow,
                        new Point(), WindowsKeyboardDevice.Instance.Modifiers);
                    break;

                case UnmanagedMethods.WindowsMessage.WM_PAINT:
                    if (Paint != null)
                    {
                        UnmanagedMethods.PAINTSTRUCT ps;

                        if (UnmanagedMethods.BeginPaint(_hwnd, out ps) != IntPtr.Zero)
                        {
                            UnmanagedMethods.RECT r;
                            UnmanagedMethods.GetUpdateRect(_hwnd, out r, false);
                            Paint(new Rect(r.left, r.top, r.right - r.left, r.bottom - r.top), Handle);
                            UnmanagedMethods.EndPaint(_hwnd, ref ps);
                        }
                    }

                    return IntPtr.Zero;

                case UnmanagedMethods.WindowsMessage.WM_SIZE:
                    if (Resized != null)
                    {
                        var clientSize = new Size((int)lParam & 0xffff, (int)lParam >> 16);
                        Resized(clientSize);
                    }

                    return IntPtr.Zero;
            }

            if (e != null && Input != null)
            {
                Input(e);
                return IntPtr.Zero;
            }

            return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void CreateWindow()
        {
            // Ensure that the delegate doesn't get garbage collected by storing it as a field.
            _wndProcDelegate = new UnmanagedMethods.WndProc(WndProc);

            _className = Guid.NewGuid().ToString();

            UnmanagedMethods.WNDCLASSEX wndClassEx = new UnmanagedMethods.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(UnmanagedMethods.WNDCLASSEX)),
                style = 0,
                lpfnWndProc = _wndProcDelegate,
                hInstance = Marshal.GetHINSTANCE(GetType().Module),
                hCursor = DefaultCursor,
                hbrBackground = (IntPtr)5,
                lpszClassName = _className,
            };

            ushort atom = UnmanagedMethods.RegisterClassEx(ref wndClassEx);

            if (atom == 0)
            {
                throw new Win32Exception();
            }

            _hwnd = CreateWindowOverride(atom, IntPtr.Zero);

            if (_hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            Handle = new PlatformHandle(_hwnd, PlatformConstants.WindowHandleType);
        }

        private Point ScreenToClient(uint x, uint y)
        {
            var p = new UnmanagedMethods.POINT { X = (int)x, Y = (int)y };
            UnmanagedMethods.ScreenToClient(_hwnd, ref p);
            return new Point(p.X, p.Y);
        }
    }
}
