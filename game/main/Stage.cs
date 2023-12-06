using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisetsar
{
    public class Stage
    {
        public static Dictionary<long, Stage> Quest;

        public long Id;
        public string Text = "";

        public Variant[] Variants = new Variant[1] { new Variant() };

        public bool Win = false;
        public bool Death = false;
        public string DeathName = "";
    }

    public class Variant
    {
        public string Text = "Далее";
        public long SetStage = 1;
        public long Piety = 0;
    }
}
