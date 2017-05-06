using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {
    
    public class Health : Collectable {

         public Health(String name) : base(name) {
             SetPoints(50);
         }
    }
}
