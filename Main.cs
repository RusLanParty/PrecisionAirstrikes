using System;
using System.Collections.Generic;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NAudio.Wave;
using LemonUI;
using LemonUI.Menus;
using System.Drawing;
using System.Diagnostics;

namespace PrecisionAirstrikes
{
    public class Main : Script
    {
        List<Vehicle> jets = new List<Vehicle>();
        public static List<Prop> bombs = new List<Prop>();
        bool planeActive = false;
        bool init = false;
        public static bool bombsActive = false;
        public static bool modeLock = false;
        public static int aimMode = 1;
        public static int empMode = 0;
        bool isDone = false;
        Ped pilot;
        public static Ped owner;
        Blip blip;
        float spd = 185f;
        public static Camera cam;
        public static bool cameraSet;
        public static bool cameraSet1;
        public static Keys camKey;
        public static Keys showMenuKey;
        int i;
        public static bool resp;
        public static int mode;
        int j = 0;
        public static int delayTime = 0;
        public static int newTime;
        public static int timePass;
        int timer = 0;
        bool isMidStrike = false;
        public static bool poolLoaded = false;
        public static float height;
        public static bool blipEnabled;
        public static bool jetAudio;
        public static bool radioAudio;
        public static float radius;
        public static float radius2;
        public static int timeOut;
        public static string model;
        public static string model1;
        public static string model2;
        private WaveFileReader wave;
        private DirectSoundOut output;
        bool shown = false;
        bool spamBlock = false;
        public static bool controller = false;
        public static int shw;
        public static int conCam;
        public static Vector3 target;
        public static bool hudDis = false;
        public static bool hideHud = false;
        void ClearMemory()
        {
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));
            List<Vehicle> musor1 = new List<Vehicle>(World.GetAllVehicles(model));
            List<Vehicle> musor2 = new List<Vehicle>(World.GetAllVehicles(model1));
            List<Vehicle> musor3 = new List<Vehicle>(World.GetAllVehicles(VehicleHash.Starling));
            List<Prop> musor4 = new List<Prop>(World.GetAllProps("prop_ld_bomb_01_open"));
            List<Prop> musor5 = new List<Prop>(World.GetAllProps("prop_ld_bomb_anim"));
            DisposeAudio();
            foreach (Ped p in musor)
            {
                p.Delete();
            }
            foreach (Vehicle p in musor1)
            {
                p.Delete();
            }
            foreach (Vehicle p in musor2)
            {
                p.Delete();
            }
            foreach (Vehicle p in musor3)
            {
                p.Delete();
            }
            foreach (Prop p in musor4)
            {
                p.Delete();
            }
            foreach (Prop p in musor5)
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
            if (World.RenderingCamera != null)
            {
                World.RenderingCamera = null;
                World.DestroyAllCameras();
            }

        }
        void LoadSettings()
        {
            model = Settings.GetValue<String>("SETTINGS", "JETMODEL", "lazer");                                   
            model1 = Settings.GetValue<String>("SETTINGS", "STEALTHJETMODEL", "volatol");
            model2 = Settings.GetValue<String>("SETTINGS", "DRONEMODEL", "starling");
            timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            if (timeOut < 0)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~timeOut~s~ value specified in the ini is set to a negative number, and has been reset to default.", true);
                timeOut = 30000;
                Settings.SetValue("SETTINGS", "timeOut", 30000);
                Settings.Save();
            }

            controller = Settings.GetValue<bool>("GAMEPAD", "enabled", false);
            shw = Settings.GetValue<int>("GAMEPAD", "gamePadShowMenu", 56);
            conCam = Settings.GetValue<int>("GAMEPAD", "gamePadJetCam", 57);
            resp = Settings.GetValue<bool>("FORSCRIPTTOREAD", "responsibility", true);
            mode = Settings.GetValue<int>("FORSCRIPTTOREAD", "currentMode", 1);
            jetAudio = Settings.GetValue<bool>("FORSCRIPTTOREAD", "jetAudio", true);
            radioAudio = Settings.GetValue<bool>("FORSCRIPTTOREAD", "radioAudio", true);
            aimMode = Settings.GetValue<int>("FORSCRIPTTOREAD", "targetingMode", 0);

            if (aimMode != 1 && aimMode != 0)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~aimMode~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                aimMode = 0;
                Settings.SetValue("FORSCRIPTTOREAD", "targetingMode", 0);
                Settings.Save();
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
            if (empMode != 0 && empMode != 1 && empMode != 2)
            {
                GTA.UI.Notification.Show("~r~PRECISION AIRSTRIKES:~s~ The ~y~empMode~s~ value specified in the ini is ~r~incorrect~s~, and has been reset to default. Please use the menu to configure the mod.", true);
                empMode = 0;
                Settings.SetValue("FORSCRIPTTOREAD", "empMode", 0);
                Settings.Save();
            }
            camKey = Settings.GetValue<Keys>("SETTINGS", "camKey", Keys.R);
            showMenuKey = Settings.GetValue<Keys>("SETTINGS", "showMenuKey", Keys.Delete);
            hideHud = Settings.GetValue<bool>("FORSCRIPTTOREAD", "hideHud", true);
            init = true;
        }
        public Main()
        {
            Tick += onTick;
        }

        void onTick(object sender, EventArgs e)
        {
            if (Game.IsLoading)
            {
                return;
            }

            Ped player = Game.Player.Character;

            if (!init)
            {
                LoadSettings();
                ClearMemory();
            }

            if (!planeActive || jets.Count == 0)
            {
                isDone = false;
                timer = 0;
            }
            if (planeActive && jets.Count >= 1)
            {
                if (timer >= 70000)
                {
                    isDone = true;
                    DespawnPlanes(true);
                    timer = 0;
                }

                if (jets.Count >= 1)
                {
                    if (!jets[0].IsInRange(player.Position, 3500f))
                    {
                        isDone = true;
                        DespawnPlanes(false);
                    }
                }
                if (isDone && jets.Count >= 1)
                {
                    DespawnPlanes(false);
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
                    newTime = Game.GameTime;
                    timePass = newTime - delayTime;
                    OutputArgument projectPos = new OutputArgument();
                    if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && (timePass > timeOut || newTime < timeOut))
                    {
                        target = projectPos.GetResult<Vector3>();
                        if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                        {
                            modeLock = true;
                            SpawnPlanes(target);
                        }
                        if (planeActive && jets.Count >= 1)
                        {
                            Shoot(target, pilot, jets);
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
                        else if (mode == 2)
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
                                SpawnPlanes(target);

                            }
                            if (planeActive && jets.Count >= 1)
                            {
                                Shoot(target, pilot, jets);
                            }
                        }
                    }
                }

                if (isMidStrike && planeActive && jets.Count >= 1 && !isDone)
                {
                    Shoot(target, pilot, jets);
                }
            }
        }

        void DespawnPlanes(bool instant)
        {
            if (planeActive && isDone)
            {
                if (!jets[0].IsInRange(Game.Player.Character.Position, 3100f))
                {
                    foreach (Vehicle jet in jets)
                    {
                        jet.ForwardSpeed = spd;


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
                            DisposeAudio();
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

                jets.Clear();
                World.RenderingCamera = null;
                World.DestroyAllCameras();
                isMidStrike = false;
                cameraSet = false;
                modeLock = false;
                isDone = false;
                timer = 0;
                Function.Call(Hash.FLASH_MINIMAP_DISPLAY);
                hudDis = false;
            }

        }
        void Shoot(Vector3 target, Ped pilot, List<Vehicle> jets)
        {
            Ped player = Game.Player.Character;
            if (jets.Count >= 1)
            {
                foreach (Vehicle jet in jets)
                {
                    Function.Call(Hash.SET_PLANE_TURBULENCE_MULTIPLIER, jet, 1.0f);
                    jet.ForwardSpeed = spd;
                    jet.Heading = Function.Call<float>(Hash.GET_HEADING_FROM_VECTOR_2D, target.X - jet.Position.X, target.Y - jet.Position.Y);
                    if (jet.Position.DistanceTo(target) > 3500f)
                    {
                        isDone = true;
                        DespawnPlanes(true);
                    }
                    if ((jet.Position.DistanceTo(target) - jet.HeightAboveGround) < 600)
                    {
                        if (resp) { owner = player; }
                        else if (!resp) { owner = pilot; }
                        i = 1;
                        if (jetAudio && j == 0 && mode == 1)
                        {
                            j++;
                            spamBlock = false;
                            PlaySFX(1);
                            // int Fx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 20);
                            //if (Fx % 2 == 0) { PlaySFX(0); }
                            // else if (Fx % 2 != 0) { PlaySFX(1); }

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
                                        i--;
                                    }
                                }
                                i = 0;
                                Wait(800);
                                while (i <= 140 && !jet.IsDead && !isDone)
                                {
                                    foreach (Vehicle jetFire in jets)
                                    {
                                        jetFire.ForwardSpeed = spd;
                                        Vector3 offset = RotationToDirection(jetFire.Rotation);
                                        Vector3 jet0 = new Vector3(jetFire.Position.X, jetFire.Position.Y, jetFire.Position.Z - 1.5f);
                                        World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) + offset * i / 4, owner, WeaponHash.Railgun, 100, -1);
                                        Wait(5);
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
                                if ((jet.Position.DistanceTo(target) - jet.HeightAboveGround) < 25)
                                {
                                    i = 10;
                                    while (i >= 0 && !jet.IsDead && !isDone)
                                    {
                                        foreach (Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            float rnd = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -10f, 10f);
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
                                            i--;
                                        }
                                    }
                                    i = 0;
                                    Wait(20);
                                    while (i <= 10 && !jet.IsDead && !isDone)
                                    {
                                        foreach (Vehicle jetFire in jets)
                                        {
                                            jetFire.ForwardSpeed = spd;
                                            float rnd = Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, -10f, 10f);
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
                                            Wait(60);
                                            i++;
                                            if (i >= 10)
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
                                i = 0;
                                if (jet.Position.DistanceTo(target) < 600)
                                {
                                    while (i <= 15 && !jet.IsDead && !isDone)
                                    {
                                        jet.ForwardSpeed = spd;
                                        foreach (Vehicle jet1 in jets)
                                        {
                                            jet.ForwardSpeed = spd;
                                            Vector3 jet0 = new Vector3(jet1.Position.X, jet1.Position.Y, jet1.Position.Z - 6.5f);
                                            World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)), owner, WeaponHash.HomingLauncher, 100, -1);
                                            Wait(1);
                                        }
                                        i++;
                                        if (i >= 15 && !isDone)
                                        {
                                            isDone = true;
                                            switch (empMode)
                                            {
                                                case 0:
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
                                                    World.AddExplosion(target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 10f)), ExplosionType.FireWork, 15f, 1f, null, true, false);
                                                    Wait(300);
                                                    Function.Call(Hash.FORCE_LIGHTNING_FLASH);
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
                                                    Function.Call(Hash.FORCE_LIGHTNING_FLASH);
                                                    Wait(64);
                                                    World.Blackout = true;
                                                    Wait(1000);
                                                    World.Blackout = false;
                                                    delayTime = Game.GameTime;
                                                    jet.ForwardSpeed = spd;
                                                    break;

                                                case 1:
                                                    List<Vehicle> victims1 = new List<Vehicle>(World.GetAllVehicles());
                                                    foreach (Vehicle v in victims1)
                                                    {
                                                        jet.ForwardSpeed = spd;
                                                        if (v.Model != VehicleHash.Starling && (v.IsAircraft || v.IsHelicopter))
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

                                                    World.AddExplosion(target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 10f)), ExplosionType.FireWork, 15f, 1f, null, true, false);
                                                    Wait(300);
                                                    Function.Call(Hash.FORCE_LIGHTNING_FLASH);
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
                                                    Function.Call(Hash.FORCE_LIGHTNING_FLASH);
                                                    Wait(64);
                                                    World.Blackout = true;
                                                    Wait(1000);
                                                    World.Blackout = false;
                                                    delayTime = Game.GameTime;
                                                    jet.ForwardSpeed = spd;
                                                    break;
                                                case 2:
                                                    delayTime = Game.GameTime;
                                                    jet.ForwardSpeed = spd;
                                                    break;
                                            }
                                        }
                                    }
                                }

                                break;
                        }
                    }
                    else if ((jet.Position.DistanceTo(target) > 600) && jet.Position.DistanceTo(target) < 3000f)
                    {
                        timer++;
                    }
                }
            }
        }
        void SpawnPlanes(Vector3 target)
        {
            Ped player = Game.Player.Character;
            Vehicle plane;
            Vector3 position = new Vector3(target.X - 20f, target.Y - 3000f, target.Z + height);
            Vector3 position2 = new Vector3(target.X + 45.0f, target.Y - 2940f, target.Z + height);

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
                        if (i == 1)
                        {
                            plane = World.CreateVehicle(model, position, 0);
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
                            plane = World.CreateVehicle(model, position2, 0);
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
                        PlaySFX(rndFx);
                    }
                    break;
                case 2:

                    plane = World.CreateVehicle(model1, position, 0);
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
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z + height, 4, 0f, 0f, hed1, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                    jets.Add(plane);
                    isDone = false;
                    timer = 0;
                    Wait(50);
                    if (radioAudio)
                    {
                        j = 0;
                        spamBlock = false;
                        int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                        PlaySFX(rndFx);
                    }
                    break;
                case 3:
                    for (int i = 1; i <= 20; i++)
                    {
                        Vector3 spwn3 = new Vector3(target.X - i * 10, target.Y - 3000f - i * 3, target.Z + height);
                        plane = World.CreateVehicle(model2, spwn3, 0);
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
                        PlaySFX(rndFx);
                    }
                    isDone = false;
                    timer = 0;
                    break;
            }

            planeActive = true;
            cam = World.CreateCamera(jets[0].Position, jets[0].Rotation, 110f);
            if (mode == 1)
            {
                Function.Call(Hash.HARD_ATTACH_CAM_TO_PED_BONE, cam, jets[0].Driver, 31086, 0.0f, 90.0f, 28.0f, 0f, -15f, 3.57f, true);
            }
            else if (mode == 2)
            {
                Function.Call(Hash.HARD_ATTACH_CAM_TO_PED_BONE, cam, jets[0].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -35f, 3.57f, true);
            }
            else if (mode == 3)
            {
                Function.Call(Hash.HARD_ATTACH_CAM_TO_PED_BONE, cam, jets[6].Driver, 31086, 0.0f, 90.0f, 0.0f, 0f, -8f, 0.3f, true);
            }

            cameraSet = true;
        }
        void PlaySFX(int fx)
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

        void DisposeAudio()
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
        Vector3 RotationToDirection(Vector3 Rotation)
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
