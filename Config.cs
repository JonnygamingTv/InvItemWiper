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
        public List<ushort> ItemID;
        public string ServerName;
        public string LevelName;

        public void LoadDefaults()
        {
            enabled = false;
            Events = true;
            ItemID = new List<ushort> { 47580 };
            ServerName = "3";
            LevelName = "Retrovia";
        }
    }
}
