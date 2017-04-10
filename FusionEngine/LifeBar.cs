using Microsoft.Xna.Framework;
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

        
        public LifeBar(int posx, int posy, float sx = 3f, float sy = 3f) {
            sprites = new Dictionary<SpriteType, Entity>();
            position = new Vector2(posx, posy);
            scale = new Vector2(sx, sy);

            Load(posx, posy, sx, sy);
        }

        public virtual void Load(int posx, int posy, float sx, float sy) {
            AddSprite(SpriteType.PLACEHOLDER, "Sprites/LifeBars/RedEarth/Placeholder", posx, posy, 0, 0, sx, sy);
            AddSprite(SpriteType.CONTAINER, "Sprites/LifeBars/RedEarth/Container", posx, posy, 144, 33, sx, sy);
            AddSprite(SpriteType.BAR, "Sprites/LifeBars/RedEarth/Bar", posx, posy, 144, 33, sx, sy);
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

            GameManager.GetSpriteBatch().Draw(placeholder.GetCurrentSprite().GetCurrentTexture(), placeholder.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, placeholder.GetScale(), placeholder.GetEffects(), 0f);
            GameManager.GetSpriteBatch().Draw(container.GetCurrentSprite().GetCurrentTexture(), container.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, container.GetScale(), container.GetEffects(), 0f);
            GameManager.GetSpriteBatch().Draw(bar.GetCurrentSprite().GetCurrentTexture(), bar.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, bar.GetScale(), bar.GetEffects(), 0f);
        }
    }
}
