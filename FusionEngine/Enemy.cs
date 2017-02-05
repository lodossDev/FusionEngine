using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine
{
    public class Enemy : Character
    {
        public Enemy(String name) : base(ObjectType.ENEMY, name) {
        }
    }
}
