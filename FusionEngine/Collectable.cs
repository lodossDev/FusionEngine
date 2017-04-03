using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Collectable : Entity {

        public Collectable(String name) : base(Entity.ObjectType.COLLECTABLE, name) {

        }
    }
}
