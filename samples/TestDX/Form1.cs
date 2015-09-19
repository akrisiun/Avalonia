using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestDX
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var ctrl = new UserControl1();
            ctrl.Visible = true;
            ctrl.BackColor = System.Drawing.Color.Red;

            this.Controls.Add(ctrl);

            RenderTarget renderTarget;
        }
    }

    /*
        RenderTarget renderTarget;

    // Create Direct2D factory
    using (var factory = new FactoryD2D())
    {
        // Get desktop DPI
        var dpi = factory.DesktopDpi;

        // Create bitmap render target from DXGI surface
        renderTarget = new RenderTarget(factory, backBuffer, new RenderTargetProperties()
        {
            DpiX = dpi.Width,
            DpiY = dpi.Height,
            MinLevel = SharpDX.Direct2D1.FeatureLevel.Level_DEFAULT,
            PixelFormat = new PixelFormat(Format.Unknown, AlphaMode.Ignore),
            Type = RenderTargetType.Default,
            Usage = RenderTargetUsage.None
        });
    }

    // Rendering function
    RenderLoop.Run(form, () =>
    {
        renderTarget.BeginDraw();
        renderTarget.Transform = Matrix3x2.Identity;
        renderTarget.Clear(Color.White);

        // put drawing code here (see below)

        renderTarget.EndDraw();

        swapChain.Present(0, PresentFlags.None);

    });

    */

}
