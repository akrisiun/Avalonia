﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using Perspex.Cairo.Media;
using Perspex.Cairo.Media.Imaging;
using Perspex.Media;
using Perspex.Platform;

namespace Perspex.Cairo
{
    using System.IO;
    using global::Cairo;

    public class CairoPlatform : IPlatformRenderInterface
    {
        static CairoPlatform()
        {
            // Debug entry
            s_instance = new CairoPlatform();
        }

        private static readonly CairoPlatform s_instance;

        private static Pango.Context s_pangoContext;

        public static void Initialize()
        {
            s_pangoContext = CreatePangoContext();
            PerspexLocator.CurrentMutable.Bind<IPlatformRenderInterface>().ToConstant(s_instance);
        }

        public IBitmapImpl CreateBitmap(int width, int height)
        {
            return new BitmapImpl(new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 32, width, height));
        }

        public IFormattedTextImpl CreateFormattedText(
            string text,
            string fontFamily,
            double fontSize,
            FontStyle fontStyle,
            TextAlignment textAlignment,
            Perspex.Media.FontWeight fontWeight)
        {
            return new FormattedTextImpl(s_pangoContext, text, fontFamily, fontSize, fontStyle, textAlignment, fontWeight);
        }

        public IRenderTarget CreateRenderer(IPlatformHandle handle, double width, double height)
        {
            if (handle.HandleDescriptor != "GtkWindow")
                throw new NotSupportedException(string.Format(
                    "Don't know how to create a Cairo renderer from a '{0}' handle",
                    handle.HandleDescriptor));
            return new RenderTarget((Gtk.Window)handle, width, height);
        }

        public IRenderTargetBitmapImpl CreateRenderTargetBitmap(int width, int height)
        {
            return new RenderTargetBitmapImpl(new ImageSurface(Format.Argb32, width, height));
        }

        public IStreamGeometryImpl CreateStreamGeometry()
        {
            return new StreamGeometryImpl();
        }

        public IBitmapImpl LoadBitmap(string fileName)
        {
            Gdk.Pixbuf pixbuf = null;
            try
            {
                pixbuf = new Gdk.Pixbuf(fileName);
            }
            catch (Exception ex) { Console.WriteLine(String.Format("Failed {0} : {1}", fileName, ex.Message)); }   // not critical error

            return new BitmapImpl(pixbuf);
        }

        public IBitmapImpl LoadBitmap(Stream stream)
        {
            var pixbuf = new Gdk.Pixbuf(stream);

            return new BitmapImpl(pixbuf);
        }

        private static Pango.Context CreatePangoContext()
        {
            Gtk.Application.Init();
            return new Gtk.Invisible().CreatePangoContext();
        }
    }
}
