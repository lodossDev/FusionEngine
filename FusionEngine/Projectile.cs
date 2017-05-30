using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Projectile : Entity {

        public Projectile(string name, Entity owner) : base(ObjectType.PROJECTILE, name) {
            SetOwner(owner);
        }
    }
}
