using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Life : Collectable {

         public Life(String name) : base(name) {
             points = 1;
         }
    }
}
