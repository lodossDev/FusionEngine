using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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
        private UpdateManager updateManager;
        private RenderManager renderManager;
        private Dictionary<string, SoundEffect> defaultSoundEffects;
        private bool pause;

        private static GraphicsDevice graphicsDevice;
        private static SpriteBatch spriteBatch;
        private static ContentManager contentManager;
        private static GameManager _instance;
        private static int playerIndex;
    
        public static readonly int DEATH_FLASH_TIME = 100000000;
        public static readonly int RESOLUTION_X = 1280;
        public static readonly int RESOLUTION_Y = 700;
        public static readonly float GAME_VELOCITY = 60;
        public static readonly SamplerState SAMPLER_STATE = SamplerState.PointClamp;


        private GameManager() {
            inputManager = new InputManager();
            collisionManager = new CollisionManager();
            updateManager = new UpdateManager();
            renderManager = new RenderManager();
            defaultSoundEffects = new Dictionary<string, SoundEffect>();
            pause = false;

            foreach (var item in SoundContent.LoadSounds("Sounds/")) {
                defaultSoundEffects.Add(item.Key, item.Value);
            }

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
            updateManager.AddEntity(entity);
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

        public void AddSpark(Entity entity) {
            updateManager.AddEntity(entity);
            renderManager.AddEntity(entity);
        }

        public void AddTrail(Entity entity) {
            renderManager.AddEntity(entity);
        }

        public void RemoveEntity(Entity entity) {
            inputManager.RemoveEntity(entity);
            collisionManager.RemoveEntity(entity);
            updateManager.RemoveEntity(entity);
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

        public UpdateManager GetUpdateManager() {
            return updateManager;
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
            updateManager.Update(gameTime);
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
                soundInstance.Play();
            }

            return soundInstance;
        }

        public SoundEffectInstance PlaySFX(Entity entity, Animation.State? state, String sfx, float volume = 1, float pitch = 0, float pan = 0, bool loop = false) {
            SoundEffect soundEffect = entity.GetAnimationSound(Animation.State.DIE1);

            if (soundEffect == null) {
                soundEffect = GameManager.GetInstance().GetSFX(sfx);
            }

            return PlaySFX(soundEffect, volume, pitch, pan, loop);
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

        public void CallPause() {
            pause = !pause;
        }

        public bool IsPause() {
            return pause;
        }

        public static GraphicsDevice GetGraphicsDevice() {
            return graphicsDevice;
        }

        public static SpriteBatch GetSpriteBatch() {
            return spriteBatch;
        }

        public static ContentManager GetContentManager() {
            return contentManager;
        }

        public static void SetupDevice(GraphicsDevice graphicsDevice, ContentManager contentManager) {
            GameManager.graphicsDevice = graphicsDevice;
            GameManager.contentManager = contentManager;

            spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public static void SetupDevice(GraphicsDevice graphicsDevice, ContentManager contentManager, SpriteBatch spriteBatch) {
            GameManager.graphicsDevice = graphicsDevice;
            GameManager.contentManager = contentManager;
            GameManager.spriteBatch = spriteBatch;
        }
    }
}
