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
        public bool Events;
        public ushort ItemID;
        public string ServerName;
        public string LevelName;

        public void LoadDefaults()
        {
            enabled = true;
            Events = true;
            ItemID = 47580;
            ServerName = "3";
            LevelName = "Retrovia";
        }
    }
}
