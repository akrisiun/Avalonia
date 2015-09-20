﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using Perspex.Platform;

namespace Perspex.Gtk
{
    /// <summary>
    /// Loads assets compiled into the application binary.
    /// </summary>
    public class AssetLoader : IAssetLoader
    {
        /// <summary>
        /// Opens the resource with the requested URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>A stream containing the resource contents.</returns>
        /// <exception cref="FileNotFoundException">
        /// The resource was not found.
        /// </exception>
        public Stream Open(Uri uri)
        {
            var assembly = Assembly.GetEntryAssembly();
            var rv = assembly.GetManifestResourceStream(uri.ToString());
            if (rv == null)
                throw new FileNotFoundException(uri.ToString());
            return rv;
        }
    }
}
