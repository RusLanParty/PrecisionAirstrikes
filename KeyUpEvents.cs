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
    internal class KeyUpEvents : Script
    {
        public KeyUpEvents()
        {
            KeyUp += onKeyUp;
        }
        void onKeyUp(object sender, KeyEventArgs e)
        {
            if (!Main.modeLock)
            {
                switch (e.KeyCode)
                {
                    case Keys.NumPad1:
                        Main.mode = 1;
                        GTA.UI.Screen.ShowHelpText("~g~Gatling gun~s~ airstrike selected", 1500, true, false);
                        break;
                    case Keys.NumPad2:
                        Main.mode = 2;
                        GTA.UI.Screen.ShowHelpText("~g~Stealth missile barrage~s~ airstrike selected", 1500, true, false);
                        break;
                }
            }
           
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
