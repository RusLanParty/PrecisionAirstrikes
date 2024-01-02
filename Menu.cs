using GTA;
using LemonUI;
using LemonUI.Menus;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace PrecisionAirstrikes
{
    internal class Menu : Script
    {
        public static ObjectPool pool = new ObjectPool();
        public static NativeMenu menu = new NativeMenu("Precision Airstrikes");
        public static NativeItem credits = new NativeItem("Made by ~t~RusLanParty~s~ (Ruslan Libin)", "Hey, hope you're having a good time with my mod. I spent hours perfecting it, not without the help of random strangers like you who helped me trace issues and come up with creative ideas, so if you have any suggestions or bug reports feel free to contact me on ~g~GTA5MODS~s~. Say \"thank you\": ~p~patreon.com/RusLanParty");
        public static NativeListItem<String> strikeMode = new NativeListItem<String>("Mode", "", "GAU-8 30mm", "Stealth Bomber", "EMP Drone Swarm");
        public static NativeListItem<String> targetingMode = new NativeListItem<String>("Targeting mode", "Choose whether to use a ~y~flare~s~, or an ~y~IR spotlight~s~ to mark your target. \nYou can press Enter or scroll through the options to equip chosen marker.", "Flare", "IR Spotlight");
        public static NativeMenu subMenu = new NativeMenu("Options", "Options");
        public static NativeCheckboxItem toggleJetAudio = new NativeCheckboxItem("GAU-8 sound");
        public static NativeCheckboxItem toggleRadioAudio = new NativeCheckboxItem("Radio chatter");
        public static NativeCheckboxItem responsibility = new NativeCheckboxItem("Responsibility", "If set to true, peds will be aware you are the one responsible for the airstrike (~b~including ~r~cops~s~). If set to false, pilots take the blame.");
        public static NativeSliderItem heightSlider = new NativeSliderItem("Height");
        public static NativeSliderItem radiusSlider = new NativeSliderItem("GAU-8 radius");
        public static NativeSliderItem missileRadius = new NativeSliderItem("Drone missiles radius");
        public static NativeCheckboxItem toggleBlips = new NativeCheckboxItem("Plane blips");
        public static NativeCheckboxItem toggleHud = new NativeCheckboxItem("Hide HUD", "Hides hud while in jet camera");
        public static NativeListItem<String> empModes = new NativeListItem<String>("EMP mode", "Normal - all vehicles in range disabled.", "Normal", "Aircraft only", "Disabled");
        public Menu()
        {
            Load();
            KeyUp += onKeyUp;
            Tick += onTick;
            strikeMode.ItemChanged += strikeChange;
            targetingMode.Activated += trgtChange2;
            targetingMode.ItemChanged += trgtChange;
            toggleJetAudio.CheckboxChanged += jetAudChange;
            responsibility.CheckboxChanged += respnChange;
            toggleRadioAudio.CheckboxChanged += radAudChange;
            heightSlider.ValueChanged += heightChange;
            radiusSlider.ValueChanged += radChange;
            missileRadius.ValueChanged += rad2Change;
            toggleBlips.CheckboxChanged += pBlipsChange;
            toggleHud.CheckboxChanged += hudChange;
            empModes.ItemChanged += empChange;
        }
        void onTick(object sender, EventArgs e)
        {
            pool.Process();

            if (Main.timeOut - Main.timePass > 0 && Main.newTime >= Main.timeOut)
            {
                menu.Name = "Timeout: " + (Main.timeOut - Main.timePass);
            }
            else
            {
                menu.Name = "Ready. Awaiting target input.";
            }

            if (Main.modeLock)
            {
                strikeMode.Enabled = false;
                responsibility.Enabled = false;
                toggleJetAudio.Enabled = false;
                toggleRadioAudio.Enabled = false;
                targetingMode.Enabled = false;
                missileRadius.Enabled = false;
                radiusSlider.Enabled = false;
                toggleBlips.Enabled = false;
                heightSlider.Enabled = false;
                toggleHud.Enabled = false;
                empModes.Enabled = false;
            }
            if (!Main.modeLock)
            {
                strikeMode.Enabled = true;
                responsibility.Enabled = true;
                toggleJetAudio.Enabled = true;
                toggleRadioAudio.Enabled = true;
                targetingMode.Enabled = true;
                missileRadius.Enabled = true;
                radiusSlider.Enabled = true;
                toggleBlips.Enabled = true;
                heightSlider.Enabled = true;
                toggleHud.Enabled = true;
                empModes.Enabled = true;
            }
        }
        void Load()
        {
            menu.UseMouse = false;
            subMenu.UseMouse = false;
            pool.Add(menu);
            pool.Add(subMenu);
            menu.Width = 450f;
            subMenu.Width = 450f;
            menu.Add(strikeMode);

            switch (Main.mode)
            {
                case 1:
                    strikeMode.Description = "2 jets with 30mm gatling cannons, delivering a burst of ~r~280~s~ rounds each. Precise and deadly.";
                    break;
                case 2:
                    strikeMode.Description = "A stealth bomber that releases a line of explosives along its path. Good for cleaning whole streets.";
                    break;
                case 3:
                    strikeMode.Description = "20 drones, each carrying ~r~15~s~ small missiles and an EMP generator. When the swarm gets close to the target it generates an electro magnetic pulse, disabling all vehicles & aircraft in area, and then launches a barrage of missiles at the target. Good against helis.";
                    break;
            }
            menu.Add(targetingMode);
            menu.AddSubMenu(subMenu);
            menu.BannerText.Shadow = true;
            menu.BannerText.Outline = true;
            subMenu.BannerText.Shadow = true;
            subMenu.BannerText.Outline = true;
            heightSlider.Multiplier = 50;
            heightSlider.Maximum = 350;
            heightSlider.Value = (int)Main.height - 150;
            heightSlider.Description = "The height at which planes will spawn: ~y~" + Main.height;
            radiusSlider.Maximum = 20;
            missileRadius.Maximum = 20;
            radiusSlider.Value = (int)Main.radius;
            radiusSlider.Description = "Radius of the gattling gun: ~y~" + Main.radius;
            missileRadius.Value = (int)Main.radius2;
            missileRadius.Description = "Radius of the missile barrage: ~y~" + Main.radius2;
            subMenu.Add(heightSlider);
            subMenu.Add(radiusSlider);
            subMenu.Add(missileRadius);
            subMenu.Add(empModes);
            switch (Main.empMode)
            {
                case 0:
                    empModes.SelectedIndex = 0;
                    break;
                case 1:
                    empModes.SelectedIndex = 1;
                    break;
                case 2:
                    empModes.SelectedIndex = 2;
                    break;
            }
            subMenu.Add(toggleJetAudio);
            subMenu.Add(toggleRadioAudio);
            subMenu.Add(responsibility);
            subMenu.Add(toggleBlips);
            subMenu.Add(toggleHud);
            subMenu.Add(credits);
            subMenu.Description = "~y~Custom jet models~s~, ~y~keybinds~s~, and ~y~timeout~s~ are only configurable via .ini";
            menu.Banner.Color = Color.Transparent;
            menu.BannerText.Font = GTA.UI.Font.Monospace;
            subMenu.Banner.Color = Color.Transparent;
            subMenu.BannerText.Font = GTA.UI.Font.Monospace;
            Game.Player.Character.Weapons.Give(WeaponHash.Widowmaker, 300, false, true);
        }
        void Reload()
        {

            int currentMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "currentMode", 1);
            int trgtMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "targetingMode", 0);
            responsibility.Checked = Settings.GetValue<bool>("FORSCRIPTTOREAD", "responsibility", true);
            toggleRadioAudio.Checked = Settings.GetValue<bool>("FORSCRIPTTOREAD", "radio", true);
            toggleJetAudio.Checked = Settings.GetValue<bool>("FORSCRIPTTOREAD", "jetAudio", true);
            toggleHud.Checked = Settings.GetValue<bool>("FORSCRIPTTOREAD", "toggleHud", true);
            toggleBlips.Checked = Settings.GetValue<bool>("FORSCRIPTTOREAD", "blips", true);

            if (trgtMode == 1)
            {
                targetingMode.SelectedItem = "IR Spotlight";
            }
            else if (trgtMode == 0)
            {
                targetingMode.SelectedItem = "Flare";
            }

            if (currentMode == 1)
            {
                strikeMode.SelectedItem = "GAU-8 30mm";
            }
            else if (currentMode == 2)
            {
                strikeMode.SelectedItem = "Stealth Bomber";
            }
            else if (currentMode == 3)
            {
                strikeMode.SelectedItem = "EMP Drone Swarm";
            }
        }
        void empChange(object sender, ItemChangedEventArgs<String> e)
        {
            if (empModes.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 0);
                Settings.Save();
                Main.empMode = 0;
            }
            else if (empModes.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 1);
                Settings.Save();
                Main.empMode = 1;
            }
            else if (empModes.SelectedIndex == 2)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "EMPMODE", 2);
                Settings.Save();
                Main.empMode = 2;
            }
        }

        void hudChange(object sender, EventArgs e)
        {
            if (toggleHud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "hideHud", true);
                Settings.Save();
                Main.hideHud = true;
            }
            else if (!toggleHud.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "hideHud", false);
                Settings.Save();
                Main.hideHud = false;
            }
        }
        void pBlipsChange(object sender, EventArgs e)
        {
            if (toggleBlips.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "planeBlips", true);
                Settings.Save();
                Main.blipEnabled = true;
            }
            else if (!toggleBlips.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "planeBlips", false);
                Settings.Save();
                Main.blipEnabled = false;
            }
        }
        void rad2Change(object sender, EventArgs e)
        {
            Settings.SetValue("FORSCRIPTTOREAD", "radius2", missileRadius.Value);
            Settings.Save();
            Main.radius2 = missileRadius.Value;
            missileRadius.Description = "Radius of the missile barrage:  ~y~" + (missileRadius.Value).ToString();
        }
        void radChange(object sender, EventArgs e)
        {
            Settings.SetValue("FORSCRIPTTOREAD", "radius", radiusSlider.Value);
            Settings.Save();
            Main.radius = radiusSlider.Value;
            radiusSlider.Description = "Radius of the gattling gun: ~y~" + (radiusSlider.Value).ToString();
        }
        void heightChange(object sender, EventArgs e)
        {

            Settings.SetValue("FORSCRIPTTOREAD", "height", heightSlider.Value + 150);
            Settings.Save();
            Main.height = heightSlider.Value + 150;
            heightSlider.Description = "The height at which planes will spawn: ~y~" + (heightSlider.Value + 150).ToString();
        }
        void radAudChange(object sender, EventArgs e)
        {
            if (toggleRadioAudio.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "radioAudio", true);
                Settings.Save();
                Main.radioAudio = true;
            }
            else if (!toggleRadioAudio.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "radioAudio", false);
                Settings.Save();
                Main.radioAudio = false;
            }
        }
        void respnChange(object sender, EventArgs e)
        {
            if (responsibility.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "responsibility", true);
                Settings.Save();
                Main.resp = true;
            }
            else if (!responsibility.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "responsibility", false);
                Settings.Save();
                Main.resp = false;
            }
        }
        void jetAudChange(object sender, EventArgs e)
        {
            if (toggleJetAudio.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "jetAudio", true);
                Settings.Save();
                Main.jetAudio = true;
            }
            else if (!toggleJetAudio.Checked)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "jetAudio", false);
                Settings.Save();
                Main.jetAudio = false;
            }
        }
        void trgtChange(object sender, ItemChangedEventArgs<string> e)
        {
            if (targetingMode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
                Main.aimMode = 0;
                Game.Player.Character.Weapons.Give(WeaponHash.Flare, 0, true, true);
            }
            else if (targetingMode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 1);
                Settings.Save();
                Main.aimMode = 1;
                Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 0, true, true);

            }
        }
        void trgtChange2(object sender, EventArgs e)
        {
            if (targetingMode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
                Main.aimMode = 0;
                Game.Player.Character.Weapons.Give(WeaponHash.Flare, 0, true, true);
            }
            else if (targetingMode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 1);
                Settings.Save();
                Main.aimMode = 1;
                Game.Player.Character.Weapons.Give(WeaponHash.Flashlight, 0, true, true);

            }
        }
        void strikeChange(object sender, ItemChangedEventArgs<string> e)
        {
            if (strikeMode.SelectedIndex == 0)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 1);
                Settings.Save();
                Main.mode = 1;
                strikeMode.Description = "2 jets with 30mm gatling cannons, delivering a burst of ~r~280~s~ rounds each. Precise and deadly.";
            }
            else if (strikeMode.SelectedIndex == 1)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 2);
                Settings.Save();
                Main.mode = 2;
                strikeMode.Description = "A stealth bomber that releases a line of explosives along its path. Good for cleaning whole streets.";
            }
            else if (strikeMode.SelectedIndex == 2)
            {
                Settings.SetValue("FORSCRIPTTOREAD", "currentMode", 3);
                Settings.Save();
                Main.mode = 3;
                strikeMode.Description = "20 drones, each carrying ~r~15~s~ small missiles and an EMP generator. When the swarm gets close to the target it generates an electro magnetic pulse, disabling all vehicles & aircraft in area, and then launches a barrage of missiles at the target. Good against helis.";
            }
        }

        void onKeyUp(object sender, KeyEventArgs e)
        {
            if (!Main.controller)
            {
                if (e.KeyCode == Main.showMenuKey && Menu.menu.Visible == false && Menu.subMenu.Visible == false)
                {
                    Menu.menu.Visible = true;
                }
                else if (e.KeyCode == Main.showMenuKey && Menu.menu.Visible == true)
                {
                    Menu.menu.Visible = false;
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
