using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PoEFlasks
{
    class Flask
    {
        public string name { get; set; }
        public bool visible { get; set; }
        public bool usable { get; set; }
        public Keys key { get; set; }
        public Flask(string _name, Keys _key)
        {
            name = _name;
            key = _key;
            usable = true;
            visible = true;
        }
        static void MakeUsable(Flask f)
        {
            f.usable = true;
        }
    }
}
