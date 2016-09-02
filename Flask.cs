using External_Overlay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

public class Flask
{
    public Name name;
    public bool visible { get; set; }
    public bool usable { get; set; }
    public double baseDuration { get; set; }
    public int qual { get; set; }
    public Keys2 key { get; set; }
    public bool inUse { get; set; }
    public float useDuration { get; set; }
    public string flaskImageLocation { get; set; }
    public Flask(bool vis, Name _name, Keys2 _key, int _qual)
    {
        visible = vis;
        name = _name;
        key = _key;
        qual = _qual;
        flaskImageLocation = "FlaskImages\\" + name + ".png";
        if (name == Name.Quicksilver_Flask || name == Name.Ruby_Flask || name == Name.Saphire_Flask || name == Name.Topaz_Flask || name == Name.Diamond_Flask || name == Name.Granite_Flask || name == Name.Jade_Flask || name == Name.Jade_Flask || name == Name.Sulphur_Flask || name == Name.Lions_Roar || name == Name.Taste_of_Hate)
            baseDuration = 4;
        else if (name == Name.Bismuth_Flask || name == Name.Stibnite_Flask || name == Name.Silver_Flask || name == Name.Aquamarine_Flask || name == Name.Basalt_Flask)
            baseDuration = 5;
        else if (name == Name.Amethyst_Flask || name == Name.Atziris_Promise)
            baseDuration = 3.5;
        usable = true;
        useDuration = 0;
    }
    static void MakeUsable(Flask f)
    {
        f.usable = true;
    }
    public enum Name
    {
        Amethyst_Flask = 1,
        Aquamarine_Flask = 2,
        Atziris_Promise = 3,
        Basalt_Flask = 4,
        Bismuth_Flask = 5,
        Diamond_Flask = 6,
        Granite_Flask = 7,
        Jade_Flask = 8,
        Lions_Roar = 9,
        Quartz_Flask = 10,
        Quicksilver_Flask = 11,
        Ruby_Flask = 12,
        Saphire_Flask = 13,
        Silver_Flask = 14,
        Stibnite_Flask = 15,
        Sulphur_Flask = 16,
        Taste_of_Hate = 17,
        Topaz_Flask = 18
    }
}

