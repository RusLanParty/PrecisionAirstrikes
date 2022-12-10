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
        int i;
        int delayTime = 0;
        int newTime;
        int timePass;
        int timer=0;
        private WaveFileReader wave;
        private DirectSoundOut output;
        bool spamBlock;

        public Main()
        {
            bool debug = false;
            bool resp = Settings.GetValue<bool>("SETTINGS", "responsibility", true);
            int timeOut = Settings.GetValue<int>("SETTINGS", "timeOut", 30000);
            bool jetAudio = Settings.GetValue<bool>("SETTINGS", "jetAudio", true);
            bool radioAudio = Settings.GetValue<bool>("SETTINGS", "radioAudio", true);
            string model = Settings.GetValue<String>("SETTINGS", "model", "lazer");
            Tick += onTick;
            List<Vehicle> trash = new List<Vehicle>(World.GetAllVehicles("lazer"));
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
            void onTick(object sender, EventArgs e)
            {
                if (planeActive)
                {
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
                            strike(target, pilot, plane);
                        }
                    }
                }
            }
            void callPlane(Vector3 target)
            {
                Ped player = Game.Player.Character;
                for(int i = 0; i < 2; i++)
                {
                    if (debug) { GTA.UI.Screen.ShowHelpText("Jets are on the way...", 2000, false, false); }
                    plane = World.CreateVehicle(model, target.Around(15f) + player.UpVector * 300 + player.ForwardVector * -3000, player.Heading);
                    pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                    plane.IsEngineRunning = true;
                    isDone = false;
                    plane.ForwardSpeed = 300;
                    blip = plane.AddBlip();
                    blip.Color = BlipColor.Blue5;
                    pilot.RelationshipGroup = player.RelationshipGroup;
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, Game.Player.Character, 0, 0, 0, 4, 300f, 0f, 0f, 300f, 400f);
                    jets.Add(plane);
                    Wait(10);
                }
                if (radioAudio)
                {
                    int rndFx = Function.Call<int>(Hash.GET_RANDOM_INT_IN_RANGE, 1, 4);
                    playSfx(rndFx);
                }
                planeActive = true;
            }
            void unCallPlane(bool instant)
            {
                plane.ForwardSpeed = 250;
                if (isDone)
                {
                    if (instant || !plane.IsInRange(Game.Player.Character.Position, 3210f))
                    {
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
                    }
                }
            }
            void strike(Vector3 target, Ped pilot, Vehicle plane)
            {
                Ped player = Game.Player.Character;
                    plane.ForwardSpeed = 250;
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 300f, 0f, 0f, 250f, 350f);
                    if (plane.Position.DistanceTo(target) > 3210)
                    {
                    isDone = true;
                    timer = 0;
                    unCallPlane(false);
                    }
                   else if (plane.Position.DistanceTo(target) < 600)
                    {
                    spamBlock = false;
                    if (resp) {owner = player; }
                    else if(!resp) {owner = pilot; }
                        i = 1;
                    if (jetAudio)
                    {
                        playSfx(0);
                    }
                    while (i <= 15 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), owner, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                    }
                    Wait(550);
                    while (i <= 60 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 15f)), owner, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                    }
                    Wait(380);
                    while (i <= 90 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 10f)), owner, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~SHOTS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                        if (i >= 90)
                            {
                                delayTime = Game.GameTime;
                                isDone = true;
                            timer = 0;
                            }
                        }
                    }
                   else if((plane.Position.DistanceTo(target) > 600) && plane.Position.DistanceTo(target) < 2500){
                    timer++;
                    Wait(0);
                    if (debug) { GTA.UI.Screen.ShowSubtitle("Emergency despawn timer: " + timer, 2000); }
                    if (timer >= 1000)
                    {
                        isDone = true;
                        unCallPlane(true);
                        timer = 0;
                    }
                }
                
            }
            void playSfx(int fx)
            {
                if (!spamBlock)
                {
                    switch (fx)
                    {
                        case 0:
                            wave = new NAudio.Wave.WaveFileReader("scripts/brt/brt.wav");
                            output = new NAudio.Wave.DirectSoundOut();
                            output.Init(new NAudio.Wave.WaveChannel32(wave));
                            output.Play();
                            break;
                        case 1:
                            spamBlock = true;
                            wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike1.wav");
                            output = new NAudio.Wave.DirectSoundOut();
                            output.Init(new NAudio.Wave.WaveChannel32(wave));
                            output.Play();
                            break;
                        case 2:
                            spamBlock = true;
                            wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike2.wav");
                            output = new NAudio.Wave.DirectSoundOut();
                            output.Init(new NAudio.Wave.WaveChannel32(wave));
                            output.Play();
                            break;
                        case 3:
                            spamBlock = true;
                            wave = new NAudio.Wave.WaveFileReader("scripts/brt/radioStrike3.wav");
                            output = new NAudio.Wave.DirectSoundOut();
                            output.Init(new NAudio.Wave.WaveChannel32(wave));
                            output.Play();
                            break;
                        case 4:
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
                            GTA.UI.Screen.ShowHelpText("Disposed output", 1000, true, false);
                    }
                    }
                    if (wave != null)
                    {
                        wave.Dispose();
                        wave = null;
                        GTA.UI.Screen.ShowHelpText("Disposed wave", 1000, true, false);
                }   
            }
            
        }
    }
}