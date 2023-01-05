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
using LemonUI.Elements;
using LemonUI.Menus;

namespace PrecisionAirstrike
{
    internal class TriggerEvents : Script
    {
        public TriggerEvents()
        {
            KeyUp += onKeyUp;
            Main.strikeMode.ItemChanged += strikeChange;
            Main.trgtmode.Activated += trgtChange2;
            Main.trgtmode.ItemChanged += trgtChange;
            Main.jetAud.CheckboxChanged += jetAudChange;
            Main.respons.CheckboxChanged += respnChange;
            Main.radAud.CheckboxChanged += radAudChange;
            Main.heightSlide.ValueChanged += heightChange;
            Main.radSlide.ValueChanged += radChange;
            Main.rad2Slide.ValueChanged += rad2Change;
            Main.blipsCheck.CheckboxChanged += pBlipsChange;
            Main.hudCheck.CheckboxChanged += hudChange;
            Main.empList.ItemChanged += empChange;
        }
        void empChange(object sender, ItemChangedEventArgs <String> e)
        {
            if (Main.empList.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 0);
                Settings.Save();
                Main.empMode = 0;
            }
            else if (Main.empList.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 1);
                Settings.Save();
                Main.empMode = 1;
            }
            else if (Main.empList.SelectedIndex == 2)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 2);
                Settings.Save();
                Main.empMode = 2;
            }
        }

        void hudChange(object sender, EventArgs e)
        {
            if (Main.hudCheck.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "hideHud", true);
                Settings.Save();
                Main.hideHud = true;
            }
            else if (!Main.hudCheck.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "hideHud", false);
                Settings.Save();
                Main.hideHud = false;
            }
        }
        void pBlipsChange(object sender, EventArgs e)
        {
            if (Main.blipsCheck.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "planeBlips", true);
                Settings.Save();
                Main.blipEnabled = true;
            }
            else if (!Main.blipsCheck.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "planeBlips", false);
                Settings.Save();
                Main.blipEnabled = false;
            }
        }
        void rad2Change(object sender, EventArgs e)
        {
            Settings.SetValue("FORSCRIPTTOREAD", "radius2", Main.rad2Slide.Value);
            Settings.Save();
            Main.radius2 = Main.rad2Slide.Value;
            Main.rad2Slide.Description = "Radius of the missile barrage:  ~y~" + (Main.rad2Slide.Value).ToString();
        }
        void radChange(object sender, EventArgs e)
        {
            Settings.SetValue("FORSCRIPTTOREAD", "radius", Main.radSlide.Value);
            Settings.Save();
            Main.radius = Main.radSlide.Value;
            Main.radSlide.Description = "Radius of the gattling gun: ~y~" + (Main.radSlide.Value).ToString();
        }
        void heightChange(object sender, EventArgs e)
        {
           
            Settings.SetValue("FORSCRIPTTOREAD", "height", Main.heightSlide.Value + 150);
            Settings.Save();
            Main.height = Main.heightSlide.Value + 150;
            Main.heightSlide.Description = "The height at which planes will spawn: ~y~" +(Main.heightSlide.Value + 150).ToString();
        }
        void radAudChange(object sender, EventArgs e)
        {
            if (Main.radAud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "radioAudio", true);
                Settings.Save();
                Main.radioAudio = true;
            }
            else if (!Main.radAud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "radioAudio", false);
                Settings.Save();
                Main.radioAudio = false;
            }
        }
        void respnChange(object sender, EventArgs e)
        {
            if (Main.respons.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "responsibility", true);
                Settings.Save();
                Main.resp = true;
            }
            else if (!Main.respons.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "responsibility", false);
                Settings.Save();
                Main.resp = false;
            }
        }
        void jetAudChange(object sender, EventArgs e)
        {
            if (Main.jetAud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "jetAudio", true);
                Settings.Save();
                Main.jetAudio = true;
            }
            else if (!Main.jetAud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "jetAudio", false);
                Settings.Save();
                Main.jetAudio = false;
            }
        }
        void trgtChange(object sender, ItemChangedEventArgs<string> e)
        {
            if (Main.trgtmode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
                Main.aimMode = 0;
                Game.Player.Character.Weapons.Give(WeaponHash.Flare, 0, true, true);
            }
            else if (Main.trgtmode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 1);
                Settings.Save();
                Main.aimMode = 1;
                Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 0, true, true);

            }
        }
        void trgtChange2(object sender, EventArgs e)
        {
            if (Main.trgtmode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
                Main.aimMode = 0;
                Game.Player.Character.Weapons.Give(WeaponHash.Flare, 0, true, true);
            }
            else if (Main.trgtmode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 1);
                Settings.Save();
                Main.aimMode = 1;
                Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 0, true, true);

            }
        }
        void strikeChange(object sender, ItemChangedEventArgs<string> e)
        {
            if (Main.strikeMode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 1);
                Settings.Save();
                Main.mode = 1;
                Main.strikeMode.Description = "2 jets with 30mm gatling cannons, delivering a burst of ~r~280~s~ rounds each. Precise and deadly.";
            }
            else if (Main.strikeMode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 2);
                Settings.Save();
                Main.mode = 2;
                Main.strikeMode.Description = "A stealth bomber that releases a line of explosives along its path. Good for cleaning whole streets.";
            }
            else if (Main.strikeMode.SelectedIndex == 2)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 3);
                Settings.Save();
                Main.mode = 3;
                Main.strikeMode.Description = "20 drones, each carrying ~r~15~s~ small missiles and an EMP generator. When the swarm gets close to the target it generates an electro magnetic pulse, disabling all vehicles & aircraft in area, and then launches a barrage of missiles at the target. Good against helis.";
            }
        }
       
        void onKeyUp(object sender, KeyEventArgs e)
        {
            if (!Main.controller)
            {
                if (e.KeyCode == Main.showMenuKey && Main.menu.Visible == false && Main.subMenu.Visible == false)
                {
                    Main.menu.Visible = true;
                }
                else if (e.KeyCode == Main.showMenuKey && Main.menu.Visible == true)
                {
                    Main.menu.Visible = false;
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
}
