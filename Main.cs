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

namespace Airstrike
{
    public class Main : Script
    {
        List<Vehicle> jets = new List <Vehicle>();
        bool planeActive = false;
        public static bool modeLock = false;
        public static int aimMode=1;
        bool isDone = false;
        Ped pilot;
        Ped owner;
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
        bool spamBlock = false;
        ObjectPool pool = new ObjectPool();
        public static Vector3 target;
        public static bool hudDis = false;
        public static NativeMenu menu = new NativeMenu("Precision Airstrikes");
        public static NativeListItem<String> strikeMode = new NativeListItem<string>("Mode", "", "Gatling gun", "Missile barrage" , "Drone swarm");
        public static NativeCheckboxItem respons = new NativeCheckboxItem("Responsibility", "If set to true, peds will be aware you are the one responsible for the airstrike (~b~including ~r~cops~s~). If set to false, pilots take the blame.");
        public static NativeCheckboxItem jetAud = new NativeCheckboxItem("Jet strike audio");
        public static NativeCheckboxItem radAud = new NativeCheckboxItem("Radio audio");
        public static NativeListItem<String> trgtmode = new NativeListItem<String>("Targeting mode", "Choose whether to use a ~r~flare~s~, or an ~p~IR Spotlight~s~ (visible only to pilots) to mark your target.", "Flare", "IR Spotlight");

        void clearMem()
        {
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));

            List<Vehicle> musor1 = new List<Vehicle>(World.GetAllVehicles(model));
            List<Vehicle> musor2 = new List<Vehicle>(World.GetAllVehicles(model1));
            List<Vehicle> musor3 = new List<Vehicle>(World.GetAllVehicles(VehicleHash.Starling));
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
            System.GC.Collect();

        }
        void loadSettings()
        {
            resp = Settings.GetValue<bool>("SETTINGS", "responsibility", true);
            if (resp)
            {
                respons.Checked = true;
            }
            else if (!resp)
            {
                respons.Checked = false;
            }
            int currentMode = Settings.GetValue<int>("SETTINGS", "currentMode", 1);
            mode = currentMode;
            if (currentMode == 1)
            {
                strikeMode.SelectedItem = "Gatling gun";
            }
            else if (currentMode == 2)
            {
                strikeMode.SelectedItem = "Missile barrage";
            }
            else if(currentMode == 3)
            {
                strikeMode.SelectedItem = "Drone swarm";
            }
            jetAudio = Settings.GetValue<bool>("SETTINGS", "jetAudio", true);
            if (jetAudio)
            {
                jetAud.Checked = true;
            }
            else if (!jetAudio)
            {
                jetAud.Checked = false;
            }
            radioAudio = Settings.GetValue<bool>("SETTINGS", "radioAudio", true);
            if (radioAudio)
            {
                radAud.Checked = true;
            }
            else if (!radioAudio)
            {
                radAud.Checked = false;
            }
            aimMode = Settings.GetValue<int>("SETTINGS", "targetingMode", 1);
            if(aimMode == 1)
            {
                trgtmode.SelectedItem = "IR Spotlight";
            }
            else if (aimMode == 0)
            {
                trgtmode.SelectedItem = "Flare";
            }
            blipEnabled = Settings.GetValue<bool>("SETTINGS", "blips", true);
            radius = Settings.GetValue<float>("SETTINGS", "radius", 1f);
            radius2 = Settings.GetValue<float>("SETTINGS", "radius2", 1f);
            timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            model = Settings.GetValue<String>("SETTINGS", "jetModel", "lazer");
            model1 = Settings.GetValue<String>("SETTINGS", "bomberModel", "volatol");
            height = Settings.GetValue<float>("SETTINGS", "height", 300.0f);
            camKey = Settings.GetValue<Keys>("SETTINGS", "camKey", Keys.R);
            showMenuKey = Settings.GetValue<Keys>("SETTINGS", "showMenuKey", Keys.Delete);
        }
        public Main()
        {
            //UI
           
            pool.Add(menu);
            menu.UseMouse = false;
            menu.Add(strikeMode);
            menu.Add(respons);
            menu.Add(jetAud);
            menu.Add(radAud);
            menu.Add(trgtmode);
            menu.Banner.Color = Color.Transparent;
            menu.TitleFont = GTA.UI.Font.Monospace;
            
            bool debug = false;
            loadSettings();
            clearMem();
            Tick += onTick;

         
            void onTick(object sender, EventArgs e)
            {

                Ped player = Game.Player.Character;
                if (timeOut - timePass > 0)
                {
                    menu.Subtitle = "~r~Timeout: ~s~" + (timeOut - timePass);
                }
                else { menu.Subtitle = "~g~Ready~s~, awaiting target input"; }
                if (debug && jets.Count < 2) { GTA.UI.Screen.ShowSubtitle("isDone= " + isDone, 2000); }
                else if(debug && jets.Count >= 2) { GTA.UI.Screen.ShowSubtitle("isDone= " + isDone + " Distance to target: " + jets[0].Position.DistanceTo(player.Position), 2000); }
                if (modeLock)
                {
                    strikeMode.Enabled = false;
                    respons.Enabled = false;
                    jetAud.Enabled = false;
                    radAud.Enabled = false;
                    trgtmode.Enabled = false;
                }
                if (!modeLock)
                {
                    strikeMode.Enabled = true;
                    respons.Enabled = true;
                    jetAud.Enabled = true;
                    radAud.Enabled = true;
                    trgtmode.Enabled = true;
                }
                pool.Process();
               
              
                if(!planeActive || jets.Count == 0)
                {
                    isDone = false;
                    timer = 0;
                }
                if (planeActive && jets.Count >= 2)
                {
                    if (timer >= 50000)
                    {
                        isDone = true;
                        unCallPlane(true);
                        timer = 0;
                    }
                    if (jets.Count >= 2)
                    {
                        if (!jets[0].IsInRange(player.Position, 3500f) || !jets[1].IsInRange(player.Position, 3500f))
                        {
                            isDone = true;
                            unCallPlane(false);
                        }
                    }
                    if (isDone && jets.Count >= 2)
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
                            if (planeActive && jets.Count >= 2)
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

                            if (Function.Call<bool>(Hash.IS_CONTROL_JUST_PRESSED, 2, 329) && !isMidStrike)
                            {
                                Function.Call(Hash.SET_DISABLE_AMBIENT_MELEE_MOVE, player, true);
                                Function.Call(Hash.DISABLE_PLAYER_FIRING, player, true);
                                if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                                {
                                    modeLock = true;
                                    callPlane(target);

                                }
                                if (planeActive && jets.Count >= 2)
                                {
                                    strike(target, pilot, jets);
                                }
                            }
                        }
                    }
                   
                    if (isMidStrike && planeActive && jets.Count >= 2 && !isDone)
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
                switch (mode)
                {
                    case 1:
                        for (int i = 1; i <= 2; i++)
                        {
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                            if (i == 1)
                            {
                                plane = World.CreateVehicle(model, spwn, 0);
                                if (plane == null)
                                {
                                    GTA.UI.Screen.ShowHelpText("~r~Invalid ~s~model name. Please make sure the plane model specified in the .ini file is ~g~correct~s~, and ~y~restart~s~ the script", 1500, true, false);
                                    unCallPlane(true);
                                    
                                }
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
                                if (plane == null)
                                {
                                    GTA.UI.Screen.ShowHelpText("~r~Invalid ~s~model name. Please make sure the plane model specified in the .ini file is ~g~correct~s~, and ~y~restart~s~ the script", 1500, true, false);
                                    unCallPlane(true);
                                    
                                }
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
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, height / 6.25f, height / 6.25f);
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
                        for (int i = 1; i <= 2; i++)
                        {
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                            if (i == 1)
                            {
                                plane = World.CreateVehicle(model1, spwn, 0);
                                if (plane == null)
                                {
                                    GTA.UI.Screen.ShowHelpText("~r~Invalid ~s~model name. Please make sure the plane model specified in the .ini file is ~g~correct~s~, and ~y~restart~s~ the script", 1500, true, false);
                                    unCallPlane(true);
                                    return;
                                }
                                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                                plane.DirtLevel = 1000f;
                                plane.Mods.PrimaryColor = VehicleColor.WornDarkGreen;
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
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 100f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);
                                Wait(50);
                            }
                            else
                            if (i == 2)
                            {
                                plane = World.CreateVehicle(model1, spwn1, 0);
                                if (plane == null)
                                {
                                    GTA.UI.Screen.ShowHelpText("~r~Invalid ~s~model name. Please make sure the plane model specified in the .ini file is ~g~correct~s~, and ~y~restart~s~ the script", 1500, true, false);
                                    unCallPlane(true);
                                    return; 
                                }
                                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                                plane.DirtLevel = 1000f;
                                plane.Mods.PrimaryColor = VehicleColor.WornDarkGreen;
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
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 100f, 0f, hed,(target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);
                                if (radioAudio)
                                {
                                    j = 0;
                                    spamBlock = false;
                                    int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                                    playSfx(rndFx);
                                }

                            }
                            isDone = false;
                            timer = 0;

                        }
                        break;
                    case 3:
                       for(int i = 1; i <= 20; i++)
                        {
                            Vector3 spwn3 = new Vector3(target.X - i * 10, target.Y - 3000f - i * 3, target.Z + height);
                            plane = World.CreateVehicle(VehicleHash.Starling, spwn3, 0);
                            if (plane == null)
                            {
                               GTA.UI.Screen.ShowHelpText("~r~Invalid ~s~model name. Please make sure the plane model specified in the .ini file is ~g~correct~s~, and ~y~restart~s~ the script", 1500, true, false);
                                unCallPlane(true);
                                return;
                            }
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
                            Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 100f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
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
                cam = World.CreateCamera(jets[1].Position, jets[1].Rotation, 110f);
                if (mode == 1)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[1].Driver, 31086, 0.0f, 90.0f, 28.0f, 0f, -15f, 3.57f, true);
                }
                else if (mode == 2)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[1].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -35f, 3.57f, true);
                }
                else if (mode == 3)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[15].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -10f, 0.2f, true);
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
                            if (jetAudio)
                            {
                                disposeAudio();
                            }
                            World.RenderingCamera = null;
                            World.DestroyAllCameras();
                            isMidStrike = false;
                            cameraSet = false;
                            modeLock = false;
                            timer = 0;
                            Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                            hudDis = false;
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
                if (jets.Count >= 2)
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
                        if (jet.Position.DistanceTo(target) < 600)
                        {
                            if (resp) { owner = player; }
                            else if (!resp) { owner = pilot; }
                            i = 1;
                            
                            switch (mode)
                            {
                                case 1:
                                    if (jetAudio && j == 0 && mode == 1)
                                    {
                                        j++;
                                        spamBlock = false;
                                        playSfx(1);
                                        // int Fx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 20);
                                        //if (Fx % 2 == 0) { playSfx(0); }
                                        // else if (Fx % 2 != 0) { playSfx(1); }

                                    }
                                    while (i <= 41 && !isDone)
                                    {
                                        foreach (Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 5.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) - offset * 41 / i, owner, WeaponHash.Railgun, 100, -1);
                                            Wait(5);
                                            if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                                            i++;
                                        }
                                       

                                    }
                                    i = 1;
                                    Wait(20);
                                    while (i <= 41 && !jet.IsDead && !isDone)
                                    {
                                        foreach(Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 5.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) + offset * i, owner, WeaponHash.Railgun, 100, -1);
                                            Wait(5);
                                            if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                                            i++;
                                            if (i >= 41)
                                            {
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jetFire.ForwardSpeed = spd;
                                            }
                                        }
                                       
                                    }
                                    break;
                                case 2:
                                    while (i <= 45 && !jet.IsDead && !isDone)
                                    {
                                        foreach(Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 7.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)) - offset * 45 / i, owner, WeaponHash.HomingLauncher, 100, -1);
                                            Wait(10);
                                            if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED1: " + i, 1000, false, false); }
                                            i++;
                                        }
                                       
                                    }
                                    i = 1;
                                    Wait(700);
                                    while (i <= 45 && !jet.IsDead && !isDone)
                                    {
                                        foreach(Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            Vector3 offset = RotationToDirection(jetFire.Rotation);
                                            Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 7.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)) + offset * i, owner, WeaponHash.HomingLauncher, 100, -1);
                                            Wait(10);
                                            if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED2: " + i, 1000, false, false); }
                                            i++;
                                            if (i >= 45)
                                            {
                                                delayTime = Game.GameTime;
                                                isDone = true;
                                                jetFire.ForwardSpeed = spd;
                                            }
                                        }
                                       
                                    }
                                    break;
                                case 3:
                                   
                                    while (i <= 20 && !jet.IsDead && !isDone)
                                    {
                                            jet.ForwardSpeed = spd;
                                        Vector3 offset = RotationToDirection(jet.Rotation);
                                        foreach(Vehicle jet1 in jets)
                                        {
                                            Vector3 jet0 = new Vector3(jet1.Position.X, jet1.Position.Y, jet1.Position.Z - 7.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)) - offset * 20 / i, owner, WeaponHash.HomingLauncher, 100, -1);
                                            Wait(20);
                                        }
                                            if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED1: " + i, 1000, false, false); }
                                            i++;
                                            if (i >= 20)
                                            {
                                            List<Vehicle> victims = new List<Vehicle>(World.GetAllVehicles());
                                            foreach (Vehicle v in victims)
                                            {
                                                if (v.Model != VehicleHash.Starling)
                                                {
                                                    v.EngineHealth = 0f;
                                                    v.IsEngineRunning = false;
                                                    v.IsConsideredDestroyed = false;
                                                    v.StartAlarm();
                                                    if (v.IsHelicopter)
                                                    {
                                                        //v.HeliBladesSpeed = 9.5f;
                                                        v.HeliEngineHealth = 0f;
                                                    }
                                                }
                                            }
                                            World.AddExplosion(target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)), ExplosionType.Tanker, 15f, 1f, null, true, false);
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
