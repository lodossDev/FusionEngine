using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class MP : Collectable {

         public MP(String name) : base(name) {
             SetPoints(35);
         }
    }
}
