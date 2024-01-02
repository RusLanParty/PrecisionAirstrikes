using System;
using GTA;
using GTA.Math;
using GTA.Native;

namespace PrecisionAirstrikes
{
    internal class VariousTickEvents: Script
    {
        public VariousTickEvents()
        {
            Tick += onTick;
        }
        void onTick(object sender, EventArgs e)
        {
           
            if (Main.controller)
            {
                if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, Main.shw) && Menu.menu.Visible == false && Menu.subMenu.Visible == false)
                {
                    Menu.menu.Visible = true;
                }
                else if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, Main.shw) && Menu.menu.Visible == true)
                {
                    Menu.menu.Visible = false;
                }

                if (Main.cameraSet && Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, Main.conCam) && !Main.cameraSet1)
                {
                    Main.cameraSet1 = true;
                    World.RenderingCamera = Main.cam;
                    Main.hudDis = true;
                }
                else if (Main.cameraSet && Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, Main.conCam) && Main.cameraSet1)
                {
                    Main.cameraSet1 = false;
                    World.RenderingCamera = null;
                    Main.hudDis = false;
                }
            }
           

            if (Main.hideHud)
            {
                if (Main.hudDis)
                {
                    Function.Call(Hash.DISPLAY_RADAR, false);
                }
                else if (!Main.hudDis)
                {
                    Function.Call(Hash.DISPLAY_RADAR, true);
                }
            }                     
        }
    }
}
