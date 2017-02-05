using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public abstract class Level {
        private Dictionary<int, List<Entity>> layers;
        private List<Entity> misc;
        private Entity mainLayer;
        private String name;
        private int maxLeft, maxRight;


        public Level(String name) {
            layers = new Dictionary<int, List<Entity>>();
            misc = new List<Entity>();

            this.name = name;
            maxLeft = maxRight = 0;

            Load();
        }

        public abstract void Load();

        public void AddLayer(int z, Entity layer) {
            if (!layers.ContainsKey(z)) {
                layers.Add(z, new List<Entity>());
            }

            layers[z].Add(layer);
        }

        public void AddMisc(Entity entity) {
            misc.Add(entity);
        }

        public void SetName(string name) {
            this.name = name;
        }

        public void SetMainLayer(Entity layer) {
            mainLayer = layer;
        }

        public void SetXScrollBoundry(int maxLeft, int maxRight) {
            this.maxLeft = maxLeft;
            this.maxRight = maxRight;
        }

        public virtual void ScrollX(float velX) {
            if (mainLayer != null) { 
                List<Entity> entities = this.layers.SelectMany(item => item.Value).ToList();
                entities.AddRange(misc);

                float x1 = mainLayer.GetPosX();

                if ((int)velX < 0 && (int)x1 <= maxRight 
                        || (int)velX > 0 && (int)x1 >= maxLeft) { 
                    
                    return;
                }

                foreach (Entity entity in entities) {
                    entity.MoveX(velX);
                }
            }
        }

        public void ScrollY(float velY) {
            if (mainLayer != null) {
                List<Entity> entities = this.layers.SelectMany(item => item.Value).ToList();
                entities.AddRange(misc);

                foreach (Entity entity in entities) {
                    entity.MoveY(velY);
                }
            }
        }

        public String GetName() {
            return name;
        }

        public List<Entity> GetLayers(int z) {
            if (layers.ContainsKey(z)) { 
                return layers[z];
            }

            return null;
        }

         public List<Entity> GetMisc() {
            return misc;
        }

        public int GetMaxLeft() {
            return maxLeft;
        }

        public int GetMaxRight() {
            return maxRight;
        }

        public Entity GetMainLayer() {
            return mainLayer;
        }
    }
}
