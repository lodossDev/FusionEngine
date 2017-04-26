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
        private Dictionary<Effect.State, Effect> blockSparks;
        private Dictionary<Effect.State, Effect> hitSparks;
        private bool pause;
        
        private static Camera camera;
        private static Resolution resolution;
        private static GraphicsDevice graphicsDevice;
        private static SpriteBatch spriteBatch;
        private static ContentManager contentManager;
        private static GameManager _instance;
        private static int playerIndex;

        public static readonly int DEATH_FLASH_TIME = 100000000;
        public static readonly int RESOLUTION_X = 1280;
        public static readonly int RESOLUTION_Y = 720;
        public static readonly float GAME_VELOCITY = 60;
        public static readonly SamplerState SAMPLER_STATE = SamplerState.PointClamp;


        private GameManager() {
            inputManager = new InputManager();
            collisionManager = new CollisionManager();
            updateManager = new UpdateManager();
            renderManager = new RenderManager();
            defaultSoundEffects = new Dictionary<string, SoundEffect>();
            hitSparks = new Dictionary<Effect.State, Effect>();
            blockSparks = new Dictionary<Effect.State, Effect>();
            
            foreach (var item in SoundContent.LoadSounds("Sounds/")) {
                defaultSoundEffects.Add(item.Key, item.Value);
            }

            hitSparks.Add(Effect.State.LIGHT, new Effect("LIGHT_HIT_SPARK", "Sprites/Misc/Hitsparks/Hit1/STANCE", Effect.Type.HIT_SPARK, Effect.State.LIGHT, 1.8f, 1.5f));
            blockSparks.Add(Effect.State.LIGHT, new Effect("LIGHT_BLOCK_SPARK", "Sprites/Misc/Hitsparks/Block1/STANCE", Effect.Type.BLOCK_SPARK, Effect.State.LIGHT, 1.3f, 1.0f, 0, 50, 5, 180, true));

            playerIndex = 0;
            pause = false;
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

        public Effect GetHitSpark(Effect.State state) {
            return hitSparks[state];
        }

        public Effect GetBlockSpark(Effect.State state) {
            return blockSparks[state];
        }

        public Entity GetEntity(Entity entity) {
            int i = GetRenderManager().GetEntities().IndexOf(entity);

            if (i != -1) { 
                return GetRenderManager().GetEntities()[i];
            }

            return null;
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
            SoundEffect soundEffect = entity.GetAnimationSound(state);

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

        public void PauseGame() {
            pause = !pause;
        }

        public bool IsPause() {
            return pause;
        }

        public static Camera GetCamera() {
            return camera;
        }

        public static Resolution GetResolution() {
            return resolution;
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

        public static void SetupResolution(int width, int height) {
            resolution = new Resolution(width, height);
        }

        public static void UpdateResolution(GraphicsDeviceManager device) {
            resolution.Update(device);
        }

        public static void SetupCamera(Viewport viewPort, float vx = 0.8f, float vy = 0.8f) {
            camera = new Camera(viewPort);
            camera.Parallax = new Vector2(vx, vy);
        }

        public static void SetupCamera(float vx = 0.8f, float vy = 0.8f) {
            camera = new Camera(GameManager.graphicsDevice.Viewport);
            camera.Parallax = new Vector2(vx, vy);
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
