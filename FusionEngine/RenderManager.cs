using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FusionEngine {

    public class RenderManager : Manager {
        private bool renderBoxes;
        private Vector2 baseSpriteScale;
        private Vector2 baseSpriteOrigin;
        private SpriteFont font1;

        private Vector2 frameScale;
        private Vector2 shadowPosition;
        private Vector2 shadowScale;

        public RenderManager() {
            renderBoxes = false;
            baseSpriteScale = new Vector2(0.5f, 0.5f);
            baseSpriteOrigin = Vector2.Zero;

            shadowPosition = Vector2.Zero;
            shadowScale = Vector2.Zero;
            frameScale = Vector2.Zero;

            //font1 = System.contentManager.Load<SpriteFont>("Fonts/MyFont");
        }

        public void ShowBoxes() {
            renderBoxes = true;
        }

        public void HideBoxes() {
            renderBoxes = false;
        }

        public void RenderBoxes() {
            renderBoxes = !renderBoxes;
        }

        private void RenderBoxes(Entity entity) {
            if (renderBoxes) {
                foreach (CLNS.BoundingBox box in entity.GetCurrentSprite().GetCurrentBoxes()) {
                    box.DrawRectangle(CLNS.DrawType.LINES);
                }

                if (entity.GetBoundsBox() != null) {
                    entity.GetBoundsBox().DrawRectangle(CLNS.DrawType.LINES);
                }

                if (entity.GetBodyBox() != null) {
                    //entity.GetBodyBox().DrawRectangle(CLNS.DrawType.LINES);
                }

                if (entity.GetDepthBox() != null) {
                    entity.GetDepthBox().DrawRectangle(CLNS.DrawType.LINES);
                }

                if (entity.GetRayBottom() != null) {
                    //entity.GetBoundsBottomRay().DrawRectangle(CLNS.DrawType.LINES);
                }

                if (entity.GetRayTop() != null) {
                    //entity.GetBoundsTopRay().DrawRectangle(CLNS.DrawType.LINES);
                }
            }
        }

        private void RenderLevelBackLayers(GameTime gameTime) {
            foreach (Level level in levels) {
                List<Entity> layers1 = level.GetLayers(1);
                List<Entity> layers2 = level.GetLayers(2);

                if (layers1 != null && layers1.Count > 0) { 
                    foreach (Entity entity in layers1) {
                        if (entity.Alive()) {
                            entity.Update(gameTime);
                            Sprite currentSprite = entity.GetCurrentSprite();
                            System.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                        }
                    }
                }

                if (layers2 != null && layers2.Count > 0) { 
                    foreach (Entity entity in layers2) {
                        if (entity.Alive()) {
                            entity.Update(gameTime);
                            Sprite currentSprite = entity.GetCurrentSprite();
                            System.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                        }
                    }
                }
            }
        }

        private void RenderLevelFrontLayers(GameTime gameTime) {
            foreach (Level level in levels) {
                List<Entity> layers3 = level.GetLayers(3);

                if (layers3 != null && layers3.Count > 0) { 
                    foreach (Entity entity in layers3) {
                        if (entity.Alive()) {
                            entity.Update(gameTime);
                            Sprite currentSprite = entity.GetCurrentSprite();
                            System.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                        }
                    }
                }
            }
        }

        public void Update(GameTime gameTime) {
            entities.RemoveAll(item => item.IsEntity(Entity.ObjectType.HIT_FLASH) && item.GetCurrentSprite().IsAnimationComplete());

            foreach (Entity entity in entities) {
                if (entity.Alive()) {
                    entity.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime) {
            entities.Sort();
            RenderLevelBackLayers(gameTime);

            foreach (Entity entity in entities) {

                if (entity != null && entity.Alive()) {

                    if (entity.IsEntity(Entity.ObjectType.HIT_FLASH)) {
                        System.graphicsDevice.BlendState = BlendState.Additive;
                    } else {
                        System.graphicsDevice.BlendState = BlendState.NonPremultiplied;
                    }

                    Sprite currentSprite = entity.GetCurrentSprite();
                    Sprite stance = entity.GetSprite(Animation.State.STANCE);

                    //if (stance != null && entity is Player) {
                        System.spriteBatch.Draw(stance.GetTextures()[0], stance.GetPosition(), null, Color.White * 0.8f, 0f, entity.GetStanceOrigin(), entity.GetScale(), stance.GetEffects(), 0f);
                    //}

                    float x2 = entity.GetPosition().X + (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X) * entity.GetScale().X);

                    if (entity.IsLeft()) {
                        x2 = entity.GetPosition().X - (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X) * entity.GetScale().X);
                    }

                    float z2 = entity.GetPosition().Z + (currentSprite.GetSpriteOffSet().Y + currentSprite.GetCurrentFrameOffSet().Y) * entity.GetScale().Y;

                    shadowPosition.X = x2;
                    shadowPosition.Y = z2 + entity.GetCurrentSpriteHeight();

                    shadowScale.X = entity.GetScale().X;
                    shadowScale.Y =  12f / 8f;

                    frameScale.X = entity.GetScale().X + currentSprite.GetCurrentScaleFrame().X;
                    frameScale.Y = entity.GetScale().Y + currentSprite.GetCurrentScaleFrame().Y;

                    //Shadow
                    System.spriteBatch.Draw(currentSprite.GetCurrentTexture(), shadowPosition, null, Color.Black * 0.6f, System.rotate, entity.GetOrigin(), shadowScale, currentSprite.GetEffects() | SpriteEffects.FlipVertically, 0f);

                    //Real sprite
                    System.spriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, entity.GetSpriteColor(), 0f, entity.GetOrigin(), frameScale, entity.GetEffects(), 0f);
                    
                    RenderBoxes(entity);

                    baseSpriteOrigin.X = (entity.GetBaseSprite().GetCurrentTexture().Width / 2);
                    baseSpriteOrigin.Y = 0;

                    System.spriteBatch.Draw(entity.GetBaseSprite().GetCurrentTexture(), entity.GetBasePosition(), null, Color.White * 1f, 0f, baseSpriteOrigin, baseSpriteScale, SpriteEffects.None, 0f);
                }
            }

            System.graphicsDevice.BlendState = BlendState.NonPremultiplied;
            //System.spriteBatch.DrawString(font1, "AMIR GROUND BASE: ", new Vector2(20, 220), Color.Blue);
            RenderLevelFrontLayers(gameTime);
        }
    }
}
