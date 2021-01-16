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
        private int cooldown;

        public Skill (string buffname = "", ushort id = 0, int cooldown = 0)
        {
            this.buffName = buffname;
            this.id = id;
            this.cooldown = cooldown;
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
        public int Cooldown
        {
            get { return Cooldown; }
            set { cooldown = value; }
        }
    } 
}
