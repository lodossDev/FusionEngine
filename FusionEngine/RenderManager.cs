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
            }
        }

        public void Update(GameTime gameTime) {
            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (!GameManager.IsPause()) {
                    entity.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime) {
            entities.Sort();

            for (int i = 0; i < entities.Count; i++) {
                Entity entity = entities[i];

                if (entity != null && entity.Alive()) {
                    GameManager.GraphicsDevice.BlendState = entity.GetBlendState();
                        
                    Sprite currentSprite = entity.GetCurrentSprite();
                    if (currentSprite == null) continue;

                    Sprite stance = entity.GetSprite(Animation.State.STANCE);

                    frameScale.X = entity.GetScale().X + currentSprite.GetCurrentScaleFrame().X;
                    frameScale.Y = entity.GetScale().Y + currentSprite.GetCurrentScaleFrame().Y;

                    if (entity.IsDrawShadow()) {
                        float x2 = entity.GetPosX() + (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X + currentSprite.GetShadowOffsetX()) * entity.GetScale().X);

                        if (entity.IsLeft()) {
                            x2 = entity.GetPosX() - (float)((currentSprite.GetSpriteOffSet().X + currentSprite.GetCurrentFrameOffSet().X + currentSprite.GetShadowOffsetX()) * entity.GetScale().X);
                        }

                        float z2 = (-entity.GetPosY() * 0.3f) + entity.GetPosZ() + (currentSprite.GetSpriteOffSet().Y + currentSprite.GetCurrentFrameOffSet().Y + currentSprite.GetShadowOffsetY()) * entity.GetScale().Y;

                        shadowScale.X = frameScale.X;
                        shadowScale.Y = (frameScale.Y * 2) / 8.2f;

                        shadowPosition.X = x2;
                        shadowPosition.Y = z2 + entity.GetCurrentSpriteHeight();

                        //Shadow
                        GameManager.SpriteBatch.Draw(currentSprite.GetCurrentTexture(), shadowPosition, null, Color.Black * 0.6f, 0, entity.GetOrigin(), shadowScale, currentSprite.GetEffects() | SpriteEffects.FlipVertically, 0f);
                    } 

                    if (entity.IsEntity(Entity.ObjectType.AFTER_IMAGE)) {
                        //After image sprite
                        Color desaturatedGreen = Color.Lerp(Color.White, Color.Green, 0.75f);
                        GameManager.SpriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, entity.GetSpriteColor() * 0.5f, 0f, entity.GetOrigin(), frameScale, entity.GetEffects(), 0f);

                    } else { 
                        if (stance != null && (renderStanceSprite == true || entity.GetName().Contains("BRED"))) {
                            //GameSystem.spriteBatch.Draw(stance.GetTextures()[0], stance.GetPosition(), null, Color.Gray * 0.8f, 0f, entity.GetStanceOrigin(), entity.GetScale(), stance.GetEffects(), 0f);
                        }

                        //Real sprite
                        if (!(entity is Wall)) {
                            GameManager.SpriteBatch.Draw(currentSprite.GetCurrentTexture(), currentSprite.GetPosition(), null, entity.GetSpriteColor(), 0f, entity.GetOrigin(), frameScale, entity.GetEffects(), 0f);
                        }

                        RenderBoxes(entity);

                        /*baseSpriteOrigin.X = (entity.GetBaseSprite().GetCurrentTexture().Width / 2);
                        baseSpriteOrigin.Y = 0;

                        GameManager.SpriteBatch.Draw(entity.GetBaseSprite().GetCurrentTexture(), entity.GetBasePosition(), null, Color.White * 1f, 0f, baseSpriteOrigin, baseSpriteScale, SpriteEffects.None, 0f);
                        */

                        if (entity.IsEntity(Entity.ObjectType.PLAYER)) {
                            //entity.gg.Draw("077128 000\nh878 78787\n343525 23432", entity.GetConvertedPosition());
                        }
                    }
                }
            }

            GameManager.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
        }
    }
}
