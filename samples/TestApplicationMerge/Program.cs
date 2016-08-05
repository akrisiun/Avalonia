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
            var reactive = Assembly.LoadFile(baseDir + "Sharp.Reactive.dll");
            var core = Assembly.LoadFile(baseDir + "Avalonia.Core.dll");

            // The version of ReactiveUI currently included is for WPF and so expects a WPF
            // dispatcher. This makes sure it's initialized.
            System.Windows.Threading.Dispatcher foo = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            app = new App();

            builder = AppBuilder.Configure(app);

            builder
                .UsePlatformDetect()
                .SetupWithoutStarting();

            app.Run();
        }
    }
}
