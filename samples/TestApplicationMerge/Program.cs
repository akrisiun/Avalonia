// Copyright (c) The Avalonia Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Linq;
using System.IO;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Controls;
//using Avalonia.Controls.Html;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Controls.Templates;
using Avalonia.Diagnostics;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System.Reflection;
#if AVALONIA_GTK
using Avalonia.Gtk;
#endif
using ReactiveUI;

namespace TestApplication
{
    public class Program
    {
        public static App app { get; private set; }
        public static AppBuilder builder { get; private set; }

        private static void Main(string[] args)
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var reactive = Assembly.LoadFile(baseDir + "SharpDX.Reactive.dll");
            var core = Assembly.LoadFile(baseDir + "Avalonia.Core.dll");

            var win32 = Assembly.LoadFile(baseDir + "Avalonia.Win32M.dll"); // merge version 

            // The version of ReactiveUI currently included is for WPF and so expects a WPF
            // dispatcher. This makes sure it's initialized.
            System.Windows.Threading.Dispatcher foo = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            app = new App();

            //Avalonia.Platform.Wi
            // Avalonia.Win32.Win32Platform, Avalonia.Win32, Version = 0.0.0.1, Culture = neutral, PublicKeyToken = null
            Type platformClass = win32.GetType("Avalonia.Win32.Win32Platform");
            // object initWin32 = Activator.CreateInstance(platformClass);
            bool hasLocator = false;

            AppBuilder.GetInitializer = (dll) =>
            {
                //new Action<string>(
                //return (dllName) =>
                var dllName = dll as string;

                if (!hasLocator)
                {
                    hasLocator = true;
                    var init = new global::Avalonia.Win32.Win32Platform();
                    init.InitializeLocator();
                };
            };

            //var initObj = initWin32;
            //var init = platformClass.GetRuntimeMethod("Initialize", new Type[0]);
            //init.Invoke(null, null);

            builder = AppBuilder.Configure(app);

            builder
                .UsePlatformDetect()
                .SetupWithoutStarting();

            app.Run();
        }
    }
}
