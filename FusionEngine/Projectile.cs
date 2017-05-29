using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Projectile : Entity {
        private Entity owner;


        public Projectile(string name, Entity owner) : base(ObjectType.PROJECTILE, name) {
            SetOwner(owner);
        }

        public Entity GetOwner() {
            return owner;
        }

        public void SetOwner(Entity owner) {
            this.owner = owner;
        }
    }
}
