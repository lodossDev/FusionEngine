using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Effect {
        public enum Type { HIT_SPARK }
        public enum State { NONE, LIGHT, MEDIUM, HEAVY }

        private Vector2 offset;
        private Type effectType;
        private State effectState;
        

        public Effect(Type effectType, State effectState, float x1, float y1) {
            this.effectType = effectType;
            this.effectState = effectState;
            offset = new Vector2(x1, y1);
        }

        public Vector2 GetOffset() {
            return offset;
        }

        public State GetState() {
            return effectState;
        }

        public Type GetEffectType() {
            return effectType;
        }
    }
}
