using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class LifeBar {
        public enum SpriteType {PLACEHOLDER, CONTAINER, BAR}
        protected Dictionary<SpriteType, Entity> sprites;
        private Vector2 scale;
        protected SpriteEffects spriteEffect;
        private Entity portrait;
        private float percent;


        public LifeBar(int posx, int posy, int ox, int oy, float sx, float sy, SpriteEffects spriteEffect = SpriteEffects.None) {
            sprites = new Dictionary<SpriteType, Entity>();
            scale = new Vector2(sx, sy);
            percent = 0;
            this.spriteEffect = spriteEffect;

            Load(posx, posy, ox, oy, sx, sy);
        }

        public virtual void Load(int posx, int posy, int ox, int oy, float sx, float sy) { }

        public void AddSprite(SpriteType type, String location, int posx, int posy, int offx, int offy, float sx, float sy) {
            Entity entity = new Entity(Entity.ObjectType.LIFE_BAR, type.ToString());
            entity.AddSprite(Animation.State.NONE, location, true);
            entity.GetSprite(Animation.State.NONE).SetAnimationType(Animation.Type.REPEAT);
            SetSprite(entity, posx, posy, offx, offy, sx, sy);

            sprites.Add(type, entity);
        }

        public void SetSprite(Entity entity, int posx, int posy, int offx, int offy, float sx, float sy) {
            entity.SetPostion(posx, posy, 0);
            entity.SetOffset(Animation.State.NONE, offx, offy);
            entity.SetScale(sx, sy);
        }

        public void SetPortrait(String location, int posx, int posy, int offx, int offy, float sx, float sy) {
            portrait = new Entity(Entity.ObjectType.PORTRAIT, "PORTRAIT");
            portrait.AddSprite(Animation.State.NONE, location, true);
            SetSprite(portrait, posx, posy, offx, offy, sx, sy);
        }

        public virtual void Update(GameTime gameTime) {
            foreach (Entity bar in sprites.Values) {
                bar.UpdateAnimation(gameTime);
                bar.Update(gameTime);
            }

            if (portrait != null) {
                portrait.UpdateAnimation(gameTime);
                portrait.Update(gameTime);
            }
        }

        public void Increase(float amount) {
            percent += amount;
            UpdateBar();
        }

        public void Decrease(float amount) {
            percent -= amount;
            UpdateBar();
        }

        public void SetPercent(float amount) {
            percent = amount;
            UpdateBar();
        }

        public float GetPercent() {
            return percent;
        }

        private void UpdateBar() {
            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            Entity bar = sprites[SpriteType.BAR];
            float sx = (scale.X * (float)((double)percent / (double)100));
            bar.SetScaleX(sx);
        }

        public virtual void Render() {
            Entity placeholder = (sprites.ContainsKey(SpriteType.PLACEHOLDER) ? sprites[SpriteType.PLACEHOLDER] : null);
            Entity container = (sprites.ContainsKey(SpriteType.CONTAINER) ? sprites[SpriteType.CONTAINER] : null);
            Entity bar = (sprites.ContainsKey(SpriteType.BAR) ? sprites[SpriteType.BAR] : null);

            if (container != null) { 
                GameManager.SpriteBatch.Draw(container.GetCurrentSprite().GetCurrentTexture(), container.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, container.GetScale(), spriteEffect, 0f);
            }

            if (bar != null) { 
                GameManager.SpriteBatch.Draw(bar.GetCurrentSprite().GetCurrentTexture(), bar.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, bar.GetScale(), spriteEffect, 0f);
            }

            if (portrait != null) {
                GameManager.SpriteBatch.Draw(portrait.GetCurrentSprite().GetCurrentTexture(), portrait.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, portrait.GetScale(), spriteEffect, 0f);
            }

            if (placeholder != null) { 
                GameManager.SpriteBatch.Draw(placeholder.GetCurrentSprite().GetCurrentTexture(), placeholder.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, placeholder.GetScale(), spriteEffect, 0f);
            }
        }
    }
}
