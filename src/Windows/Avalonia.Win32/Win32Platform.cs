// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using Avalonia.Input.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.Platform;
using Avalonia.Shared.PlatformSupport;
using Avalonia.Win32.Input;
using Avalonia.Win32.Interop;
using Avalonia.Controls;

using System.IO;
using System.Diagnostics;

namespace Avalonia
{
    public static class Win32ApplicationExtensions
    {
        public static AppBuilder UseWin32(this AppBuilder builder)
        {
            // var init = Avalonia.Win32.Win32Platform.Initialize as Action; // <string>;
            Avalonia.Win32.Win32Platform.Initialize();
            //builder.WindowingSubsystem = init;
            return builder;
        }
    }
}

namespace Avalonia.Win32
{
    public class Win32Platform : IPlatformThreadingInterface, IPlatformSettings, IWindowingPlatform, IPlatformIconLoader
    {
        private static readonly Win32Platform s_instance = new Win32Platform();
        private static Thread _uiThread;
        private UnmanagedMethods.WndProc _wndProcDelegate;
        private IntPtr _hwnd;
        private readonly List<Delegate> _delegates = new List<Delegate>();

        public Win32Platform()
        {
            // Declare that this process is aware of per monitor DPI
            if (UnmanagedMethods.ShCoreAvailable)
            {
                UnmanagedMethods.SetProcessDpiAwareness(UnmanagedMethods.PROCESS_DPI_AWARENESS.PROCESS_PER_MONITOR_DPI_AWARE);
            }

        }

        public static object lockMessages = new object();
        public static bool hasMessages = false;

        public Size DoubleClickSize => new Size(
            UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CXDOUBLECLK),
            UnmanagedMethods.GetSystemMetrics(UnmanagedMethods.SystemMetric.SM_CYDOUBLECLK));

        public TimeSpan DoubleClickTime => TimeSpan.FromMilliseconds(UnmanagedMethods.GetDoubleClickTime());

        public void InitializeLocator()
        {
            lock (lockMessages)
            {
                if (!hasMessages)
                {
                    hasMessages = true;
                    CreateMessageWindow();
                }
            }

            Initialize();
        }

        public static void Initialize()
        {
            AvaloniaLocator.CurrentMutable
                .Bind<IClipboard>().ToSingleton<ClipboardImpl>()
                .Bind<IStandardCursorFactory>().ToConstant(CursorFactory.Instance)
                .Bind<IKeyboardDevice>().ToConstant(WindowsKeyboardDevice.Instance)
                .Bind<IMouseDevice>().ToConstant(WindowsMouseDevice.Instance)
                .Bind<IPlatformSettings>().ToConstant(s_instance)
                .Bind<IPlatformThreadingInterface>().ToConstant(s_instance)
                .Bind<ISystemDialogImpl>().ToSingleton<SystemDialogImpl>()
                .Bind<IWindowingPlatform>().ToConstant(s_instance)
                .Bind<IPlatformIconLoader>().ToConstant(s_instance);

            SharedPlatform.Register();
            _uiThread = Thread.CurrentThread;
        }

        public bool HasMessages()
        {
            UnmanagedMethods.MSG msg;
            return UnmanagedMethods.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
        }

        public void ProcessMessage()
        {
            UnmanagedMethods.MSG msg;
            UnmanagedMethods.GetMessage(out msg, IntPtr.Zero, 0, 0);
            UnmanagedMethods.TranslateMessage(ref msg);
            UnmanagedMethods.DispatchMessage(ref msg);
        }

        public void RunLoop(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                UnmanagedMethods.MSG msg;
                try
                {
                    UnmanagedMethods.GetMessage(out msg, IntPtr.Zero, 0, 0);
                    UnmanagedMethods.TranslateMessage(ref msg);

                    UnmanagedMethods.DispatchMessage(ref msg);
                }
                catch (Exception ex)
                {
                    //                    Managed Debugging Assistant 'CallbackOnCollectedDelegate' has detected a problem in TestApplicationMerge.vshost.exe'.

                    //Additional information: A callback was made on a garbage collected delegate of type 'Avalonia.Win32M!Avalonia.Win32.Interop.UnmanagedMethods+WndProc::Invoke'.
                    // This may cause application crashes, corruption and data loss.
                    //When passing delegates to unmanaged code, they must be kept alive by the managed application until it is guaranteed that they will never be called.occurred

                    Debugger.Log(0, "UnmanagedMethods.DispatchMessage", ex.Message);
                    Console.WriteLine("UnmanagedMethods.DispatchMessage", ex.Message);
                }
            }
        }

        public IDisposable StartTimer(TimeSpan interval, Action callback)
        {
            UnmanagedMethods.TimerProc timerDelegate =
                (hWnd, uMsg, nIDEvent, dwTime) => callback();

            IntPtr handle = UnmanagedMethods.SetTimer(
                IntPtr.Zero,
                IntPtr.Zero,
                (uint)interval.TotalMilliseconds,
                timerDelegate);

            // Prevent timerDelegate being garbage collected.
            _delegates.Add(timerDelegate);

            return Disposable.Create(() =>
            {
                _delegates.Remove(timerDelegate);
                UnmanagedMethods.KillTimer(IntPtr.Zero, handle);
            });
        }

        private static readonly int SignalW = unchecked((int)0xdeadbeaf);
        private static readonly int SignalL = unchecked((int)0x12345678);

        public void Signal()
        {
            UnmanagedMethods.PostMessage(
                _hwnd,
                (int)UnmanagedMethods.WindowsMessage.WM_DISPATCH_WORK_ITEM,
                new IntPtr(SignalW),
                new IntPtr(SignalL));
        }

        public bool CurrentThreadIsLoopThread => _uiThread == Thread.CurrentThread;

        public event Action Signaled;

        [SuppressMessage("Microsoft.StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "Using Win32 naming for consistency.")]
        private IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            if (msg == (int)UnmanagedMethods.WindowsMessage.WM_DISPATCH_WORK_ITEM && wParam.ToInt64() == SignalW && lParam.ToInt64() == SignalL)
            {
                Signaled?.Invoke();
            }
            return UnmanagedMethods.DefWindowProc(hWnd, msg, wParam, lParam);
        }

        private void CreateMessageWindow()
        {
            // Ensure that the delegate doesn't get garbage collected by storing it as a field.
            _wndProcDelegate = new UnmanagedMethods.WndProc(WndProc);

            UnmanagedMethods.WNDCLASSEX wndClassEx = new UnmanagedMethods.WNDCLASSEX
            {
                cbSize = Marshal.SizeOf(typeof(UnmanagedMethods.WNDCLASSEX)),
                lpfnWndProc = _wndProcDelegate,
                hInstance = Marshal.GetHINSTANCE(GetType().Module),
                lpszClassName = "AvaloniaMessageWindow",
            };

            ushort atom = UnmanagedMethods.RegisterClassEx(ref wndClassEx);

            if (atom == 0)
            {
                throw new Win32Exception();
            }

            _hwnd = UnmanagedMethods.CreateWindowEx(0, atom, null, 0, 0, 0, 0, 0, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);

            if (_hwnd == IntPtr.Zero)
            {
                throw new Win32Exception();
            }
        }

        public IWindowImpl CreateWindow()
        {
            return new WindowImpl();
        }

        public IWindowImpl CreateEmbeddableWindow()
        {
            return new EmbeddedWindowImpl();
        }

        public IPopupImpl CreatePopup()
        {
            return new PopupImpl();
        }

        public IWindowIconImpl LoadIcon(string fileName)
        {
            var icon = new System.Drawing.Bitmap(fileName);
            return new IconImpl(icon);
        }

        public IWindowIconImpl LoadIcon(Stream stream)
        {
            var icon = new System.Drawing.Bitmap(stream);
            return new IconImpl(icon);
        }

        public IWindowIconImpl LoadIcon(IBitmapImpl bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream);
                return new IconImpl(new System.Drawing.Bitmap(memoryStream));
            }
        }
    }
}
