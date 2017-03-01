using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TexturePackerLoader;

namespace FusionEngine {

    public class Sprite {
        private int currentFrame;      
        private List<Texture2D> sprites;
        private List<float> frameDelays;
        private float frameTimeElapsed;
        private float totalTimeRemaining;
        private List<bool> isFrameComplete;

        private List<Vector2> offsets;
        public List<Vector2> frameScales;
        private Vector2 spriteOffset;
        private Vector2 position;
        
        private int resetFrame;
        private Dictionary<int, List<CLNS.BoundingBox>> boxes;
        private Animation.Type animationType;
        private SpriteEffects effects;
        private bool isAnimationComplete;

        private SpriteSheet spriteSheet;
        private List<string> frames;
        

        public Sprite(Animation.Type animationType = Animation.Type.REPEAT) {
            currentFrame = 0;
            sprites = new List<Texture2D>();
            frameDelays = new List<float>();
            offsets = new List<Vector2>();
            frameScales = new List<Vector2>();
            frameTimeElapsed = 0.0f;
            totalTimeRemaining = 0.0f;
            resetFrame = 0;
            boxes = new Dictionary<int, List<CLNS.BoundingBox>>();
            spriteOffset = Vector2.Zero;
            position = Vector2.Zero;
            this.animationType = animationType;
            effects = SpriteEffects.None;
            isAnimationComplete = false;
            isFrameComplete = new List<bool>();
            frames = new List<string>();
        }

        public Sprite(string contentFolder, Animation.Type animationType = Animation.Type.REPEAT, int resetFrame = 1) : this(animationType) {
            AddTextures(contentFolder);
            SetResetFrame(resetFrame);
        }

        public Sprite(Texture2D sprite, Animation.Type animationType = Animation.Type.REPEAT, int resetFrame = 1) : this(animationType) {
            AddTexture(sprite);
            SetResetFrame(resetFrame);
        }

        public Sprite(List<Texture2D> sprites, Animation.Type animationType = Animation.Type.REPEAT, int resetFrame = 1) : this(animationType) {
            AddTextures(sprites);
            SetResetFrame(resetFrame);
        }

        public Sprite(String content, List<String> frames, Animation.Type animationType = Animation.Type.REPEAT, int resetFrame = 1) : this(animationType) {
            var spriteSheetLoader = new SpriteSheetLoader(System.contentManager);
            spriteSheet = spriteSheetLoader.Load(content);

            foreach (String frame in frames) {
                AddTexture(frame);
            }
        }

        public Sprite(SpriteSheet sheet, List<String> frames, Animation.Type animationType = Animation.Type.REPEAT, int resetFrame = 1) : this(animationType) {
            spriteSheet = sheet;

            foreach (String frame in frames) {
                AddTexture(frame);
            }
        }

        public Sprite Clone(int index) {
            Sprite clone = new Sprite(Animation.Type.NONE);
            Texture2D source = this.sprites[index - 1];
            Texture2D target = new Texture2D(System.graphicsDevice, source.Width, source.Height);

            Color[] bits = new Color[source.Width * source.Height];
            source.GetData(bits);
            target.SetData(bits);

            clone.AddTexture(target);
            clone.SetFrameOffset(this.offsets[index - 1].X, this.offsets[index - 1].Y);
            clone.SetFrameScale(this.frameScales[index - 1].X, this.frameScales[index - 1].Y);
            clone.SetCurrentFrame(1);

            return clone;
        }

        public void AddTextures(string contentFolder) {
            foreach(Texture2D texture in TextureContent.LoadTextures(contentFolder)) {
                AddTexture(texture);
            }
        }

        public void AddTextures(List<Texture2D> sprites) {
            foreach (Texture2D texture in sprites) {
                AddTexture(texture);
            }
        }

        public void AddTexture(Texture2D sprite) {
            sprites.Add(sprite);
            offsets.Add(Vector2.Zero);
            frameScales.Add(Vector2.Zero);

            isFrameComplete.Add(false);
            frameDelays.Add(Animation.DEFAULT_TICKS);
            frameTimeElapsed = Animation.DEFAULT_TICKS;
        }

        public void AddTexture(String frame) {
            frames.Add(frame);
            offsets.Add(Vector2.Zero);
            frameScales.Add(Vector2.Zero);

            isFrameComplete.Add(false);
            frameDelays.Add(Animation.DEFAULT_TICKS);
            frameTimeElapsed = Animation.DEFAULT_TICKS;
        }

        public void AddBox(int frame, CLNS.BoundingBox box) {
            if (!boxes.ContainsKey(frame - 1)) {
                boxes.Add(frame - 1, new List<CLNS.BoundingBox>());
            }

            boxes[frame - 1].Add(box);
            boxes[frame - 1][boxes[frame - 1].Count - 1].SetFrame(frame);
        }

        public void SetFrameTime(int frame, float frameDelay) {
            if (frameDelay == 0) {
                frameDelays[frame - 1] = 0f;

                if ((frame - 1) == 0) {
                    frameTimeElapsed = 0f;
                }
            } else {
                frameDelays[frame - 1] = Animation.TICK_RATE * frameDelay;

                if ((frame - 1) == 0) {
                    frameTimeElapsed = frameDelays[frame - 1] = Animation.TICK_RATE * frameDelay;
                }
            }

            totalTimeRemaining = GetTotalAnimationTime();
        }

        public void SetFrameTime(float frameDelay) {
            for(int i = 0; i < frameDelays.Count; i++) {
                SetFrameTime(i + 1, frameDelay);
            }
        }

        public void SetFrameScale(int frame, float x, float y) {
            frameScales[frame - 1] = new Vector2(x, y);
        }

        public void SetFrameScale(float x, float y) {
            for (int i = 0; i < offsets.Count; i++) {
                SetFrameScale(i + 1, x, y);
            }
        }

        public void SetFrameOffset(int frame, float x, float y) {
            offsets[frame - 1] = new Vector2(x, y);
        }

        public void SetFrameOffset(float x, float y) {
            for (int i = 0; i < offsets.Count; i++) {
                SetFrameOffset(i + 1, x, y);
            }
        }
        
        public void SetSpriteOffset(float x, float y) {
            spriteOffset.X = x;
            spriteOffset.Y = y;
        }

        public void SetResetFrame(int frame) {
            this.resetFrame = frame - 1;
        }

        public void SetAnimationType(Animation.Type animationType) {
            this.animationType = animationType;
        }

        public void SetIsLeft(bool isLeft) {
            if (isLeft) {
                effects = SpriteEffects.FlipHorizontally;
            } else {
                effects = SpriteEffects.None;
            }
        }

        public void SetSpriteSheet(SpriteSheet sheet) {
            this.spriteSheet = sheet;
        }

        public SpriteSheet GetSpriteSheet() {
            return spriteSheet;
        }

        public List<String> GetSpriteFrames() {
            return frames;
        }

        public Animation.Type GetAnimationType() {
            return animationType;
        }

        public SpriteEffects GetEffects() {
            return effects;
        }

        public String GetCurrentSpriteFrame() {
            return frames[currentFrame];
        }

        public Texture2D GetCurrentTexture() {
            return sprites[currentFrame];
        }

        public List<Texture2D> GetTextures() {
            return sprites;
        }

        public int GetSpriteFramesCount() {
            return frames.Count;
        }

        public int GetFrames() {
            return sprites.Count;
        }

        public Vector2 GetCurrentFrameOffSet() {
            return offsets[currentFrame];
        }

        public Vector2 GetSpriteOffSet() {
            return spriteOffset;
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes() {
            return (boxes.ContainsKey(currentFrame) ? boxes[currentFrame] : new List<CLNS.BoundingBox>());
        }

        public List<CLNS.BoundingBox> GetCurrentBoxes(CLNS.BoxType boxType) {
            return (boxes.ContainsKey(currentFrame) ? boxes[currentFrame].FindAll(item => item.GetBoxType() == boxType) : new List<CLNS.BoundingBox>());
        }

        public List<CLNS.BoundingBox> GetAllBoxes() {
            return boxes.SelectMany(item => item.Value).ToList();
        }

        public List<CLNS.BoundingBox> GetAllBoxes(CLNS.BoxType boxType) {
            return boxes.SelectMany(item => item.Value).ToList().FindAll(item => item.GetBoxType() == boxType);
        }

        public List<CLNS.BoundingBox> GetBoxes(int frame) {
            return boxes[frame - 1];
        }

        public Vector2 GetPosition() {
            return position;
        }

        public Vector2 GetCurrentScaleFrame() {
            return frameScales[currentFrame];
        }

        public bool IsBoxFrame() {
            return boxes.ContainsKey(currentFrame);
        }

        public bool IsAnimationComplete() {
            return isAnimationComplete;
        }

        public bool IsFrameComplete() {
            int frame = currentFrame - 1;
            if (frame < 0) frame = 0;

            return isFrameComplete[frame];
        }

        public bool IsFrameComplete(int frame) {
            int index = frame - 1;
            if (index < 0) index = 0;

            return isFrameComplete[index];
        }

        public bool IsLeft() {
            return (effects == SpriteEffects.FlipHorizontally);
        }

        public int GetCurrentFrame() {
            return currentFrame;
        }

        public float GetTotalAnimationTime() {
            return frameDelays.Sum(item => item);
        }

        public List<float> GetFrameDelays() {
            return frameDelays;
        }

        public float GetCurrentFrameDelay() {
            return frameDelays[currentFrame];
        }

        public float GetTotalRemainingTime() {
            return totalTimeRemaining;
        }

        public void ResetFrames() {
            isAnimationComplete = false;

            for (int i = 0; i < isFrameComplete.Count; i ++) {
                isFrameComplete[i] = false;
            }

            currentFrame = 0;
        }

        public void ResetAnimation() {
            ResetFrames();
            frameTimeElapsed = (float)frameDelays[currentFrame];
            totalTimeRemaining = GetTotalAnimationTime();
        }

        private void OnFrameComplete(){
            isFrameComplete[currentFrame] = true;

            if (animationType == Animation.Type.REPEAT) {
                currentFrame = (resetFrame != 0 ? resetFrame : 0);
            } else {
                currentFrame = (frames.Count > 0 ? frames.Count - 1 : sprites.Count - 1);
            }

            isAnimationComplete = true;
        }

        public void IncrementFrame(){
            if (currentFrame >= (frames.Count > 0 ? frames.Count - 1 : sprites.Count - 1)) {
                currentFrame = 0;
            } else {
                currentFrame++;
            }
        }

        private void NextFrame() {
            isFrameComplete[currentFrame] = true;
            currentFrame++;
        }

        public void SetCurrentFrame(int frame) {
            currentFrame = frame - 1;
        }

        public void UpdateAnimation(GameTime gameTime) {
            isAnimationComplete = false;

            if (sprites.Count > 0 && (animationType != Animation.Type.NONE)) {
                frameTimeElapsed -= (float)(gameTime.ElapsedGameTime.TotalSeconds);
                totalTimeRemaining -= (float)(gameTime.ElapsedGameTime.TotalSeconds);

                if (frameTimeElapsed <= 0.0) {
                    if (currentFrame >= sprites.Count - 1) {
                        OnFrameComplete();
                    } else {
                        NextFrame();
                    }

                    frameTimeElapsed = (float)frameDelays[currentFrame];
                }

                if (totalTimeRemaining <= 0.0) {
                    totalTimeRemaining = 0.0f;
                }
            }
        }

        public void Update(GameTime gameTime, Vector3 position, Vector2 scale) {
            if (IsLeft()) {
                this.position.X = position.X - (spriteOffset.X * scale.X) - (offsets[currentFrame].X * scale.X);
            } else {
                this.position.X = position.X + (spriteOffset.X * scale.X) + (offsets[currentFrame].X * scale.X);
            }

            this.position.Y = (position.Y + (spriteOffset.Y * scale.Y) + (offsets[currentFrame].Y * scale.Y)) + position.Z;
        }
    }
}
