using Airstrike;
using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrecisionAirstrike
{
    
    internal class BombTick : Script
    {
        public BombTick()
        {
            Tick += onTick;
        }
        void onTick(object sender, EventArgs e)
        {
            if (Main.bombsActive)
            {
                for (int i = 0; i < Main.bombs.Count; i++)
                {
                    //Vector3 offset2 = Main.RotationToDirection(Main.bombs[i].Rotation);
                    Vector3 vel = new Vector3(Main.bombs[i].Velocity.X , Main.bombs[i].Velocity.Y, Main.bombs[i].Velocity.Z - 1.0f);
                    Main.bombs[i].Velocity = vel;
                    Main.bombs[i].MaxSpeed = 300f;
                    if (Main.bombs[i].HeightAboveGround <= 70f)
                       {
                        World.AddExplosion(Main.bombs[i].Position, ExplosionType.ExplosiveAmmo, 1.0f, 0.0f, Main.owner, true, false);
                        Main.bombs[i].IsVisible = false;
                    for (int j = 0; j <= 4; j++)
                        {
                            World.ShootBullet(Main.bombs[i].Position.Around(1f), Main.bombs[i].Position + Game.Player.Character.UpVector * -5, Main.owner, WeaponHash.CompactGrenadeLauncher, 100, 205);
                           // Wait(5);
                        }
                        Main.bombs.RemoveAt(i);
                        if (Main.bombs.Count <= 0)
                        {
                            List<Prop> musor = new List<Prop>(World.GetAllProps("prop_ld_bomb_anim"));
                            foreach(Prop p in musor)
                            {
                                p.Delete();
                            }
                            Main.bombs.Clear();
                        }
                        }
                }
            }
        }
    }
}
