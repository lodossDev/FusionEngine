using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Money : Collectable {

         public Money(String name) : base(name) {
             points = 500;
         }
    }
}
