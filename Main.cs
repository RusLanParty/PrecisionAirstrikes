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

namespace Airstrike
{
    public class Main : Script
    {
        List<Vehicle> jets = new List <Vehicle>();
        bool planeActive = false;
        bool isDone = false;
        Vehicle plane;
        Ped pilot;
        Blip blip;
        int i;
        int delayTime = 0;
        int newTime;
        int timePass;
        int timeOut =10000;
        int timer=0;
        
        public Main()
        {
            bool debug = true;
            Tick += onTick;
            List<Vehicle> trash = new List<Vehicle>(World.GetAllVehicles("lazer"));
            List<Ped> musor = new List<Ped>(World.GetAllPeds(PedHash.Blackops03SMY));
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
            void callPlane(Vector3 target)
            {
                Ped player = Game.Player.Character;
                for(int i = 0; i < 2; i++)
                {
                    if (debug) { GTA.UI.Screen.ShowHelpText("Plane called...", 2000, true, false); }
                    plane = World.CreateVehicle("lazer", target.Around(15f) + player.UpVector * 300 + player.ForwardVector * -2000, player.Heading);
                    pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                    plane.IsEngineRunning = true;
                    isDone = false;
                    plane.ForwardSpeed = 300;
                    blip = plane.AddBlip();
                    blip.Color = BlipColor.Blue;
                    pilot.RelationshipGroup = player.RelationshipGroup;
                    //Function.Call(Hash.TASK_PLANE_CHASE, pilot, player, 0f, 0f, 150f);
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, Game.Player.Character, 0, 0, 0, 4, 300f, 0f, 0f, 300f, 400f);
                    jets.Add(plane);
                    Wait(10);
                }
               
                planeActive = true;
            }
            void unCall()
            {
                plane.ForwardSpeed = 250;
                if (!plane.IsInRange(Game.Player.Character.Position, 1500f) && isDone)
                {
                    if (debug) { GTA.UI.Screen.ShowHelpText("Plane uncalled...", 2000, true, false); }
                    planeActive = false;
                    isDone = false;
                    foreach(Vehicle jet in jets)
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
            void strike(Vector3 target, Ped pilot, Vehicle plane)
            {
                Ped player = Game.Player.Character;
                    plane.ForwardSpeed = 250;
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 300f, 0f, 0f, 250f, 350f);
                    if (plane.Position.DistanceTo(target) > 2500)
                    {
                    isDone = true;
                    unCall();
                    }
                   else if (plane.Position.DistanceTo(target) < 600)
                    {
                        i = 1;
                    while (i <= 30 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), player, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~ROCKETS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                    }
                    Wait(470);
                    while (i <= 60 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), player, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~ROCKETS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                    }
                    Wait(330);
                    while (i <= 90 && !plane.IsDead && !isDone)
                    {
                        plane.ForwardSpeed = 250;
                        World.ShootBullet(plane.Position, target.Around(Function.Call<float>(Hash.GET_RANDOM_FLOAT_IN_RANGE, 0f, 20f)), player, WeaponHash.Railgun, 100, -1);
                        if (debug) { GTA.UI.Screen.ShowHelpText("~r~ROCKETS FIRED: " + i, 1000, false, false); }
                        Wait(20);
                        i++;
                        if (i >= 90)
                            {
                                delayTime = Game.GameTime;
                                isDone = true;
                            }
                        }
                    }
                   else {
                    timer++;
                if(timer >= 10000)
                    {
                        isDone = true;
                        unCall();
                    }
                }
                
            }
            void onTick(object sender, EventArgs e)
            {
                if (planeActive)
                {
                    if(!plane.IsInRange(Game.Player.Character.Position, 2200f))
                    {
                        isDone=true;
                        unCall();
                    }
                    if (isDone)
                    {
                        unCall();
                    }
                    if (debug) { GTA.UI.Screen.ShowSubtitle("Distance: " + plane.Position.DistanceTo(Game.Player.Character.Position)); }
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
        }
    }
}