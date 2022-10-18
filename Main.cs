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
        bool planeActive = false;
        Vehicle plane;
        Ped pilot;
        Blip blip;
        int i;
        bool strikeReload;
        public Main()
        {
            Tick += onTick;
            KeyUp += onKeyUp;
            List<Vehicle> trash = new List<Vehicle>(World.GetAllVehicles("titan"));
            foreach (Vehicle lazer in trash)
            {
                lazer.Delete();
            }

            void onKeyUp(object sender, KeyEventArgs e)
            {
                if (planeActive && e.KeyCode == Keys.NumPad5)
                {
                    plane.EngineHealth = 0;
                    GTA.UI.Screen.ShowHelpText("Going down " + plane.EngineHealth, 1000, false, false);
                }
                if (e.KeyCode == Keys.U && !planeActive)
                {
                    callPlane();
                }
                else if (e.KeyCode == Keys.U && planeActive)
                {
                    unCall();
                }
            }
            void callPlane()
            {
                Ped player = Game.Player.Character;
                GTA.UI.Screen.ShowHelpText("Plane called...", 2000, true, false);
                plane = World.CreateVehicle("titan", player.Position + player.UpVector * 300 + player.ForwardVector * -1500, 0);
                pilot = plane.CreatePedOnSeat(VehicleSeat.Driver, PedHash.Blackops03SMY);
                plane.IsEngineRunning = true;
                plane.ForwardSpeed = 40;
                blip = plane.AddBlip();
                blip.Color = BlipColor.Blue;
                pilot.RelationshipGroup = player.RelationshipGroup;
                //Function.Call(Hash.TASK_PLANE_CHASE, pilot, player, 0f, 0f, 150f);
                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, Game.Player.Character, 0, 0, 0, 4, 300f, 0f, 0f, 300f, 400f);
                planeActive = true;
            }
            void unCall()
            {
                GTA.UI.Screen.ShowHelpText("Plane uncalled...", 2000, true, false);
                planeActive = false;
                strikeReload = false;
                blip.Delete();
                pilot.Delete();
                plane.Delete();
            }
            void strike(Vector3 target, Ped pilot, Vehicle plane)
            {
                Function.Call(Hash.TASK_PLANE_MISSION, pilot, plane, 0, 0, target.X, target.Y, target.Z, 4, 300f, 0f, 0f, 250f, 350f);
                if (!strikeReload && plane.Position.DistanceTo(target) < 2000)
                {
                    i = 1;
                    while (i <= 15 && !plane.IsDead)
                    {
                        World.ShootBullet(plane.Position, target.Around(13f), pilot, WeaponHash.HomingLauncher, 100, -1);
                        GTA.UI.Screen.ShowHelpText("~r~ROCKETS FIRED: " + i, 1000, false, false);
                        Wait(500);
                        i++;
                        if (i >= 15)
                        {
                            strikeReload = true;
                            reload();
                        }
                    }
                }
            }
            void reload()
            {
                if (strikeReload)
                {
                    i = 0;
                    while (i <= 500 && !plane.IsDead)
                    {
                        GTA.UI.Screen.ShowHelpText("~g~RELOADING " + i, 1000, false, false);
                        Wait(10);
                        i++;
                        if (i <= 500)
                        {
                            strikeReload = false;
                        }
                    }
                }
            }

            void onTick(object sender, EventArgs e)
            {
                if (planeActive)
                {
                    GTA.UI.Screen.ShowSubtitle("Distance: " + plane.Position.DistanceTo(Game.Player.Character.Position));
                }
                if (Function.Call<bool>(Hash.IS_EXPLOSION_IN_SPHERE, 22, Game.Player.Character.Position.X, Game.Player.Character.Position.Y, Game.Player.Character.Position.Z, 500f) && planeActive)
                {
                    OutputArgument projectPos = new OutputArgument();
                    if (!plane.IsDead)
                    {
                        if (Function.Call<bool>(Hash.GET_COORDS_OF_PROJECTILE_TYPE_WITHIN_DISTANCE, Game.Player.Character, WeaponHash.Flare, 500f, projectPos, true) && planeActive)
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