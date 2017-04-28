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
        private bool renderAttackBoxes;
        private bool renderBodyBoxes;
        private bool renderBoundsBoxes;
        private bool renderStanceSprite;

        private Vector2 baseSpriteScale;
        private Vector2 baseSpriteOrigin;

        private Vector2 frameScale;
        private Vector2 shadowPosition;
        private Vector2 shadowScale;

        public RenderManager() : base() {
            renderBoxes = renderAttackBoxes = renderBodyBoxes = renderBoundsBoxes = false;
            renderStanceSprite = false;
            renderBoxes = true;

            baseSpriteScale = new Vector2(0.5f, 0.5f);
            baseSpriteOrigin = Vector2.Zero;

            shadowPosition = Vector2.Zero;
            shadowScale = Vector2.Zero;
            frameScale = Vector2.Zero;
        }

        public void ShowBoxes() {
            renderBoxes = true;
            renderAttackBoxes = renderBodyBoxes = renderBoundsBoxes = renderBoxes;
        }

        public void HideBoxes() {
            renderBoxes = false;
            renderAttackBoxes = renderBodyBoxes = renderBoundsBoxes = renderBoxes;
        }

        public void RenderBoxes() {
            renderBoxes = !renderBoxes;
            renderAttackBoxes = renderBodyBoxes = renderBoundsBoxes = renderBoxes;
        }

        public void ShowAttackBoxes() {
            renderBoxes = true;
            renderAttackBoxes = true;
        }

        public void HideAttackBoxes() {
            renderBoxes = true;
            renderAttackBoxes = false;
        }

        public void ShowBodyBoxes() {
            renderBoxes = true;
            renderBodyBoxes = true;
        }

        public void HideBodyBoxes() {
            renderBoxes = true;
            renderBodyBoxes = false;
        }

        public void ShowBoundsBoxes() {
            renderBoxes = true;
            renderBoundsBoxes = true;
        }

        public void HideBoundsBoxes() {
            renderBoxes = true;
            renderBoundsBoxes = false;
        }

        public void ShowStanceSprite() {
            renderStanceSprite = true;
        }

        public void HideStanceSprite() {
            renderStanceSprite = false;
        }

        private void RenderBoxes(Entity entity) {
            if (renderBoxes) {
                foreach (CLNS.BoundingBox box in entity.GetCurrentSprite().GetCurrentBoxes()) {

                    if (box.GetBoxType() == CLNS.BoxType.HIT_BOX && renderAttackBoxes == true) { 

                        box.DrawRectangle(CLNS.DrawType.LINES);

                    } else if (box.GetBoxType() == CLNS.BoxType.BODY_BOX && renderBodyBoxes == true) {

                        box.DrawRectangle(CLNS.DrawType.LINES);

                    } else if ((box.GetBoxType() == CLNS.BoxType.BOUNDS_BOX 
                            || box.GetBoxType() == CLNS.BoxType.DEPTH_BOX) && renderBoundsBoxes == true) {

                        box.DrawRectangle(CLNS.DrawType.LINES);
                    }
                }

                if (renderBodyBoxes == true) {
                     if (entity.GetBodyBox() != null && entity.GetBodyBox().GetBoxType() == CLNS.BoxType.BODY_BOX) {
                        entity.GetBodyBox().DrawRectangle(CLNS.DrawType.LINES);
                    }
                }

                if (renderBoundsBoxes == true) { 

                    if (entity.GetBoundsBox() != null && entity.GetBoundsBox().GetBoxType() == CLNS.BoxType.BOUNDS_BOX) {
                        //entity.GetBoundsBox().DrawRectangle(CLNS.DrawType.LINES);
                    }

                    if (entity.GetDepthBox() != null && entity.GetDepthBox().GetBoxType() == CLNS.BoxType.DEPTH_BOX) {
                        entity.GetDepthBox().DrawRectangle(CLNS.DrawType.LINES);
                    }
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
                            GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                        }
                    }
                }

                if (layers2 != null && layers2.Count > 0) { 
                    foreach (Entity entity in layers2) {
                        if (entity.Alive()) {
                            entity.Update(gameTime);
                            Sprite currentSprite = entity.GetCurrentSprite();
                            GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
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
                            GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, Color.White * 1f, 0f, entity.GetOrigin(), entity.GetScale(), entity.GetEffects(), 0f);
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime) {
            entities.Sort();
            RenderLevelBackLayers(gameTime);

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (entity != null && entity.Alive()) {
                    if (!GameManager.IsPause()) {
                        entity.Update(gameTime);
                    }

                    if (entity.IsEntity(Entity.ObjectType.HIT_FLASH) /*|| entity.IsEntity(Entity.ObjectType.AFTER_IMAGE)*/) {
                        GameManager.GetGraphicsDevice().BlendState = BlendState.Additive;
                    } else {
                        GameManager.GetGraphicsDevice().BlendState = BlendState.NonPremultiplied;
                    }

                    Sprite currentSprite = entity.GetCurrentSprite();
                    if (currentSprite == null) continue;

                    Sprite stance = entity.GetSprite(Animation.State.STANCE);

                    frameScale.X = entity.GetScale().X + currentSprite.GetCurrentScaleFrame().X;
                    frameScale.Y = entity.GetScale().Y + currentSprite.GetCurrentScaleFrame().Y;

                    if (entity.IsEntity(Entity.ObjectType.AFTER_IMAGE) == false) { 
                        float x2 = entity.GetPosition().X + (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X + currentSprite.GetShadowOffsetX()) * entity.GetScale().X);

                        if (entity.IsLeft()) {
                            x2 = entity.GetPosition().X - (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X +  + currentSprite.GetShadowOffsetX()) * entity.GetScale().X);
                        }

                        float z2 = entity.GetPosition().Z + (currentSprite.GetSpriteOffSet().Y + currentSprite.GetCurrentFrameOffSet().Y + currentSprite.GetShadowOffsetY()) * entity.GetScale().Y;

                        shadowPosition.X = x2;
                        shadowPosition.Y = z2 + entity.GetCurrentSpriteHeight();

                        shadowScale.X = frameScale.X;
                        shadowScale.Y = (frameScale.Y * 2) / 8f;

                        //Shadow
                        GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), shadowPosition, null, Color.Black * 0.6f, 0, entity.GetOrigin(), shadowScale, currentSprite.GetEffects() | SpriteEffects.FlipVertically, 0f);

                        if (stance != null && (renderStanceSprite == true || entity.GetName().Contains("BRED"))) {
                            //GameSystem.spriteBatch.Draw(stance.GetTextures()[0], stance.GetPosition(), null, Color.Gray * 0.8f, 0f, entity.GetStanceOrigin(), entity.GetScale(), stance.GetEffects(), 0f);
                        }

                         //Real sprite
                        GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, entity.GetSpriteColor(), 0f, entity.GetOrigin(), frameScale, entity.GetEffects(), 0f);

                    } else {
                        //After image sprite
                        Color desaturatedGreen = Color.Lerp(Color.White, Color.Blue, 0.75f);
                        GameManager.GetSpriteBatch().Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, desaturatedGreen * 0.5f, 0f, entity.GetOrigin(), frameScale, entity.GetEffects(), 0f);
                    }
                    
                    
                    RenderBoxes(entity);

                    if (entity.IsEntity(Entity.ObjectType.AFTER_IMAGE) == false) { 
                        baseSpriteOrigin.X = (entity.GetBaseSprite().GetCurrentTexture().Width / 2);
                        baseSpriteOrigin.Y = 0;

                        GameManager.GetSpriteBatch().Draw(entity.GetBaseSprite().GetCurrentTexture(), entity.GetBasePosition(), null, Color.White * 1f, 0f, baseSpriteOrigin, baseSpriteScale, SpriteEffects.None, 0f);

                    }

                    if (entity.IsEntity(Entity.ObjectType.PLAYER)) {
                        //entity.gg.Draw("077128 000\nh878 78787\n343525 23432", entity.GetConvertedPosition());
                    }
                }
            }

            GameManager.GetGraphicsDevice().BlendState = BlendState.NonPremultiplied;
            RenderLevelFrontLayers(gameTime);
        }
    }
}
