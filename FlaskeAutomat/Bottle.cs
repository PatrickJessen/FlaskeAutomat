using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlaskeAutomat
{
    enum BottleType
    {
        ØL, Sodavand
    }
    class Bottle
    {
        // Which number the bottle has in the array (incase i needed it)
        public int Number { get; set; }
        // Type of bottle
        public BottleType Type { get; set; }

        public Bottle(int number, BottleType type)
        {
            this.Number = number;
            this.Type = type;
        }
    }
}
