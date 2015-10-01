using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gtk
{
    public class CoreGtk3
    {
        static CoreGtk3()
        {
            Instance = new CoreGtk3();
            LoadDlls();
        }

        public static CoreGtk3 Instance { get; set; }

        public override string ToString()
        {
            return "CoreGtk3, " + typeof(CoreGtk3).Assembly.FullName;
        }

        // make sure Base ddl loaded
        public static void LoadDlls()
        {
            try {
                // Exception thrown: 'System.BadImageFormatException' in cairo3 - sharp.dll
                var cairo = Cairo.CairoAPI.VersionString;
                var atk = Atk.Global.AtkVersion();
                var glib = GLibSharp.LoadGlib.Version();
                var gio = GLib.Gio.Version();
            }
            catch (Exception ex) { LastError = ex; }
        }

        public static Exception LastError { get; set; }

    }
}
