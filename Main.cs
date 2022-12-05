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
        public Main()
        {
            Tick += onTick;
            KeyUp += onKeyUp;
            List<Vehicle> trash = new List<Vehicle>(World.GetAllVehicles("lazer"));
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
                if (e.KeyCode == Keys.NumPad5 && planeActive)
                {
                    plane.EngineHealth = 0;
                    GTA.UI.Screen.ShowHelpText("Going down " + plane.EngineHealth, 1000, false, false);
                }
                if (e.KeyCode == Keys.U)
                {
                    unCall();
                }
            }
            void callPlane()
            {
                Ped player = Game.Player.Character;
                for(int i = 0; i < 2; i++)
                {
                    GTA.UI.Screen.ShowHelpText("Plane called...", 2000, true, false);
                    plane = World.CreateVehicle("lazer", player.Position.Around(15f) + player.UpVector * 300 + player.ForwardVector * -2000, player.Heading);
                    pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                    plane.IsEngineRunning = true;
                    isDone = false;
                    plane.ForwardSpeed = 1000;
                    blip = plane.AddBlip();
                    blip.Color = BlipColor.Blue;
                    pilot.RelationshipGroup = player.RelationshipGroup;
                    //Function.Call(Hash.TASK_PLANE_CHASE, pilot, player, 0f, 0f, 150f);
                    Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, Game.Player.Character, 0, 0, 0, 4, 300f, 0f, 0f, 300f, 400f);
                    jets.Add(plane);
                    Wait(2);
                    }
               
                planeActive = true;
            }
            void unCall()
            {
                if (!plane.IsInRange(Game.Player.Character.Position, 1500f) && (isDone))
                {
                    GTA.UI.Screen.ShowHelpText("Plane uncalled...", 2000, true, false);
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
                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 300f, 0f, 0f, 250f, 350f);
                if (plane.Position.DistanceTo(target) < 500)
                {
                    i = 1;
                    while (i <= 50 && !plane.IsDead && !isDone)
                    {
                        World.ShootBullet(plane.Position, target.Around(7f), pilot, WeaponHash.GrenadeLauncher, 100, -1);
                        GTA.UI.Screen.ShowHelpText("~r~ROCKETS FIRED: " + i, 1000, false, false);
                        Wait(200);
                        i++;
                        if (i >= 50)
                        {
                            delayTime = Game.GameTime;
                            isDone = true;
                            unCall();
                        }
                    }
                }
            }
           

            void onTick(object sender, EventArgs e)
            {
                if (planeActive)
                {
                    if (isDone)
                    {
                        unCall();
                    }
                    GTA.UI.Screen.ShowSubtitle("Distance: " + plane.Position.DistanceTo(Game.Player.Character.Position));
                }
                if (Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, 22, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 500f))
                {
                    newTime = Game.GameTime;
                    timePass = newTime - delayTime;
                    if (!planeActive && timePass > 60000)
                    {
                        callPlane();
                    }
                    OutputArgument projectPos = new OutputArgument();
                    if (!plane.IsDead)
                    {
                        if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && planeActive && timePass > 60000)
                        {
                            Vector3 target = projectPos.GetResult<Vector3>();
                            strike(target, pilot, plane);
                        }
                    }
                }
            }
        }
    }
}