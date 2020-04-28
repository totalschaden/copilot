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

        public Skill (string buffname = "", ushort id = 0)
        {
            this.buffName = buffname;
            this.id = id;
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
    } 
}
