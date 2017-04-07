using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Collectable : Entity {
        protected int points;

        public Collectable(String name) : base(Entity.ObjectType.COLLECTABLE, name) {
            points = 0;
        }

        public int GetPoints() {
            return points;
        }

        public void SetPoints(int points) {
            this.points = points;
        }
    }
}
