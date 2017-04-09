using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class GameManager {
        private InputManager inputManager;
        private CollisionManager collisionManager;
        private RenderManager renderManager;
        private Dictionary<string, SoundEffect> defaultSoundEffects;

        private static GameManager _instance;
        private static int playerIndex;
        public static readonly int FLASH_TIME_DEATH_DEFAULT = 100000000;


        private GameManager() {
            inputManager = new InputManager();
            collisionManager = new CollisionManager();
            renderManager = new RenderManager();
            playerIndex = 0;

            defaultSoundEffects = new Dictionary<string, SoundEffect>();

            foreach (var item in SoundContent.LoadSounds("Sounds/")) {
                Debug.WriteLine("SFX: " + item.Key);
                defaultSoundEffects.Add(item.Key, item.Value);
            }
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

        public Entity GetEntity(Entity entity) {
            int i = GetRenderManager().GetEntities().IndexOf(entity);
            return GetRenderManager().GetEntities()[i];
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

        public void PlaySFX(string sfx) {
            if (defaultSoundEffects.ContainsKey(sfx)) { 
                defaultSoundEffects[sfx].CreateInstance().Play();
            }
        }

        public SoundEffectInstance PlaySFX(SoundEffect effect, float volume = 1, float pitch = 0, float pan = 0, bool loop = false) {
            SoundEffectInstance soundInstance = null;

            if (effect != null) { 
                soundInstance = effect.CreateInstance();
                soundInstance.Volume = volume;
                soundInstance.Pitch = pitch;
                soundInstance.Pan = pan;
                soundInstance.IsLooped = loop;
                soundInstance.Play();
            }

            return soundInstance;
        }

        public SoundEffectInstance PlaySFX(string sfx, float volume = 1, float pitch = 0, float pan = 0, bool loop = false) {
            SoundEffectInstance soundInstance = GetSoundInstance(sfx);

            if (soundInstance != null) {
                soundInstance.Volume = volume;
                soundInstance.Pitch = pitch;
                soundInstance.Pan = pan;
                soundInstance.IsLooped = loop;
            }

            return soundInstance;
        }

        public SoundEffectInstance PlaySFX(Entity entity, Animation.State state, String sfx) {
            SoundEffect soundEffect = entity.GetAnimationSound(Animation.State.DIE1);

            if (soundEffect == null) {
                soundEffect = GameManager.GetInstance().GetSFX(sfx);
            }

            return PlaySFX(soundEffect);
        }

        public SoundEffectInstance GetSoundInstance(string sfx) {
            if (defaultSoundEffects.ContainsKey(sfx)) { 
                SoundEffectInstance soundInstance = defaultSoundEffects[sfx].CreateInstance();
                return soundInstance;
            }

            return null;
        }

        public SoundEffect GetSFX(string sfx) {
            if (defaultSoundEffects.ContainsKey(sfx)) { 
                return defaultSoundEffects[sfx];
            }

            return null;
        }
    }
}
