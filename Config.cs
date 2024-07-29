using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvItemWiper
{
    public class Config : IRocketPluginConfiguration
    {
        public bool enabled;
        public ushort ItemID;

        public void LoadDefaults()
        {
            enabled = true;
            ItemID = 47580;
        }
    }
}
