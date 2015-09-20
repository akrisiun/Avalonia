﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Generic;
using Gdk;
using Perspex.Input;
using Perspex.Platform;

namespace Perspex.Gtk
{
    using Gtk = global::Gtk;

    internal class CursorFactory : IStandardCursorFactory
    {
        static CursorFactory ()
        {
            Instance = new CursorFactory();
        }

        public static CursorFactory Instance { get; private set; } // = new CursorFactory();

        private CursorFactory()
        {
        }

        private static readonly Dictionary<StandardCursorType, object> CursorTypeMapping = new Dictionary
            <StandardCursorType, object>
        {
            { StandardCursorType.AppStarting, CursorType.Watch },
            { StandardCursorType.Arrow, CursorType.LeftPtr },
            { StandardCursorType.Cross, CursorType.Cross },
            { StandardCursorType.Hand, CursorType.Hand1 },
            { StandardCursorType.Ibeam, CursorType.Xterm },
            { StandardCursorType.No, Gtk.Stock.Cancel},
            { StandardCursorType.SizeAll, CursorType.Sizing },
            //{ StandardCursorType.SizeNorthEastSouthWest, 32643 },
            { StandardCursorType.SizeNorthSouth, CursorType.SbVDoubleArrow},
            //{ StandardCursorType.SizeNorthWestSouthEast, 32642 },
            { StandardCursorType.SizeWestEast, CursorType.SbHDoubleArrow },
            { StandardCursorType.UpArrow, CursorType.BasedArrowUp },
            { StandardCursorType.Wait, CursorType.Watch },
            { StandardCursorType.Help, Gtk.Stock.Help }
        };

        private static readonly Dictionary<StandardCursorType, IPlatformHandle> Cache =
            new Dictionary<StandardCursorType, IPlatformHandle>();

        private Gdk.Cursor GetCursor(object desc)
        {
            Gdk.Cursor rv;
            var name = desc as string;
            if (name != null)
            {
                var theme = Gtk.IconTheme.Default;
                var icon = theme.LoadIcon(name, 32, default(Gtk.IconLookupFlags));
                rv = icon == null ? new Gdk.Cursor(CursorType.XCursor) : new Gdk.Cursor(Display.Default, icon, 0, 0);
            }
            else
            {
                rv = new Gdk.Cursor((CursorType)desc);
            }

#if GTK2
            rv.Owned = false;
#endif
            return rv;
        }

        public IPlatformHandle GetCursor(StandardCursorType cursorType)
        {
            IPlatformHandle rv;
            if (!Cache.TryGetValue(cursorType, out rv))
            {
                Cache[cursorType] =
                    rv =
                        new PlatformHandle(
                            GetCursor(CursorTypeMapping[cursorType]).Handle,
                            "GTKCURSOR");
            }

            return rv;
        }
    }
}
