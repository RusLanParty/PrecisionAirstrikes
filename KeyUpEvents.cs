using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Airstrike;
using GTA;
using GTA.Native;
using GTA.UI;

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
                if(Main.menu.Visible==true && Main.menu.SelectedIndex == 0)
                {
                    if (Main.strikeMode.SelectedIndex == 0)
                    {
                        Settings.SetValue("SETTINGS", "defaultMode", 1);
                        Settings.Save();
                        Main.mode = 1;
                    }
                    else if (Main.strikeMode.SelectedIndex == 1)
                    {
                        Settings.SetValue("SETTINGS", "defaultMode", 2);
                        Settings.Save();
                        Main.mode = 2;
                    }
                    else if (Main.strikeMode.SelectedIndex == 2)
                    {
                        Settings.SetValue("SETTINGS", "defaultMode", 3);
                        Settings.Save();
                        Main.mode = 3;
                    }
                }
                else if (Main.menu.Visible && Main.menu.SelectedIndex == 1)
                {
                    if (Main.respons.Checked)
                    {
                        Settings.SetValue("SETTINGS", "responsibility", true);
                        Settings.Save();
                        Main.resp = true;
                    }
                    else if (!Main.respons.Checked)
                    {
                        Settings.SetValue("SETTINGS", "responsibility", false);
                        Settings.Save();
                        Main.resp = false;
                    }
                }
                else if (Main.menu.Visible && Main.menu.SelectedIndex == 2)
                {
                    if (Main.jetAud.Checked)
                    {
                        Settings.SetValue("SETTINGS", "jetAudio", true);
                        Settings.Save();
                        Main.jetAudio = true;
                    }
                    else if (!Main.jetAud.Checked)
                    {
                        Settings.SetValue("SETTINGS", "jetAudio", false);
                        Settings.Save();
                        Main.jetAudio = false;
                    }
                }
                else if (Main.menu.Visible && Main.menu.SelectedIndex == 3)
                {
                    if (Main.radAud.Checked)
                    {
                        Settings.SetValue("SETTINGS", "radioAudio", true);
                        Settings.Save();
                        Main.radioAudio = true;
                    }
                    else if (!Main.radAud.Checked)
                    {
                        Settings.SetValue("SETTINGS", "radioAudio", false);
                        Settings.Save();
                        Main.radioAudio = false;
                    }
                }
                if (Main.menu.Visible == true && Main.menu.SelectedIndex == 4)
                {
                    if (Main.trgtmode.SelectedIndex == 0)
                    {
                        Settings.SetValue("SETTINGS", "targetingMode", 0);
                        Settings.Save();
                        Main.aimMode = 0;
                        Game.Player.Character.Weapons.Give(WeaponHash.Flare, 0, true, true);
                    }
                    else if (Main.trgtmode.SelectedIndex == 1)
                    {
                        Settings.SetValue("SETTINGS", "targetingMode", 1);
                        Settings.Save();
                        Main.aimMode = 1;
                        Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 0, true, true);
                    }
                }

            }
           
            if (e.KeyCode == Keys.Delete && Main.menu.Visible == false)
            {
                Main.menu.Visible = true;
            }
            else if(e.KeyCode == Keys.Delete && Main.menu.Visible == true)
            {
                Main.menu.Visible= false;
            }
           
            if (Main.cameraSet && e.KeyCode == Main.camKey && !Main.cameraSet1)
            {
                Main.cameraSet1 = true;
                World.RenderingCamera = Main.cam;
                Main.hudDis = true;
            }
            else if (Main.cameraSet && e.KeyCode == Main.camKey && Main.cameraSet1)
            {
                Main.cameraSet1 = false;
                World.RenderingCamera = null;
                Main.hudDis = false;
            }
        }
    }
}
