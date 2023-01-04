using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NAudio.Gui;
using NAudio.Wave;
using LemonUI;
using LemonUI.Menus;
using System.Threading;
using System.Drawing;
using GTA.UI;
using LemonUI.Elements;
using LemonUI.Scaleform;

namespace Airstrike
{
    public class Main : Script
    {
        List<Vehicle> jets = new List <Vehicle>();
        public static List<Prop> bombs = new List<Prop>();
        bool planeActive = false;
        bool init = false;
        public static bool bombsActive=false;
        public static bool modeLock = false;
        public static int aimMode=1;
        public static int empMode=0;
        bool isDone = false;
        Ped pilot;
        public static Ped owner;
        Blip blip;
        float spd=185f;
        public static Camera cam;
        public static bool cameraSet;
        public static bool cameraSet1;
        public static Keys camKey;
        public static Keys showMenuKey;
        int i;
        public static bool resp;
        public static int mode;
        int j=0;
        int delayTime = 0;
        int newTime;
        public static int timePass;
        int timer=0;
        bool isMidStrike = false;
        public static bool poolLoaded=false;
        public static float height;
        public static bool blipEnabled;
        public static bool jetAudio;
        public static bool radioAudio;
        public static float radius;
        public static float radius2;
        public static int timeOut;
        public static string model;
        public static string model1;
        private WaveFileReader wave;
        private DirectSoundOut output;
        bool shown = false;
        bool spamBlock = false;
        public static bool controller = false;
        public static int shw;
        public static int conCam;
        public static ObjectPool pool = new ObjectPool();
        public static Vector3 target;
        public static bool hudDis = false;
        public static bool hideHud = false;
        public static NativeMenu menu = new NativeMenu("Precision Airstrikes");
        public static NativeItem credit = new NativeItem("Made by ~t~RusLanParty~s~ (Ruslan Libin)", "Hey, hope you're having a good time with my mod. I spent hours perfecting it, not without the help of random strangers like you who helped me trace issues and come up with creative ideas, so if you have any suggestions or bug reports feel free to contact me on ~g~GTA5MODS~s~. Say \"thank you\": ~p~patreon.com/RusLanParty");
        public static NativeListItem<String> strikeMode = new NativeListItem<String>("Mode", "", "GAU-8 30mm", "Carpet bombing" , "EMP drone swarm");
        public static NativeListItem<String> trgtmode = new NativeListItem<String>("Targeting mode", "Choose whether to use a ~y~flare~s~, or an ~y~IR spotlight~s~ to mark your target. \nYou can press Enter or scroll through the options to equip chosen marker.", "Flare", "IR Spotlight");
        public static NativeMenu subMenu = new NativeMenu("Options", "Options");
        public static NativeCheckboxItem jetAud = new NativeCheckboxItem("GAU-8 sound");
        public static NativeCheckboxItem radAud = new NativeCheckboxItem("Radio chatter");
        public static NativeCheckboxItem respons = new NativeCheckboxItem("Responsibility", "If set to true, peds will be aware you are the one responsible for the airstrike (~b~including ~r~cops~s~). If set to false, pilots take the blame.");
        public static NativeSliderItem heightSlide = new NativeSliderItem("Height");
        public static NativeSliderItem radSlide = new NativeSliderItem("GAU-8 radius");
        public static NativeSliderItem rad2Slide = new NativeSliderItem("Drone missiles radius");
        public static NativeCheckboxItem blipsCheck = new NativeCheckboxItem("Plane blips");
        public static NativeCheckboxItem hudCheck = new NativeCheckboxItem("Hide HUD", "Hides hud while in jet camera");
        public static NativeListItem<String> empList = new NativeListItem<String>("EMP mode", "Normal - all vehicles in range disabled.", "Normal", "Aircraft only", "Disabled");
       

        void clearMem()
        {
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));

            List<Vehicle> musor1 = new List<Vehicle>(World.GetAllVehicles(model));
            List<Vehicle> musor2 = new List<Vehicle>(World.GetAllVehicles(model1));
            List<Vehicle> musor3 = new List<Vehicle>(World.GetAllVehicles(VehicleHash.Starling));
            List<Prop> musor4 = new List<Prop>(World.GetAllProps("prop_ld_bomb_01_open"));
            List<Prop> musor5 = new List<Prop>(World.GetAllProps("prop_ld_bomb_anim"));
            disposeAudio();
            foreach (Ped p in musor)
            {
                p.Delete();
            }
            foreach(Vehicle p in musor1)
            {
                p.Delete();
            }
            foreach (Vehicle p in musor2)
            {
                p.Delete();
            }
            foreach(Vehicle p in musor3)
            {
                p.Delete();
            }
            foreach(Prop p in musor4)
            {
                p.Delete(); 
            }
            foreach(Prop p in musor5)
            {
                p.Delete();
            }
            musor.Clear();
            musor1.Clear();
            musor2.Clear();
            musor3.Clear();
            musor4.Clear();
            musor5.Clear();
            System.GC.Collect();
            if(World.RenderingCamera != null)
            {
                World.RenderingCamera = null;
                World.DestroyAllCameras();
            }

        }
        void loadSettings()
        {
            timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            if (timeOut < 0)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~timeOut~s~ value specified in the ini is set to a negative number, and has been reset to default.", true);
                timeOut = 30000;
                Settings.SetValue("SETTINGS", "timeOut", 30000);
                Settings.Save();
            }
            model = Settings.GetValue<String>("SETTINGS", "JETMODEL", "lazer");
            Vehicle test = World.CreateVehicle(model, Game.Player.Character.Position + Game.Player.Character.UpVector * 20, 0f);
            if (test == null || test != null && !test.IsAircraft)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The jet model specified in the ini is ~r~incorrect~s~, and has been reset to \"lazer\".");
                model = "lazer";
                Settings.SetValue("SETTINGS", "JETMODEL", "lazer");
                Settings.Save();
            }
            else
            { test.Delete(); }
            model1 = Settings.GetValue<String>("SETTINGS", "STEALTHJETMODEL", "volatol");
            Vehicle test1 = World.CreateVehicle(model1, Game.Player.Character.Position + Game.Player.Character.UpVector * 20, 0f);

            if (test1 == null || test1 != null && !test1.IsAircraft)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The jet model specified in the ini is ~r~incorrect~s~, and has been reset to \"volatol\".");
                model1 = "volatol";
                Settings.SetValue("SETTINGS", "STEALTHJETMODEL", "volatol");
                Settings.Save();
            }
            else
            { test1.Delete(); }
            controller = Settings.GetValue<bool>("GAMEPAD", "enabled", false);
            shw = Settings.GetValue<int>("GAMEPAD", "gamePadShowMenu", 56);
            conCam = Settings.GetValue<int>("GAMEPAD", "gamePadJetCam", 57);
            resp = Settings.GetValue<bool>("FORSCRIPTTOREAD", "responsibility", true);
            respons.Checked = resp;
            int currentMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "currentMode", 1);
            mode = currentMode;
            if (currentMode == 1)
            {
                strikeMode.SelectedItem = "GAU-8 30mm";
            }
            else if (currentMode == 2)
            {
                strikeMode.SelectedItem = "Carpet bombing";
            }
            else if(currentMode == 3)
            {
                strikeMode.SelectedItem = "EMP drone swarm";
            }
            jetAudio = Settings.GetValue<bool>("FORSCRIPTTOREAD", "jetAudio", true);
            jetAud.Checked = jetAudio;
            radioAudio = Settings.GetValue<bool>("FORSCRIPTTOREAD", "radioAudio", true);
            radAud.Checked = radioAudio;
            aimMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "targetingMode", 0);
            if(aimMode != 1 && aimMode != 0)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~aimMode~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                aimMode = 0;
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
            }
            if (aimMode == 1)
            {
                trgtmode.SelectedItem = "IR Spotlight";
            }
            else if (aimMode == 0)
            {
                trgtmode.SelectedItem = "Flare";
            }
            height = Settings.GetValue<float>("FORSCRIPTTOREAD", "height", 300.0f);
            if (height > 500 || height < 150)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~height~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                height = 300f;
                Settings.SetValue("FORSCRIPTTOREAD", "height", 300);
                Settings.Save();
            }
            blipEnabled = Settings.GetValue<bool>("FORSCRIPTTOREAD", "planeBlips", true);
            blipsCheck.Checked = blipEnabled;
            radius = Settings.GetValue<float>("FORSCRIPTTOREAD", "radius", 5);
            if (radius > 20 || radius < 0) 
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~radius~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                radius = 5f;
                Settings.SetValue("FORSCRIPTTOREAD", "radius", 5);
                Settings.Save();
            }
            radius2 = Settings.GetValue<float>("FORSCRIPTTOREAD", "radius2", 5);
            if (radius2 > 20 || radius2 < 0)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~radius2~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                radius2 = 5f;
                Settings.SetValue("FORSCRIPTTOREAD", "radius2", 5);
                Settings.Save();
            }
            empMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "EMPMODE", 0);
           if(empMode != 0 && empMode != 1 && empMode != 2)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~empMode~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                empMode = 0;
                Settings.SetValue("FORSCRIPTTOREAD", "empMode", 0);
                Settings.Save();
            }
            camKey = Settings.GetValue<Keys>("SETTINGS", "camKey", Keys.R);
            showMenuKey = Settings.GetValue<Keys>("SETTINGS", "showMenuKey", Keys.Delete);
            hideHud = Settings.GetValue<bool>("FORSCRIPTTOREAD", "hideHud", true);
            hudCheck.Checked = hideHud;

        }
        public Main()
        {
            bool debug = false;
            Tick += onTick;
            
         
            void onTick(object sender, EventArgs e)
            {
                if (Game.IsLoading)
                {
                    return;
                }
                if (!init)
                {
                    loadSettings();
                    clearMem();
                    //UI
                    menu.UseMouse = false;
                    subMenu.UseMouse = false;
                    pool.Add(menu);
                    pool.Add(subMenu);
                    menu.Width = 450f;
                    subMenu.Width = 450f;
                    menu.Add(strikeMode);
                    switch (mode)
                    {
                        case 1:
                            strikeMode.Description = "2 jets with 30mm gatling cannons, delivering a burst of ~r~280~s~ rounds each. Precise and deadly.";
                            break;
                        case 2:
                            Main.strikeMode.Description = "A stealth bomber that releases a line of explosives along its path. Good for cleaning whole streets.";
                            break;
                        case 3:
                            Main.strikeMode.Description = "20 drones, each carrying ~r~15~s~ small missiles and an EMP generator. When the swarm gets close to the target it starts raining down missiles, and then generates an electro magnetic pulse, disabling all vehicles in area, including aircraft. Good against helis.";
                            break;
                    }
                    menu.Add(trgtmode);
                    menu.AddSubMenu(subMenu);
                    menu.Title.Shadow = true;
                    menu.Title.Outline = true;
                    subMenu.Title.Shadow = true;
                    subMenu.Title.Outline = true;
                    heightSlide.Multiplier = 50;
                    heightSlide.Maximum = 350;
                    heightSlide.Value = (int)height - 150;
                    heightSlide.Description = "The height at which planes will spawn: ~y~" + height;
                    radSlide.Maximum = 20;
                    rad2Slide.Maximum = 20;
                    radSlide.Value = (int)radius;
                    radSlide.Description = "Radius of the gattling gun: ~y~" + radius;
                    rad2Slide.Value = (int)radius2;
                    rad2Slide.Description = "Radius of the missile barrage: ~y~" + radius2;
                    subMenu.Add(heightSlide);
                    subMenu.Add(radSlide);
                    subMenu.Add(rad2Slide);
                    subMenu.Add(empList);
                    switch (empMode)
                    {
                        case 0:
                            empList.SelectedIndex = 0;
                            break;
                        case 1:
                            empList.SelectedIndex = 1;
                            break;
                        case 2:
                            empList.SelectedIndex = 2;
                            break;
                    }
                    subMenu.Add(jetAud);
                    subMenu.Add(radAud);
                    subMenu.Add(respons);
                    subMenu.Add(blipsCheck);
                    subMenu.Add(hudCheck);
                    subMenu.Add(credit);
                    subMenu.Description = "~y~Custom jet models~s~, ~y~keybinds~s~, and ~y~timeout~s~ are only configurable via .ini";
                    menu.Banner.Color = Color.Transparent;
                    menu.TitleFont = GTA.UI.Font.Monospace;
                    menu.SubtitleFont = GTA.UI.Font.ChaletComprimeCologne;
                    subMenu.Banner.Color = Color.Transparent;
                    subMenu.TitleFont = GTA.UI.Font.Monospace;
                    Game.Player.Character.Weapons.Give(WeaponHash.Widowmaker, 300, false, true);
                    init = true;
                    poolLoaded = true;
                }
                Ped player = Game.Player.Character;
                if (timeOut - timePass > 0 && newTime >= timeOut)
                {
                    menu.Subtitle = "Timeout: " + (timeOut - timePass);
                }
                else { menu.Subtitle = "Ready. Awaiting target input."; }
                if (debug && jets.Count < 1) { GTA.UI.Screen.ShowSubtitle("isDone= " + isDone, 2000); }
                else if(debug && jets.Count >= 1) { GTA.UI.Screen.ShowSubtitle("isDone= " + isDone + " Distance to target: " + jets[0].Position.DistanceTo(player.Position), 2000); }
                if (modeLock)
                {
                    strikeMode.Enabled = false;
                    respons.Enabled = false;
                    jetAud.Enabled = false;
                    radAud.Enabled = false;
                    trgtmode.Enabled = false;
                    rad2Slide.Enabled = false;
                    radSlide.Enabled = false;
                    blipsCheck.Enabled = false;
                    heightSlide.Enabled = false;
                    hudCheck.Enabled = false;
                    empList.Enabled = false;
                }
                if (!modeLock)
                {
                    strikeMode.Enabled = true;
                    respons.Enabled = true;
                    jetAud.Enabled = true;
                    radAud.Enabled = true;
                    trgtmode.Enabled = true;
                    rad2Slide.Enabled = true;
                    radSlide.Enabled = true;
                    blipsCheck.Enabled = true;
                    heightSlide.Enabled = true;
                    hudCheck.Enabled = true;
                    empList.Enabled = true;
                }
                
                if(!planeActive || jets.Count == 0)
                {
                    isDone = false;
                    timer = 0;
                }
                if (planeActive && jets.Count >= 1)
                {
                    if (timer >= 70000)
                    {
                        isDone = true;
                        unCallPlane(true);
                        timer = 0;
                    }
                    if (jets.Count >= 1)
                    {
                        if (!jets[0].IsInRange(player.Position, 3500f))
                        {
                            isDone = true;
                            unCallPlane(false);
                        }
                    }
                    if (isDone && jets.Count >= 1)
                    {
                        unCallPlane(false);
                    }

                    foreach (Vehicle jet in jets)
                    {
                        jet.ForwardSpeed = spd;
                    }
                }
                newTime = Game.GameTime;
                timePass = newTime - delayTime;
                if (aimMode == 0)
                {
                    if (Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, 22, player.Position.X, player.Position.Y, player.Position.Z, 500f))
                    {
                        if (false) { GTA.UI.Screen.ShowHelpText("Timepass: " + timePass, 2000, false, false); }
                        newTime = Game.GameTime;
                        timePass = newTime - delayTime;
                        OutputArgument projectPos = new OutputArgument();
                        if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && (timePass > timeOut || newTime < timeOut))
                        {
                            target = projectPos.GetResult<Vector3>();
                            if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                            {
                                modeLock = true;
                                callPlane(target);
                            }
                            if (planeActive && jets.Count >= 1)
                            {
                                strike(target, pilot, jets);
                            }
                        }
                    }
                }
                else if (aimMode == 1 && timePass > timeOut || newTime < timeOut)
                {
                   
                    if (!isMidStrike)
                    {
                        
                        if (player.Weapons.Current == WeaponHash.Flashlight && player.IsAiming)
                        {
                           
                            target = rayCast();
                            if (mode == 1)
                            {
                                World.DrawLightWithRange(target + player.UpVector, Color.Indigo, 10f, 10f);
                                World.DrawLine(player.Weapons.CurrentWeaponObject.Position, target, Color.Indigo);
                            }
                            else if(mode == 2)
                            {
                                World.DrawLightWithRange(target + player.UpVector, Color.DarkBlue, 10f, 10f);
                                World.DrawLine(player.Weapons.CurrentWeaponObject.Position, target, Color.DarkBlue);
                            }
                            else if (mode == 3)
                            {
                                World.DrawLightWithRange(target + player.UpVector, Color.DarkGreen, 10f, 10f);
                                World.DrawLine(player.Weapons.CurrentWeaponObject.Position, target, Color.DarkGreen);
                            }
                            
                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 0, 24) && !isMidStrike)
                            {
                                Function.Call(Hash.SET_DISABLE_AMBIENT_MELEE_MOVE, player, true);
                                Function.Call(Hash.DISABLE_PLAYER_FIRING, player, true);
                                if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                                {
                                    modeLock = true;
                                    callPlane(target);

                                }
                                if (planeActive && jets.Count >= 1)
                                {
                                    strike(target, pilot, jets);
                                }
                            }
                        }
                    }
                   
                    if (isMidStrike && planeActive && jets.Count >= 1 && !isDone)
                    {
                            strike(target, pilot, jets);
                    }
                }
            }
            void callPlane(Vector3 target)
            {
                Ped player = Game.Player.Character;
                Vehicle plane;
                Vector3 spwn = new Vector3(target.X, target.Y - 3000f, target.Z + height);
                Vector3 spwn1 = new Vector3(target.X + 45.0f, target.Y - 2940f, target.Z + height);
                isMidStrike = true;
                if (!shown)
                {
                    GTA.UI.Screen.ShowHelpText("Press " + "~y~" + "[" + camKey + "]" + "~s~" + " to toggle jet camera");
                    shown = true;
                }
                
                switch (mode)
                {
                    case 1:
                        for (int i = 1; i <= 2; i++)
                        {
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                            if (i == 1)
                            {
                                plane = World.CreateVehicle(model, spwn, 0);
                                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                                plane.DirtLevel = 1000f;
                                plane.Mods.PrimaryColor = VehicleColor.WornWhite;
                                float hed = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, target.X - plane.Position.X, target.Y - plane.Position.Y);
                                plane.Heading = hed;
                                pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                                plane.IsEngineRunning = true;
                                plane.IsCollisionEnabled = false;
                                plane.LandingGearState = VehicleLandingGearState.Retracted;
                                plane.ForwardSpeed = spd;
                                if (blipEnabled)
                                {
                                    blip = plane.AddBlip();
                                    blip.Color = BlipColor.Purple;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);
                            }
                            else
                            if (i == 2)
                            {
                                plane = World.CreateVehicle(model, spwn1, 0);
                                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                                plane.DirtLevel = 1000f;
                                plane.Mods.PrimaryColor = VehicleColor.WornWhite;
                                float hed = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, target.X - plane.Position.X, target.Y - plane.Position.Y);
                                plane.Heading = hed;
                                pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                                plane.IsEngineRunning = true;
                                plane.IsCollisionEnabled = false;
                                plane.LandingGearState = VehicleLandingGearState.Retracted;
                                plane.ForwardSpeed = spd;
                                if (blipEnabled)
                                {
                                    blip = plane.AddBlip();
                                    blip.Color = BlipColor.Purple;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);
                            }
                            isDone = false;
                            timer = 0;
                            Wait(250);
                        }
                        if (radioAudio)
                        {
                            j = 0;
                            spamBlock = false;
                            int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                            playSfx(rndFx);
                        }
                        break;
                    case 2:
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                           
                                plane = World.CreateVehicle(model1, spwn, 0);
                                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                                plane.DirtLevel = 1000f;
                                plane.Mods.PrimaryColor = VehicleColor.MetallicBlack;
                                float hed1 = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, target.X - plane.Position.X, target.Y - plane.Position.Y);
                                plane.Heading = hed1;
                                pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                                plane.IsEngineRunning = true;
                                plane.IsCollisionEnabled = false;
                                plane.LandingGearState = VehicleLandingGearState.Retracted;
                                plane.ForwardSpeed = spd;
                                
                                if (blipEnabled)
                                {
                                    blip = plane.AddBlip();
                                    blip.Color = BlipColor.Purple;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 0f, 0f, hed1, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);
                                isDone = false;
                                timer = 0;
                                Wait(50);
                                if (radioAudio)
                                {
                                    j = 0;
                                    spamBlock = false;
                                    int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                                    playSfx(rndFx);
                                }
                        break;
                    case 3:
                       for(int i = 1; i <= 20; i++)
                        {
                            Vector3 spwn3 = new Vector3(target.X - i * 10, target.Y - 3000f - i * 3, target.Z + height);
                            plane = World.CreateVehicle(VehicleHash.Starling, spwn3, 0);
                            Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                            plane.DirtLevel = 1000f;
                            plane.Mods.PrimaryColor = VehicleColor.WornBlack;
                            float hed = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, target.X - plane.Position.X, target.Y - plane.Position.Y);
                            plane.Heading = hed;
                            pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                            plane.IsEngineRunning = true;
                            plane.IsCollisionEnabled = false;
                            plane.LandingGearState = VehicleLandingGearState.Retracted;
                            plane.ForwardSpeed = spd;
                            if (blipEnabled)
                            {
                                blip = plane.AddBlip();
                                blip.Color = BlipColor.Purple;
                            }
                            pilot.RelationshipGroup = player.RelationshipGroup;
                            plane.IsInvincible = true;
                            Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 2500f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                            jets.Add(plane);
                            Wait(30);
                        }
                        if (radioAudio)
                        {
                            j = 0;
                            spamBlock = false;
                            int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                            playSfx(rndFx);
                        }
                        isDone = false;
                        timer = 0;
                        break;
                }
               
                planeActive = true;
                cam = World.CreateCamera(jets[0].Position, jets[0].Rotation, 110f);
                if (mode == 1)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[0].Driver, 31086, 0.0f, 90.0f, 28.0f, 0f, -15f, 3.57f, true);
                }
                else if (mode == 2)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[0].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -35f, 3.57f, true);
                }
                else if (mode == 3)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[6].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -8f, 0.3f, true);
                }

                cameraSet = true;
            }
            void unCallPlane(bool instant)
            {
                if (planeActive && isDone)
                {
                    if (!jets[0].IsInRange(Game.Player.Character.Position, 3100f))
                    {
                        foreach (Vehicle jet in jets)
                        {
                            jet.ForwardSpeed = spd;
                            
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets despawned...", 2000, false, false); }

                            if (jet.Driver != null)
                            {
                                jet.Driver.Delete();
                            }
                            if (jet != null)
                            {
                                jet.Delete();
                            }
                        }
                        jets.Clear();
                        if (jets.Count <= 0)
                        {
                            planeActive = false;
                            isDone = false;
                            timer = 0;
                            if (jetAudio || radioAudio)
                            {
                                disposeAudio();
                                j = 0;
                            }
                            World.RenderingCamera = null;
                            World.DestroyAllCameras();
                            isMidStrike = false;
                            cameraSet = false;
                            cameraSet1 = false;
                            modeLock = false;
                            timer = 0;
                            Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                            hudDis = false;
                            shown = false;
                            j = 0;
                        }
                    }
                }

               else if (instant)
                {
                    foreach (Vehicle jet in jets)
                    {
                        if (jet.Driver != null)
                        {
                            jet.Driver.Delete();
                        }
                        if (jet != null)
                        {
                            jet.Delete();
                        }
                    }
                    if (debug) { GTA.UI.Screen.ShowHelpText("Jets despawned instantly...", 2000, false, false); }
                    jets.Clear();
                    World.RenderingCamera = null;
                    World.DestroyAllCameras();
                    isMidStrike = false;
                    cameraSet = false;
                    modeLock = false;
                    isDone=false;
                    timer = 0;
                    Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                    hudDis = false;
                }

            }
            void strike(Vector3 target, Ped pilot, List<Vehicle> jets)
            {
                Ped player = Game.Player.Character;
                if (jets.Count >= 1)
                {
                    
                    foreach(Vehicle jet in jets)
                    {
                        Function.Call(Hash.SET_PLANE_TURBULENCE_MULTIPLIER, jet, 1.0f);
                        jet.ForwardSpeed = spd;
                        if (jet.Position.DistanceTo(target) > 3500f)
                        {
                                isDone = true;
                            unCallPlane(true);
                        }
                        if (jet.Position.DistanceTo(target) < 1040)
                        {
                            if (resp) { owner = player; }
                            else if (!resp) { owner = pilot; }
                            i = 1;
                            if (jetAudio && j == 0 && mode == 1)
                            {
                                j++;
                                spamBlock = false;
                                playSfx(1);
                                // int Fx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 20);
                                //if (Fx % 2 == 0) { playSfx(0); }
                                // else if (Fx % 2 != 0) { playSfx(1); }

                            }
                            switch (mode)
                            {
                                case 1:
                                    i = 140;
                                    while (i >= 0 && !isDone)
                                    {
                                        foreach (Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 1.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) - offset * i / 4, owner, WeaponHash.Railgun, 100, -1);
                                            Wait(5);
                                            if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                                            i--;
                                        }
                                    }
                                    i = 0;
                                    Wait(800);
                                    while (i <= 140 && !jet.IsDead && !isDone)
                                    {
                                        foreach(Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 1.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) + offset * i / 4, owner, WeaponHash.Railgun, 100, -1);
                                            Wait(5);
                                            if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED 2: " + i, 1000, false, false); }
                                            i++;
                                            if (i >= 140)
                                            {
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jetFire.ForwardSpeed = spd;
                                            }
                                        }
                                    }
                                    break;
                                case 2:
                                    if(jet.Position.DistanceTo(target) < 400)
                                    {
                                    i = 20;
                                        while (i >= 0 && !jet.IsDead && !isDone)
                                        {
                                            foreach (Vehicle jetFire in jets)
                                            {
                                                jetFire.ForwardSpeed = spd;
                                                float rnd = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -8f, 8f);
                                                Vector3 jet0 = new Vector3(jetFire.Position.X - rnd, jetFire.Position.Y, jetFire.Position.Z - 1.5f);
                                                var modelbomb = new Model("prop_ld_bomb_anim");
                                                if (!modelbomb.IsLoaded)
                                                    modelbomb.Request(1000);
                                                var dBomb = World.CreateProp(modelbomb, jet0, true, false);
                                                Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, dBomb.Handle, true);
                                                Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, dBomb.Handle, true);
                                                Function.Call(Hash.SET_ENTITY_LOD_DIST, dBomb.Handle, 1000);
                                                dBomb.Rotation = jetFire.Rotation;
                                                dBomb.Velocity = jetFire.Velocity;
                                                dBomb.Speed = jetFire.Speed;
                                                dBomb.Heading = jetFire.Heading - 180f;
                                                dBomb.RotationVelocity = jetFire.RotationVelocity;
                                                bombs.Add(dBomb);
                                                Wait(30);
                                                if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED1: " + i, 1000, false, false); }
                                                i--;
                                            }
                                        }
                                        i = 0;
                                        Wait(20);
                                        while (i <= 20 && !jet.IsDead && !isDone)
                                        {
                                            foreach (Vehicle jetFire in jets)
                                            {
                                                jetFire.ForwardSpeed = spd;
                                                float rnd = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -8f, 8f);
                                                Vector3 jet0 = new Vector3(jetFire.Position.X - rnd, jetFire.Position.Y, jetFire.Position.Z - 1.5f);
                                                var modelbomb = new Model("prop_ld_bomb_anim");
                                                if (!modelbomb.IsLoaded)
                                                    modelbomb.Request(1000);
                                                var dBomb = World.CreateProp(modelbomb, jet0, true, false);
                                                Function.Call(Hash.SET_ENTITY_RECORDS_COLLISIONS, dBomb.Handle, true);
                                                Function.Call(Hash.SET_ENTITY_LOAD_COLLISION_FLAG, dBomb.Handle, true);
                                                Function.Call(Hash.SET_ENTITY_LOD_DIST, dBomb.Handle, 1000);
                                                dBomb.Rotation = jetFire.Rotation;
                                                dBomb.Velocity = jetFire.Velocity;
                                                dBomb.Speed = jetFire.Speed;
                                                dBomb.Heading = jetFire.Heading - 180f;
                                                dBomb.RotationVelocity = jetFire.RotationVelocity;
                                                bombs.Add(dBomb);
                                                bombsActive = true;
                                                Wait(30);
                                                if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED2: " + i, 1000, false, false); }
                                                i++;
                                                if (i >= 20)
                                                {
                                                    delayTime = Game.GameTime;
                                                    isDone = true;
                                                    jetFire.ForwardSpeed = spd;
                                                }
                                            }
                                        }
                                    }
                                   
                                    break;
                                case 3:
                                    while (i <= 15 && !jet.IsDead && !isDone)
                                    {
                                            jet.ForwardSpeed = spd;
                                        Vector3 offset = RotationToDirection(jet.Rotation);
                                        foreach (Vehicle jet1 in jets)
                                        {
                                            jet.ForwardSpeed = spd;
                                            Vector3 jet0 = new Vector3(jet1.Position.X, jet1.Position.Y, jet1.Position.Z - 6.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)), owner, WeaponHash.HomingLauncher, 100, -1);
                                            Wait(20);
                                            i++;
                                        }    
                                    }
                                    if(i >= 15 && !isDone)
                                    {
                                        switch (empMode)
                                        {
                                            case 0:
                                                if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED1: " + i, 1000, false, false); }
                                                    List<Vehicle> victims = new List<Vehicle>(World.GetAllVehicles());
                                                    foreach (Vehicle v in victims)
                                                    {
                                                        jet.ForwardSpeed = spd;
                                                        if (v.Model != VehicleHash.Starling)
                                                        {
                                                            Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, false, true, true);
                                                            v.EngineHealth = -4000f;
                                                            v.IsDriveable = true;
                                                            v.IsConsideredDestroyed = false;
                                                            if (v.IsHelicopter)
                                                            {
                                                                Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, false, true, true);
                                                                v.HeliEngineHealth = -4000f;
                                                            }
                                                        }
                                                    }
                                                
                                                World.AddExplosion(target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), ExplosionType.EmpLauncherEmp, 15f, 1f, null, true, false);
                                                Wait(300);
                                                World.Blackout = true;
                                                Wait(300);
                                                World.Blackout = false;
                                                Wait(12);
                                                World.Blackout = true;
                                                Wait(45);
                                                World.Blackout = false;
                                                Wait(29);
                                                World.Blackout = true;
                                                Wait(896);
                                                World.Blackout = false;
                                                Wait(64);
                                                World.Blackout = true;
                                                Wait(1000);
                                                World.Blackout = false;
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jet.ForwardSpeed = spd;
                                                break;

                                            case 1:
                                                if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED1: " + i, 1000, false, false); }
                                                    List<Vehicle> victims1 = new List<Vehicle>(World.GetAllVehicles());
                                                    foreach (Vehicle v in victims1)
                                                    {
                                                        jet.ForwardSpeed = spd;
                                                        if (v.Model != VehicleHash.Starling && v.IsAircraft || v.IsHelicopter)
                                                        {
                                                            Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, false, true, true);
                                                            v.EngineHealth = -4000f;
                                                            v.IsDriveable = true;
                                                            v.IsConsideredDestroyed = false;
                                                            if (v.IsHelicopter)
                                                            {
                                                                Function.Call(Hash.SET_VEHICLE_ENGINE_ON, v, false, true, true);
                                                                v.HeliEngineHealth = -4000f;
                                                            }
                                                        }
                                                    }
                                                
                                                World.AddExplosion(target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), ExplosionType.EmpLauncherEmp, 15f, 1f, null, true, false);
                                                Wait(300);
                                                World.Blackout = true;
                                                Wait(300);
                                                World.Blackout = false;
                                                Wait(12);
                                                World.Blackout = true;
                                                Wait(45);
                                                World.Blackout = false;
                                                Wait(29);
                                                World.Blackout = true;
                                                Wait(896);
                                                World.Blackout = false;
                                                Wait(64);
                                                World.Blackout = true;
                                                Wait(1000);
                                                World.Blackout = false;
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jet.ForwardSpeed = spd;
                                                break;
                                            case 2:
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jet.ForwardSpeed = spd;
                                                break;
                                        }
                                    }

                                    break;
                            }
                        }
                        else if ((jet.Position.DistanceTo(target) > 600) && jet.Position.DistanceTo(target) < 3000f)
                        {
                            timer++;
                            if (debug) { GTA.UI.Screen.ShowHelpText("Emergency despawn timer: " + timer, 2000, false, false); }
                        }
                    }
                }
            }
        }
        void playSfx(int fx)
        {
            if (!spamBlock)
            {
                switch (fx)
                {

                    case 1:
                        spamBlock = true;
                        wave = new NAudio.Wave.WaveFileReader("scripts/brt/brt.wav");
                        output = new NAudio.Wave.DirectSoundOut();
                        output.Init(new NAudio.Wave.WaveChannel32(wave));
                        output.Play();
                        break;
                    case 2:
                        spamBlock = true;
                        wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike1.wav");
                        output = new NAudio.Wave.DirectSoundOut();
                        output.Init(new NAudio.Wave.WaveChannel32(wave));
                        output.Play();
                        break;
                    case 3:
                        spamBlock = true;
                        wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike2.wav");
                        output = new NAudio.Wave.DirectSoundOut();
                        output.Init(new NAudio.Wave.WaveChannel32(wave));
                        output.Play();
                        break;
                    case 4:
                        spamBlock = true;
                        wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike3.wav");
                        output = new NAudio.Wave.DirectSoundOut();
                        output.Init(new NAudio.Wave.WaveChannel32(wave));
                        output.Play();
                        break;
                    case 5:
                        spamBlock = true;
                        wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike4.wav");
                        output = new NAudio.Wave.DirectSoundOut();
                        output.Init(new NAudio.Wave.WaveChannel32(wave));
                        output.Play();
                        break;
                }
            }
        }
        void disposeAudio()
        {
            if (output != null)
            {
                if (output.PlaybackState == NAudio.Wave.PlaybackState.Playing)
                {
                    output.Stop();
                    output.Dispose();
                    output = null;
                    spamBlock = false;
                    //GTA.UI.Screen.ShowHelpText("Disposed output", 1000, true, false);
                }
            }
            if (wave != null)
            {
                wave.Dispose();
                wave = null;
                spamBlock = false;
                // GTA.UI.Screen.ShowHelpText("Disposed wave", 1000, true, false);
            }
        }
       
        Vector3 rayCast()
        {
            //get Aim Postion
            Vector3 camPos = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_COORD);
            Vector3 camRot = Function.Call<Vector3>(Hash.GET_GAMEPLAY_CAM_ROT);
            float retz = camRot.Z * 0.0174532924F;
            float retx = camRot.X * 0.0174532924F;
            float absx = (float)Math.Abs(Math.Cos(retx));
            Vector3 camStuff = new Vector3((float)Math.Sin(retz) * absx * -1, (float)Math.Cos(retz) * absx, (float)Math.Sin(retx));
            //AimPostion Result
            RaycastResult ray = World.Raycast(camPos, camPos + camStuff * 1000, IntersectFlags.Everything);
            Vector3 trg = ray.HitPosition;
            return trg;
        }
        public static Vector3 RotationToDirection(Vector3 Rotation)
        {
            float z = Rotation.Z;
            float num = z * 0.0174532924f;
            float x = Rotation.X;
            float num2 = x * 0.0174532924f;
            float num3 = Math.Abs((float)Math.Cos((double)num2));
            return new Vector3
            {
                X = (float)((double)((float)(-(float)Math.Sin((double)num))) * (double)num3),
                Y = (float)((double)((float)Math.Cos((double)num)) * (double)num3),
                Z = (float)Math.Sin((double)num2)
            };
        }
    }
    }
