using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Manager {
        public List<Entity> entities;
        public List<Player> players;
        protected List<Level> levels;


        public Manager() {
            entities = new List<Entity>();
            players = new List<Player>();
            levels = new List<Level>();
        }

        public virtual void AddEntity(Entity entity) {
            entities.Add(entity);

            if (entity is Player) {
                players.Add((Player)entity);
            }
        }

        public virtual void AddLevel(Level level) {
            levels.Add(level);
            List<Entity> misc = level.GetMisc();

            if (misc != null) { 
                entities.AddRange(misc);
            }
        }

        public virtual void AddEntity(List<Entity> entities) {
            entities.AddRange(entities);
        }

        public virtual void RemoveEntity(Entity entity) {
            entities.Remove(entity);
        }

        public List<Player> GetPlayers() {
            return players;
        }

        public List<Level> GetLevels() {
            return levels;
        }

        public List<Entity> GetEntities() {
            return entities;
        }
    }
}
