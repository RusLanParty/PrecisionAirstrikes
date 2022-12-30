using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Airstrike;
using GTA;
using GTA.Native;

namespace PrecisionAirstrike
{
    internal class VariousTickEvents: Script
    {
        public VariousTickEvents()
        {
            Tick += onTick;
        }
        void onTick(object sender, EventArgs e)
        {
            if (Main.poolLoaded)
            {
                Main.pool.Process();
            }
            
            if (Main.hudDis)
            {
                Function.Call(Hash.DISPLAY_RADAR, false);
            }
            else if (!Main.hudDis)
            {
                Function.Call(Hash.DISPLAY_RADAR, true);
            }
          
        }
    }
}
