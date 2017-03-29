using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Effect {
        public enum Type { HIT_SPARK, BLOCK_SPARK }
        public enum State { NONE, LIGHT, MEDIUM, HEAVY }

        private Vector2 offset;
        private Vector2 scale;
        private Type effectType;
        private State effectState;
        private string asset;
        private string name;
        private int delay;
        private int alpha;
        

        public Effect(string name, string asset, Type effectType, State effectState, float sx, float sy, float x1 = 0, float y1 = 0, int delay = 2, int alpha = 255) {
            this.name = name;
            this.asset = asset;

            this.effectType = effectType;
            this.effectState = effectState;

            offset = new Vector2(x1, y1);
            scale = new Vector2(sx, sy);

            this.delay = delay;
            this.alpha = alpha;
        }

        public string GetName() {
            return name;
        }

        public string GetAsset() {
            return asset;
        }

        public Vector2 GetOffset() {
            return offset;
        }

        public Vector2 GetScale() {
            return scale;
        }

        public State GetState() {
            return effectState;
        }

        public Type GetEffectType() {
            return effectType;
        }

        public int GetDelay() {
            return delay;
        }

        public int GetAlpha() {
            return alpha;
        }
    }
}
