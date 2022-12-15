﻿using System;
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

namespace Airstrike
{
    public class Main : Script
    {
        List<Vehicle> jets = new List <Vehicle>();
        bool planeActive = false;
        public static bool modeLock = false;
        bool isDone = false;
        Ped pilot;
        Ped owner;
        Blip blip;
        float spd=170f;
        public static Camera cam;
        public static bool cameraSet;
        public static bool cameraSet1;
        public static Keys camKey;
        int i;
        public static int mode;
        int j=0;
        int delayTime = 0;
        int newTime;
        int timePass;
        int timer=0;
        private WaveFileReader wave;
        private DirectSoundOut output;
        bool spamBlock = false;

        public Main()
        {
            bool debug = false;
            int defaultMode = Settings.GetValue<int>("SETTINGS", "defaultMode", 1);
            mode = defaultMode;
            bool blipEnabled = Settings.GetValue<bool>("SETTINGS", "blips", true);
            float radius = Settings.GetValue<float>("SETTINGS", "radius", 1f);
            float radius2 = Settings.GetValue<float>("SETTINGS", "radius2", 1f);
            bool resp = Settings.GetValue<bool>("SETTINGS", "responsibility", true);
            int timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            bool jetAudio = Settings.GetValue<bool>("SETTINGS", "jetAudio", true);
            bool radioAudio = Settings.GetValue<bool>("SETTINGS", "radioAudio", true);
            string model = Settings.GetValue<String>("SETTINGS", "jetModel", "lazer");
            string model1 = Settings.GetValue<String>("SETTINGS", "bomberModel", "volatol");
            float height = Settings.GetValue<float>("SETTINGS", "height", 300.0f);
            camKey = Settings.GetValue<Keys>("SETTINGS", "camKey", Keys.R);
            Tick += onTick;
           
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));
            disposeAudio();
            foreach(Ped p in musor)
            {
                p.Delete();
            }
           
            
           
            void onTick(object sender, EventArgs e)
            {
                Ped player = Game.Player.Character;
                if (radioAudio)
                {
                    GTA.UI.Screen.ShowHelpText("SOUND ON", 2500, true, false);
                }
               
               // GTA.UI.Screen.ShowHelpText("output= " + output + " wave= " + wave, 1000, false, false); 
                if (planeActive && jets.Count == 2)
                {
                    timer++;
                    if (timer >= 2500)
                    {
                        isDone = true;
                        unCallPlane(true);
                        timer = 0;
                    }
                    if (jets.Count == 2)
                    {
                        if (!jets[0].IsInRange(player.Position, 3500f) || !jets[1].IsInRange(player.Position, 3500f))
                        {
                            isDone = true;
                            unCallPlane(false);
                        }
                    }
                    if (isDone && jets.Count == 2)
                    {
                        unCallPlane(false);
                    }

                    if (debug && jets.Count == 2) { GTA.UI.Screen.ShowSubtitle("Distance to target: " + jets[0].Position.DistanceTo(player.Position)); }
                    foreach (Vehicle jet in jets)
                    {
                        jet.ForwardSpeed = spd;
                    }
                }
                if (Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, 22, player.Position.X, player.Position.Y, player.Position.Z, 500f))
                {
                    if (false) { GTA.UI.Screen.ShowHelpText("Timepass: " + timePass, 2000, false, false); }
                    newTime = Game.GameTime;
                    timePass = newTime - delayTime;


                    OutputArgument projectPos = new OutputArgument();

                    if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && (timePass > timeOut || newTime < timeOut))
                    {
                        Vector3 target = projectPos.GetResult<Vector3>();
                        if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                        {
                            modeLock = true;
                            callPlane(target);
                        }
                        if (planeActive && jets.Count == 2)
                        {
                            strike(target, pilot, jets);
                        }
                    }
                }
            }
            void callPlane(Vector3 target)
            {
                Ped player = Game.Player.Character;
                Vehicle plane;
                Vector3 spwn = new Vector3(target.X, target.Y - 3000f, target.Z + height);
                Vector3 spwn1 = new Vector3(target.X + 9.0f, target.Y - 3000f,target.Z + height);
                switch (mode)
                {
                    case 1:
                        for (int i = 1; i <= 2; i++)
                        {
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                            if (i == 1)
                            {
                                plane = World.CreateVehicle(model, spwn, 0);
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
                                    blip.Color = BlipColor.WhiteNotPure;
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
                                    blip.Color = BlipColor.WhiteNotPure;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, height / 6.25f, height / 6.25f);
                                jets.Add(plane);

                            }
                            isDone = false;
                            Wait(250);
                        }
                        break;
                    case 2:
                        for (int i = 1; i <= 2; i++)
                        {
                            if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                            if (i == 1)
                            {
                                plane = World.CreateVehicle(model1, spwn, 0);
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
                                    blip.Color = BlipColor.WhiteNotPure;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, (target.Z + height) / 6.25f, (target.Z + height) / 6.25f);
                                jets.Add(plane);

                            }
                            else
                            if (i == 2)
                            {
                                plane = World.CreateVehicle(model1, spwn1, 0);
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
                                    blip.Color = BlipColor.WhiteNotPure;
                                }
                                pilot.RelationshipGroup = player.RelationshipGroup;
                                plane.IsInvincible = true;
                                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 200f, 0f, hed, height / 6.25f, height / 6.25f);
                                jets.Add(plane);

                            }
                            isDone = false;
                            Wait(500);
                        }
                        break;
                }
                if (radioAudio)
                {
                    j = 0;
                    spamBlock = false;
                    int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                    playSfx(rndFx);
                }
                planeActive = true;
                cam = World.CreateCamera(jets[1].Position, jets[1].Rotation, 95f);
                if (mode == 1)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[1].Driver, 31086, 0.0f, 90.0f, 28.0f, 0f, -15f, 3.57f, true);
                } else if(mode == 2)
                {
                    Function.Call(Hash._ATTACH_CAM_TO_PED_BONE_2, cam, jets[1].Driver, 31086, 0.0f, 90.0f, 5.0f, 0f, -35f, 3.57f, true);
                }
                
                cameraSet = true;
            }
            void unCallPlane(bool instant)
            {
                if(jets.Count == 2)
                {
                    jets[0].ForwardSpeed = spd;
                    jets[1].ForwardSpeed = spd;
                }
                if (isDone && jets.Count == 2)
                {
                    if (instant || !jets[0].IsInRange(Game.Player.Character.Position,3500f) || !jets[1].IsInRange(Game.Player.Character.Position, 3500f))
                    {
                        //if (instant) { GTA.UI.Screen.ShowSubtitle("Time out"); }
                        if (debug) { GTA.UI.Screen.ShowHelpText("Jets despawned...", 2000, false, false); }
                        planeActive = false;
                        isDone = false;
                        timer = 0;
                        if (jetAudio)
                        {
                            disposeAudio();
                        }
                        foreach (Vehicle jet in jets)
                        {
                            if (jet.Driver != null)
                            {
                                jet.Driver.Delete();
                            }
                            jet.Delete();
                        }
                        jets.Clear();
                        World.RenderingCamera = null;
                        World.DestroyAllCameras();
                        cameraSet = false;
                        modeLock= false;
                    }
                }
            }
            void strike(Vector3 target, Ped pilot, List<Vehicle> jets)
            {
                Ped player = Game.Player.Character;
                if (jets.Count == 2)
                {
                    Function.Call(Hash.SET_PLANE_TURBULENCE_MULTIPLIER, jets[0], 1.0f);
                    Function.Call(Hash.SET_PLANE_TURBULENCE_MULTIPLIER, jets[1], 1.0f);
                    jets[0].ForwardSpeed = spd;
                    jets[1].ForwardSpeed = spd;
                    if (jets[0].Position.DistanceTo(target) > 3500f || jets[1].Position.DistanceTo(target) > 3500f)
                    {
                        isDone = true;
                        unCallPlane(true);
                    }
                    if (jets[0].Position.DistanceTo(target) < 600 || jets[1].Position.DistanceTo(target) < 600)
                    {
                        if (resp) { owner = player; }
                        else if (!resp) { owner = pilot; }
                        i = 0;
                        if (jetAudio && j == 0 && mode==1)
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
                                while (i <= 100 && !jets[0].IsDead && !isDone)
                                {
                                    jets[0].ForwardSpeed = spd;
                                    jets[1].ForwardSpeed = spd;
                                    Vector3 offset = RotationToDirection(jets[0].Rotation);
                                    Vector3 jet0 = new Vector3(jets[0].Position.X, jets[0].Position.Y, jets[0].Position.Z - 1.5f);
                                    Vector3 jet1 = new Vector3(jets[1].Position.X, jets[1].Position.Y, jets[1].Position.Z - 1.5f);
                                    World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) + offset * -100 / i, owner, WeaponHash.Railgun, 100, -1);
                                    World.ShootBullet(jet1, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius)) + offset * -100 / i, owner, WeaponHash.Railgun, 100, -1);
                                    if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                                    Wait(10);
                                    i++;
                                    if (i >= 100)
                                    {
                                        delayTime = Game.GameTime;
                                        isDone = true;
                                        jets[0].ForwardSpeed = spd;
                                        jets[1].ForwardSpeed = spd;
                                        j = 0;

                                    }
                                }
                                break;
                            case 2:
                                while (i <= 100 && !jets[0].IsDead && !isDone)
                                {
                                    jets[0].ForwardSpeed = spd;
                                    jets[1].ForwardSpeed = spd;
                                    Vector3 offset = RotationToDirection(jets[0].Rotation);
                                    Vector3 jet0 = new Vector3(jets[0].Position.X, jets[0].Position.Y, jets[0].Position.Z - 2.5f);
                                    Vector3 jet1 = new Vector3(jets[1].Position.X, jets[1].Position.Y, jets[1].Position.Z - 2.5f);
                                    World.ShootBullet(jet0, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)) + offset * i, owner, WeaponHash.HomingLauncher, 100, -1);
                                    World.ShootBullet(jet1, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, radius2)) + offset * i, owner, WeaponHash.HomingLauncher, 100, -1);
                                    if (false) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                                    Wait(10);
                                    i++;
                                    if (i >= 100)
                                    {
                                        delayTime = Game.GameTime;
                                        isDone = true;
                                        jets[0].ForwardSpeed = spd;
                                        jets[1].ForwardSpeed = spd;
                                        j = 0;

                                    }
                                }
                                break;
                        }
                       
                    }
                    else if ((jets[0].Position.DistanceTo(target) > 600) && jets[0].Position.DistanceTo(target) < 3500)
                    {
                        timer++;
                        Wait(0);
                        if (debug) { GTA.UI.Screen.ShowHelpText("Emergency despawn timer: " + timer, 2000, false, false); }

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
                        spamBlock= false;
                       // GTA.UI.Screen.ShowHelpText("Disposed wave", 1000, true, false);
                }   
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
