using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using SharpDX;
using SharpDX.Windows;

//using SharpDX.DXGI;
//using SharpDX.Direct3D;
//using SharpDX.Direct3D11;
//using SharpDX.Direct2D1;
//using Device = SharpDX.Direct3D11.Device;
//using FactoryD2D = SharpDX.Direct2D1.Factory;
//using FactoryDXGI = SharpDX.DXGI.Factory1;

namespace TestDX
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Tutorial6.Program.MainDX();
            //Application.Run(new Form1());
        }
    }
}
