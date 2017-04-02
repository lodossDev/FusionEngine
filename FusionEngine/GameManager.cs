using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class GameManager {
        private InputManager inputManager;
        private CollisionManager collisionManager;
        private RenderManager renderManager;
        private static GameManager _instance;
        private static int playerIndex;


        private GameManager() {
            inputManager = new InputManager();
            collisionManager = new CollisionManager();
            renderManager = new RenderManager();
            playerIndex = 0;
        }

        public static GameManager GetInstance() {
            if (_instance == null) {
                _instance = new GameManager();
            }

            return _instance;
        }

        public void AddEntity(Entity entity) {
            collisionManager.AddEntity(entity);
            renderManager.AddEntity(entity);

            if (entity is Player) {
                inputManager.AddControl(entity, (PlayerIndex)playerIndex);
                playerIndex++;
            }
        }

        public void AddEntity(List<Entity> entities) {
            foreach(Entity entity in entities) { 
                AddEntity(entity);
            }
        }

        public void AddLevel(Level level) {
            renderManager.AddLevel(level);
            List<Entity> miscEntities = level.GetEntities();

            if (miscEntities != null) { 
                AddEntity(miscEntities);
            }
        }

        public void RemoveEntity(Entity entity) {
            inputManager.RemoveEntity(entity);
            collisionManager.RemoveEntity(entity);
            renderManager.RemoveEntity(entity);
        }

        public void RemoveEntity(List<Entity> entities) {
            foreach(Entity entity in entities) {
                RemoveEntity(entity);
            }
        }

        public InputManager GetInputManager() {
            return inputManager;
        }

        public CollisionManager GetCollisionManager() {
            return collisionManager;
        }

        public RenderManager GetRenderManager() {
            return renderManager;
        }

        public void Update(GameTime gameTime) {
            collisionManager.BeforeUpdate(gameTime);
            inputManager.Update(gameTime);
            collisionManager.AfterUpdate(gameTime);
            renderManager.Update(gameTime);
        }

        public void Render(GameTime gameTime) {
            renderManager.Draw(gameTime);
        }
    }
}
