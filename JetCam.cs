using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Airstrike;
using GTA;

namespace PrecisionAirstrike
{
    internal class JetCam : Script
    {
        public JetCam()
        {
            KeyUp += onKeyUp;
        }
        void onKeyUp(object sender, KeyEventArgs e)
        {
            if (Main.cameraSet && e.KeyCode == Main.camKey && !Main.cameraSet1)
            {
                Main.cameraSet1 = true;
                World.RenderingCamera = Main.cam;
            }
            else if (Main.cameraSet && e.KeyCode == Main.camKey && Main.cameraSet1)
            {
                Main.cameraSet1 = false;
                World.RenderingCamera = null;
            }
        }
    }
}
