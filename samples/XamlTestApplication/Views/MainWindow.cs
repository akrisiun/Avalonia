// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Perspex;
using Perspex.Controls;
using Perspex.Diagnostics;

//using OmniXaml;
//using Perspex.Markup.Xaml;

namespace XamlTestApplication.Views
{
    public class MainWindow : Window
    {
        static MainWindow() { }
        public static void Load() { }

        public MainWindow()
        {
            InitializeComponent();

            DevTools.Attach(this, () => new Window(), (w) => (w as Window).Show());
        }

        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}