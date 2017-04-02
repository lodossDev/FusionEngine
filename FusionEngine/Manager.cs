using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Manager {
        protected List<Entity> entities;
        protected List<Player> players;
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

        public virtual void AddEntity(List<Entity> entities) {
            foreach (Entity entity in entities) {
                AddEntity(entity);
            }
        }

        public virtual void AddLevel(Level level) {
            levels.Add(level);
        }

        public virtual void RemoveEntity(Entity entity) {
            entities.Remove(entity);
        }

        public virtual void RemoveEntity(List<Entity> entities) {
            foreach (Entity entity in entities) {
                RemoveEntity(entity);
            }
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
