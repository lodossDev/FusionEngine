using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Obstacle : Entity {

        public Obstacle(String name) : base(ObjectType.OBSTACLE, name) {
            SetDeathMode(DeathType.DEFAULT | DeathType.FLASH);
            SetDieTime(55);
            GetCollisionInfo().SetIsCollidable(true);
        }
    }
}
