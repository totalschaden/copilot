using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoPilot
{
    class Skill
    {
        private string buffName;
        private ushort id;
        private float cooldown;
        private int lastUsed;

        public Skill (string buffname = "", ushort id = 0, float cooldown = 0, int lastUsed = 0)
        {
            this.buffName = buffname;
            this.id = id;
            this.cooldown = cooldown;
            this.lastUsed = lastUsed;
        }
        public string BuffName
        {
            get { return buffName;  }
            set { buffName = value; }
        }
        public ushort Id
        {
            get { return id;  }
            set { id = value; }
        }
        public float Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }
        public int LastUsed
        {
            get { return lastUsed; }
            set { lastUsed = value; }
        }
    } 
}
