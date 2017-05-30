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
        private Dictionary<string, Level> levels;
        private List<Entity> entities;
        private List<Player> players;
        private Level currentLevel;
        public float slowTime;
       
        private static Camera camera;
        private static Resolution resolution;
        private static GraphicsDevice graphicsDevice;
        private static GraphicsDeviceManager graphicsDeviceManager;
        private static SpriteBatch spriteBatch;
        private static ContentManager contentManager;
        private static GameManager _instance;
        private static FrameRateCounter frameRate;

        private static bool pause = false;
        private static int playerIndex = 0;

        public static readonly int DEATH_FLASH_TIME = 100000000;
        public static readonly int RESOLUTION_X = 1280;
        public static readonly int RESOLUTION_Y = 800;
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

            entities = new List<Entity>();
            levels = new Dictionary<string, Level>();
            players = new List<Player>();
            
            foreach (var item in SoundContent.LoadSounds("Sounds/")) {
                defaultSoundEffects.Add(item.Key, item.Value);
            }

            hitSparks.Add(Effect.State.LIGHT, new Effect("HIT_SPARK_LIGHT", "Sprites/Misc/Hitsparks/Hit1/STANCE", Effect.Type.HIT_SPARK, Effect.State.LIGHT, 1.8f, 1.5f));
            blockSparks.Add(Effect.State.LIGHT, new Effect("BLOCK_SPARK_LIGHT", "Sprites/Misc/Hitsparks/Block1/STANCE", Effect.Type.BLOCK_SPARK, Effect.State.LIGHT, 1.3f, 1.0f, 0, 50, 5, 180, true));
            slowTime = 0f;
        }

        public static GameManager GetInstance() {
            if (_instance == null) {
                _instance = new GameManager();
            }

            return _instance;
        }

        public void AddEntity(Entity entity) {
            if (entity != null) { 
                updateManager.AddEntity(entity);
                collisionManager.AddEntity(entity);
                renderManager.AddEntity(entity);

                if (entity is Player) {
                    AddPlayer((Player)entity);
                    inputManager.AddControl(entity, (PlayerIndex)playerIndex);

                    playerIndex++;
                }

                entities.Add(entity);
            }
        }

        public void AddEntity(List<Entity> entities) {
            if (entities != null && entities.Count > 0) { 
                foreach (Entity entity in entities) { 
                    AddEntity(entity);
                }
            }
        }

        private void AddPlayer(Player player) {
            players.Add(player);
        }

        public void SetLevel(Level level) {
            //First check if level exists then unload whole level then reload etc..
            if (levels.ContainsKey(level.GetName())) {
                Level existing = levels[level.GetName()];

                RemoveEntity(level.Layers);
                RemoveEntity(level.Enemies.Cast<Entity>().ToList());
                RemoveEntity(level.Bosses.Cast<Entity>().ToList());
                RemoveEntity(level.Obstacles.Cast<Entity>().ToList());
                RemoveEntity(level.Collectables.Cast<Entity>().ToList());
                RemoveEntity(level.Walls.Cast<Entity>().ToList());

                levels.Remove(level.GetName());
                existing = null;
            }

            levels.Add(level.GetName(), level);
            AddLayers(level.Layers);
            AddWalls(level.Walls);
            AddEntity(level.Enemies.Cast<Entity>().ToList());
            AddEntity(level.Bosses.Cast<Entity>().ToList());
            AddEntity(level.Obstacles.Cast<Entity>().ToList());
            AddEntity(level.Collectables.Cast<Entity>().ToList());

            currentLevel = level;
        }

        public void AddLayers(List<Entity> layers) {
            foreach (Entity layer in layers) {
                Render(layer);
            }
        }

        public void AddWalls(List<Wall> walls) {
            foreach (Entity wall in walls) {
                entities.Add(wall);
                collisionManager.AddEntity(wall);
                updateManager.AddEntity(wall);
                renderManager.AddEntity(wall);
            }
        }

        public void Render(Entity entity) {
            entities.Add(entity);
            updateManager.AddEntity(entity);
            renderManager.AddEntity(entity);
        }

        public void RemoveEntity(Entity entity) {
            inputManager.RemoveEntity(entity);
            collisionManager.RemoveEntity(entity);
            updateManager.RemoveEntity(entity);
            renderManager.RemoveEntity(entity);
            entities.Remove(entity);
        }

        public void RemoveEntity(List<Entity> entities) {
            foreach (Entity entity in entities) {
                RemoveEntity(entity);
            }
        }

        public void DropAllEnemies() {
            foreach (Entity enemy in entities.Where(entity => entity is Enemy).ToList()) {
                float dir = (enemy.IsLeft() ? 1 : -1);
                enemy.SetAnimationState(Animation.State.KNOCKED_DOWN1);
                enemy.Toss(-19, (3 * dir), 2); 
                enemy.SetTossGravity(0.6f);
            }
        }

        public InputManager InputManager {
            get { return inputManager; }
        }

        public CollisionManager CollisionManager {
            get { return collisionManager; }
        }

        public UpdateManager UpdateManager {
            get { return updateManager; }
        }

        public RenderManager RenderManager {
            get { return renderManager; }
        }

        public Player GetPlayer(int index) {
            return players[index];
        }

        public List<Player> Players {
            get { return players; }
        }

        public Entity GetEntity(Entity entity) {
            int i = entities.IndexOf(entity);

            if (i != -1) { 
                return entities[i];
            }

            return null;
        }

        public List<Entity> GetEntities() {
            return entities;
        }

        public Effect GetHitSpark(Effect.State state) {
            if (hitSparks.ContainsKey(state)) {
                return hitSparks[state];
            }

            return null;
        }

        public Effect GetBlockSpark(Effect.State state) {
            if (blockSparks.ContainsKey(state)) {
                return blockSparks[state];
            }

            return null;
        }

        public Level CurrentLevel {
            get { return currentLevel; }
        }

        public void Update(GameTime gameTime) {
            slowTime--;

            if (slowTime < 0) {
                slowTime = 0;
                //return;
            }

            if (slowTime <= 0)updateManager.BeforeUpdate(gameTime);
            if (slowTime <= 0)collisionManager.BeforeUpdate(gameTime);

            inputManager.Update(gameTime);

            if (slowTime <= 0)collisionManager.AfterUpdate(gameTime);
            if (slowTime <= 0)updateManager.AfterUpdate(gameTime);
        }

        public void UpdatePosition(GameTime gameTime) {
            if (slowTime <= 0)renderManager.Update(gameTime);
        }

        public void Render(GameTime gameTime) {
            GameManager.UpdateFrameRate(gameTime);
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

        public static void PauseGame() {
            pause = !pause;
        }

        public static bool IsPause() {
            return pause;
        }

        public static Camera Camera {
            get { return camera; }
        }

        public static Resolution Resolution {
            get { return resolution; }
        }

        public static GraphicsDevice GraphicsDevice {
            get {return graphicsDevice;}
        }

        public static SpriteBatch SpriteBatch {
            get { return spriteBatch; }
        }

        public static ContentManager ContentManager {
            get { return contentManager; }
        }

        public static FrameRateCounter FrameRate {
            get { 
                if (frameRate == null) {
                    frameRate = new FrameRateCounter();
                }

                return frameRate;
            }
        }

        private static void UpdateFrameRate(GameTime gameTime) {
            FrameRate.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        private static void UpdateResolution() {
            resolution.Update(GameManager.graphicsDeviceManager);
        }

        public static void SetupCamera(Viewport viewPort, float vx = 0.8f, float vy = 0.3f) {
            camera = new Camera(viewPort);
            camera.Parallax = new Vector2(vx, vy);
        }

        public static void SetupCamera(float vx = 0.8f, float vy = 0.3f) {
            camera = new Camera(GameManager.graphicsDevice.Viewport);
            camera.Parallax = new Vector2(vx, vy);
        }

        public static void SetupDevice(GraphicsDeviceManager deviceManager, ContentManager contentManager) {
            GameManager.graphicsDeviceManager = deviceManager;
            GameManager.graphicsDevice = deviceManager.GraphicsDevice;
            GameManager.contentManager = contentManager;

            spriteBatch = new SpriteBatch(deviceManager.GraphicsDevice);
            resolution = new Resolution(GameManager.RESOLUTION_X, GameManager.RESOLUTION_Y);
            ChangeResolution(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight);
        }

        public static void SetupDevice(GraphicsDeviceManager deviceManager, ContentManager contentManager, SpriteBatch spriteBatch) {
            GameManager.graphicsDeviceManager = deviceManager;
            GameManager.graphicsDevice = deviceManager.GraphicsDevice;
            GameManager.contentManager = contentManager;
            GameManager.spriteBatch = spriteBatch;

            resolution = new Resolution(GameManager.RESOLUTION_X, GameManager.RESOLUTION_Y);
            ChangeResolution(deviceManager.PreferredBackBufferWidth, deviceManager.PreferredBackBufferHeight);
        }

        public static void ChangeResolution(int width, int height) {
            GameManager.graphicsDeviceManager.PreferredBackBufferWidth = width;
            GameManager.graphicsDeviceManager.PreferredBackBufferHeight = height;
            UpdateResolution();
        }

        public static void TakeScreenshot(IGameScreen game) {
            var screenshot = TextureContent.TakeScreenshot(game);

            using (var fs = new System.IO.FileStream(@"screenshot.png", System.IO.FileMode.OpenOrCreate)) {
                screenshot.Save(System.Drawing.Imaging.ImageFormat.Png, fs);
            }

            screenshot.Dispose();
        }

        public static Entity CreateEffect(Effect effect, Entity entity, float x1, float y1) {
            return CreateEffect(effect, entity, entity, x1, y1);
        }

        public static Entity CreateEffect(Effect effect, Entity entity, Entity target, float x1, float y1) {
            Entity spark = new Entity(Entity.ObjectType.HIT_FLASH, effect.GetName());
            spark.AddSprite(Animation.State.STANCE, new Sprite(effect.GetAsset(), Animation.Type.ONCE));
            spark.SetAnimationState(Animation.State.STANCE);
            spark.SetFrameDelay(Animation.State.STANCE, effect.GetDelay());
            spark.SetOffset(Animation.State.STANCE, effect.GetOffset().X, effect.GetOffset().Y);
            spark.SetScale(effect.GetScale().X, effect.GetScale().Y);

            spark.SetPostion(x1, y1, entity.GetPosZ() + 5);
            spark.SetLayerPos(target.GetDepthBox().GetRect().Bottom + 15);
            spark.SetFade(effect.GetAlpha());
            spark.SetBlendState(BlendState.Additive);

            if (effect.IsLeft()) { 
                if (entity.GetDirX() > 0) {
                    spark.SetIsLeft(true);
                } else {
                    spark.SetIsLeft(false);
                }
            }

            return spark;
        }

        public static void AddProjectile(Projectile projectile, float x1, float y1) {
            projectile.SetFade(200);
            projectile.SetBlendState(BlendState.Additive);
            projectile.SetIsLeft(projectile.GetOwner().IsLeft());
            projectile.SetLayerPos(10);

            if (projectile.IsLeft()) {
                projectile.SetPostion(projectile.GetOwner().GetPosX() - x1, projectile.GetOwner().GetPosY() + y1, projectile.GetOwner().GetPosZ());
                projectile.MoveX(6, -1);
            } else {
                projectile.SetPostion(projectile.GetOwner().GetPosX() + x1, projectile.GetOwner().GetPosY() + y1, projectile.GetOwner().GetPosZ());
                projectile.MoveX(6, 1);
            }

            projectile.UpdateBoxes(null);

            GameManager.GetInstance().AddEntity(projectile);
        }

        private static Effect GetSparkState(Entity entity, Effect.Type effectType, Effect.State effectState) {
            Effect spark = null;

            if (effectType == Effect.Type.HIT_SPARK) { 
                spark = entity.GetHitSpark(effectState);

            } else if (effectType == Effect.Type.BLOCK_SPARK) {
                spark = entity.GetBlockSpark(effectState);
            } 

            if (spark == null) {
                if (effectType == Effect.Type.HIT_SPARK) { 
                    spark = GameManager.GetInstance().GetHitSpark(effectState);

                } else if (effectType == Effect.Type.BLOCK_SPARK) {
                    spark = GameManager.GetInstance().GetBlockSpark(effectState);
                }
            }

            return spark;
        }

        public static Effect GetSpark(Entity entity, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect spark = null;
            Effect.State effectState;

            switch(state) {
                case CLNS.AttackBox.AttackType.LIGHT: 
                    effectState = Effect.State.LIGHT;
                break;

                case CLNS.AttackBox.AttackType.MEDIUM: 
                    effectState = Effect.State.MEDIUM;
                break;

                case CLNS.AttackBox.AttackType.HEAVY: 
                    effectState = Effect.State.HEAVY;
                break;

                default:
                    effectState = Effect.State.LIGHT;
                break;
            }

            spark = GetSparkState(entity, effectType, effectState);

            if (spark == null) {
               spark = GetSparkState(entity, effectType, Effect.State.LIGHT);
            }

            return spark;
        }

        public static void AddSpark(Entity entity, Entity target, CLNS.BoundingBox box, CLNS.AttackBox.AttackType state, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, state, effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, box);
                float y1 = HitBodyY(target, entity, box);

                Entity spark = CreateEffect(sparkInfo, entity, target, x1, y1);
                GameManager.GetInstance().Render(spark);
            }
        }

         public static void AddSpark(Entity entity, Entity target, CLNS.AttackBox attackBox, Effect.Type effectType) {
            Effect sparkInfo = GetSpark(entity, attackBox.GetAttackType(), effectType);

            if (sparkInfo != null) { 
                float x1 = HitBodyX(target, entity, attackBox);
                float y1 = HitBodyY(target, entity, attackBox);

                Entity spark = CreateEffect(sparkInfo, entity, target, x1, y1);
                GameManager.GetInstance().Render(spark);
            }
        }

        private static float HitBodyX(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);

            if (entity.IsLeft()) {
                x1 -= attackBox.GetOffset().X + attackBox.GetSparkOffset().X;
            } else {
                x1 += attackBox.GetOffset().X + attackBox.GetSparkOffset().X;
            }

            return x1;
        }

        private static float HitBodyX(Entity target, Entity entity, CLNS.BoundingBox box) {
            float x1 = ((target.GetPosX() + entity.GetPosX()) / 2);
            return x1;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.AttackBox attackBox) {
            return (int)-attackBox.GetRect().Height + (int)Math.Round(attackBox.GetOffset().Y + entity.GetPosY()) + attackBox.GetSparkOffset().Y;
        }

        private static float HitBodyY(Entity target, Entity entity, CLNS.BoundingBox box) {
            return ((int)Math.Round(box.GetOffset().Y + entity.GetPosY())) / 2;
        }
    }
}
