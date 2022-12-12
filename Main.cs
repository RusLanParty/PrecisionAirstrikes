using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;
using NAudio.Wave;

namespace Airstrike
{
    public class Main : Script
    {
        List<Vehicle> jets = new List <Vehicle>();
        bool planeActive = false;
        bool isDone = false;
        Vehicle plane;
        Ped pilot;
        Ped owner;
        Blip blip;
        Camera cam;
        bool cameraSet;
        bool cameraSet1;
        int i;
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
            bool resp = Settings.GetValue<bool>("SETTINGS", "responsibility", true);
            int timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            bool jetAudio = Settings.GetValue<bool>("SETTINGS", "jetAudio", true);
            bool radioAudio = Settings.GetValue<bool>("SETTINGS", "radioAudio", true);
            string model = Settings.GetValue<String>("SETTINGS", "model", "lazer");
            Keys camKey = Settings.GetValue<Keys>("SETTINGS", "camKey", Keys.NumPad5);
            Tick += onTick;
            KeyDown += onKeyUp;
            List<Vehicle> trash = new List<Vehicle>(World.GetAllVehicles(model));
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));
            disposeAudio();
            foreach(Ped p in musor)
            {
                p.Delete();
            }
            foreach (Vehicle lazer in trash)
            {
                if(lazer.Driver != null)
                {
                    lazer.Driver.Delete();
                }
                lazer.Delete();
            }
            void onKeyUp(object sender, KeyEventArgs e)
            {
                if (cameraSet && e.KeyCode == camKey && !cameraSet1)
                {
                    cameraSet1 = true;
                    World.RenderingCamera = cam;
                }
                else if (cameraSet && e.KeyCode == camKey && cameraSet1)
                {
                    cameraSet1 = false;
                    World.RenderingCamera = null;
                }
            }
            void onTick(object sender, EventArgs e)
            {
               // GTA.UI.Screen.ShowHelpText("output= " + output + " wave= " + wave, 1000, false, false); 
                if (planeActive)
                {
                    timer++;
                    if (timer >= 1000)
                    {
                        isDone = true;
                        unCallPlane(true);
                        timer = 0;
                    }
                    if (!plane.IsInRange(Game.Player.Character.Position, 3210f))
                    {
                        isDone = true;
                        unCallPlane(false);
                    }
                    if (isDone)
                    {
                        unCallPlane(false);
                    }
                    if (debug) { GTA.UI.Screen.ShowSubtitle("Distance to target: " + plane.Position.DistanceTo(Game.Player.Character.Position)); }
                    foreach (Vehicle jet in jets)
                    {
                        jet.ForwardSpeed = 250;
                    }
                }
                if (Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, 22, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 500f))
                {
                    if (debug) { GTA.UI.Screen.ShowHelpText("Timepass: " + timePass, 2000, false, false); }
                    newTime = Game.GameTime;
                    timePass = newTime - delayTime;


                    OutputArgument projectPos = new OutputArgument();

                    if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && (timePass > timeOut || newTime < timeOut))
                    {
                        Vector3 target = projectPos.GetResult<Vector3>();
                        if (!planeActive && timePass > timeOut || !planeActive && newTime < timeOut)
                        {
                            callPlane(target);
                        }
                        if (planeActive)
                        {
                            strike(target, pilot, jets);
                        }
                    }
                }
            }
            void callPlane(Vector3 target)
            {
                Ped player = Game.Player.Character;
                Vector3 spwn = new Vector3(target.X, target.Y, 350f);
                Vector3 spwn1 = new Vector3(target.X + 25f, target.Y + 2f, 351f);
                for (int i = 1; i <= 2; i++)
                {
                    if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                    if (i == 1) { plane = World.CreateVehicle(model, spwn  + player.ForwardVector * -3000, player.Heading); }
                    if (i == 2) { plane = World.CreateVehicle(model, spwn1  + player.ForwardVector * -3001, player.Heading); }
                    pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                    plane.IsEngineRunning = true;
                    plane.LandingGearState = VehicleLandingGearState.Retracted;
                    isDone = false;
                    plane.ForwardSpeed = 300;
                    blip = plane.AddBlip();
                    blip.Color = BlipColor.BlueDark;
                    pilot.RelationshipGroup = player.RelationshipGroup;
                    //plane.IsInvincible = true;
                    //Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X + 2000f, target.Y, 300f, 0, 600f, 0f, target.ToHeading(), 300f, 300f);
                    jets.Add(plane);
                    Wait(20);
                }
                if (radioAudio)
                {
                    j = 0;
                    spamBlock = false;
                    int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 2, 5);
                    playSfx(rndFx);
                }
                planeActive = true;
                cam = World.CreateCamera(jets[0].Position, jets[0].Rotation, 80f);
                Function.Call(Hash.ATTACH_CAM_TO_ENTITY, cam, jets[0], 0f, -15f, 2f, true);
                cameraSet = true;
            }
            void unCallPlane(bool instant)
            {
                plane.ForwardSpeed = 250;
                if (isDone)
                {
                    if (instant || !plane.IsInRange(Game.Player.Character.Position, 3210f))
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
                    }
                }
            }
            void strike(Vector3 target, Ped pilot, List<Vehicle> jets)
            {
                Ped player = Game.Player.Character;
                    jets[0].ForwardSpeed = 250;
                    jets[1].ForwardSpeed = 251;
                if (jets[0].Position.DistanceTo(target) > 3210)
                    {
                        isDone = true;
                        unCallPlane(false);
                    }
                    if (plane.Position.DistanceTo(target) < 600)
                    {
                        if (resp) { owner = player; }
                        else if (!resp) { owner = pilot; }
                        i = 1;
                    if (jetAudio && j == 0)
                    {
                        j++;
                        spamBlock = false;
                        playSfx(1);
                        // int Fx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 0, 20);
                        //if (Fx % 2 == 0) { playSfx(0); }
                        // else if (Fx % 2 != 0) { playSfx(1); }

                    }
                    while (i <= 70 && !jets[0].IsDead && !isDone)
                        {
                        jets[0].ForwardSpeed = 250;
                        jets[1].ForwardSpeed = 250;
                        World.ShootBullet(jets[0].Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), owner, WeaponHash.Railgun, 100, -1);
                        World.ShootBullet(jets[1].Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), owner, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                            Wait(20);
                            i++;
                        if (i >= 70)
                        {
                            delayTime = Game.GameTime;
                            isDone = true;
                            jets[0].ForwardSpeed = 250;
                            jets[1].ForwardSpeed = 250;
                            foreach (Vehicle jet in jets)
                            {
                                Function.Call(Hash.TASK_PLANE_MISSION, jet.Driver, jet, 0, player, 0, 0, 0, 8, 0f, 0f, 0f, 0f, 0f);
                            }
                        }
                    }
                    }
                    else if ((jets[0].Position.DistanceTo(target) > 600) && jets[0].Position.DistanceTo(target) < 2500)
                    {
                        timer++;
                        Wait(0);
                        if (debug) { GTA.UI.Screen.ShowSubtitle("Emergency despawn timer: " + timer, 2000); }

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
            
        }
    }
