// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex;
using Perspex.Themes.Default;

namespace TestApplication
{
    public class App : Application
    {
        public App()
        {
            RegisterServices();

#if GTK
            // Should be 32bit
            Gtk.Application.Init();
            Perspex.Cairo.CairoPlatform.Initialize();
            Perspex.Gtk.GtkPlatform.Initialize();
#else
            InitializeSubsystems((int)Environment.OSVersion.Platform);
#endif

            Styles = new DefaultTheme();
            Styles.Add(new SampleTabStyle());
        }
    }
}
