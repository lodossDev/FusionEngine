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
        private Vector2 position;
        private Vector2 scale;
        private Dictionary<SpriteType, Entity> sprites;

        
        public LifeBar(int posx, int posy, float sx = 3.8f, float sy = 3f, bool isLeft = false) {
            sprites = new Dictionary<SpriteType, Entity>();
            position = new Vector2(posx, posy);
            scale = new Vector2(sx, sy);

            Load(posx, posy, sx, sy, isLeft);
            if (isLeft)SetIsLeft(isLeft);
        }

        public virtual void Load(int posx, int posy, float sx, float sy, bool isLeft = false) {
            AddSprite(SpriteType.PLACEHOLDER, "Sprites/LifeBars/SFIII/PLACEHOLDER", posx, posy, 0, 0, sx, sy);

            int ox = (isLeft ? -85 : 19);
            AddSprite(SpriteType.CONTAINER, "Sprites/LifeBars/SFIII/CONTAINER", posx, posy, ox, 18, sx, sy);
            AddSprite(SpriteType.BAR, "Sprites/LifeBars/SFIII/BAR", posx, posy, ox, 18, sx, sy);
        }

        public void AddSprite(SpriteType type, String location, int posx, int posy, int offx, int offy, float sx, float sy) {
            Entity entity = new Entity(Entity.ObjectType.LIFE_BAR, type.ToString());
            entity.AddSprite(Animation.State.NONE, location, true);
            entity.SetPostion(posx, posy, 0);
            entity.SetOffset(Animation.State.NONE, offx, offy);
            entity.SetScale(sx, sy);

            sprites.Add(type, entity);
        }

        public void Update(GameTime gameTime) {
            foreach(Entity bar in sprites.Values) {
                bar.Update(gameTime);
            }
        }

        private void SetIsLeft(bool status) {
            foreach(Entity bar in sprites.Values) {
                bar.SetIsLeft(true);
            }
        }

        public void Percent(int percent) {
            Entity bar = sprites[SpriteType.BAR];

            if (percent < 0) percent = 0;
            if (percent > 100) percent = 100;

            float sx = (scale.X * (float)((double)percent / (double)100));
            bar.SetScaleX(sx);
        }

        public void Render() {
            Entity placeholder = sprites[SpriteType.PLACEHOLDER];
            Entity container = sprites[SpriteType.CONTAINER];
            Entity bar = sprites[SpriteType.BAR];

            GameManager.SpriteBatch.Draw(container.GetCurrentSprite().GetCurrentTexture(), container.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, container.GetScale(), container.GetEffects(), 0f);
            GameManager.SpriteBatch.Draw(bar.GetCurrentSprite().GetCurrentTexture(), bar.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, bar.GetScale(), bar.GetEffects(), 0f);
            GameManager.SpriteBatch.Draw(placeholder.GetCurrentSprite().GetCurrentTexture(), placeholder.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, placeholder.GetScale(), placeholder.GetEffects(), 0f);
        }
    }
}
