using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Manager {
        protected List<Entity> entities;
        
        public Manager() {
            entities = new List<Entity>();
        }

        public virtual void AddEntity(Entity entity) {
            entities.Add(entity);
        }

        public virtual void AddEntity(List<Entity> entities) {
            foreach (Entity entity in entities) {
                AddEntity(entity);
            }
        }

        public virtual void RemoveEntity(Entity entity) {
            entities.Remove(entity);
        }

        public virtual void RemoveEntity(List<Entity> entities) {
            foreach (Entity entity in entities) {
                RemoveEntity(entity);
            }
        }

        public List<Entity> Entities {
            get { return entities; }
        }
    }
}
