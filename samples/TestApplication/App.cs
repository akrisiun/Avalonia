// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex;
using Perspex.Themes.Default;
using Perspex.Controls;

namespace TestApplication
{
    public class App : Perspex.Application
    {
        public App()
        {
            RegisterServices();
#if REACTIVE
            InitializeSubsystems((int)Environment.OSVersion.Platform);
            Styles = new DefaultTheme().Add<Perspex.Window>();
#else

            // InitializeSubsystems((int)PlatformID.Unix);
            // BadImageFormatException: must be 32bit application
            Perspex.Gtk.GtkPlatform.Initialize();
            Styles = new DefaultTheme().Add<Window>();
#endif
        }
    }
}
