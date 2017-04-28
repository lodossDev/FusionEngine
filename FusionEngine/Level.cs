using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public abstract class Level {
        private Dictionary<int, List<Entity>> layers;
        private List<Entity> entities;
        private Entity mainLayer;
        private String name;
        private int xmin, xmax, zmin, zmax;


        public Level(String name) {
            layers = new Dictionary<int, List<Entity>>();
            entities = new List<Entity>();

            this.name = name;
            xmin = xmax = zmin = zmax = 0;

            Load();
        }

        public abstract void Load();

        public void AddLayer(int z, Entity layer) {
            if (!layers.ContainsKey(z)) {
                layers.Add(z, new List<Entity>());
            }

            layers[z].Add(layer);
        }

        public void AddEntity(Entity entity) {
            entities.Add(entity);
        }

        public void SetName(string name) {
            this.name = name;
        }

        public void SetBoundries(int xmin, int xmax, int zmin, int zmax) {
            this.xmin = xmin;
            this.xmax = xmax;
            this.zmin = zmin;
            this.zmax = zmax;
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

         public List<Entity> GetEntities() {
            return entities;
         }

        public int GetXMin() {
            return xmin;
        }

        public int GetXMax() {
            return xmax;
        }
    }
}
