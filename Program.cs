using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace External_Overlay
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
           /* Flask f3 = new Flask(true, Flask.Name.Atziris_Promise, Keys.D9, 0);
            Flask f4 = new Flask(true, Flask.Name.Atziris_Promise, Keys.D9, 0);
            Flask f = new Flask(true, Flask.Name.Atziris_Promise, Keys.D3, 0);
            List<Flask> Flasks = new List<Flask>();
            Flasks.Add(f3);
            Flasks.Add(f4);
            Flasks.Add(f);
            Flask f2 = new Flask(true, Flask.Name.Lions_Roar, Keys.D4, 0);
            Flasks.Add(f2);*/
            Application.Run(new Form2());
        }
    }
}
