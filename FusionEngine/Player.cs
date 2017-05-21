using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FusionEngine {

    public class Player : Character {
        private int playerIndex;
        private SFIII_SimpleLifebar currentHitLifeBar;
        private int lifeBarHitTime;


        public Player(String name) : base(ObjectType.PLAYER, name) {
            currentHitLifeBar = null;
            lifeBarHitTime = 0;
        }

        public void SetPlayerIndex(int index) {
            playerIndex = index - 1;
        }

        public int GetPlayerIndex() {
            return playerIndex + 1;
        }

        public void SetLifebarHitTime(int time) {
            lifeBarHitTime = time;
        }

        public void SetCurrentHitLifeBar(SFIII_SimpleLifebar currentHitLifeBar) {
            this.currentHitLifeBar = currentHitLifeBar;
        }

        public void UpdateHitLifebarTimer(GameTime gameTime) {
            if (lifeBarHitTime > 0) {
                lifeBarHitTime --;
            }
        }

        public void RenderHitLifebar() {
            if (lifeBarHitTime > 0) {
                if (currentHitLifeBar != null) {
                    currentHitLifeBar.Render();
                }
            }
        }

        public void RenderHitLifebar(Vector2 holderPos, Vector2 barPos, Vector2 portraitPos, SpriteEffects effects = SpriteEffects.None) {
            if (lifeBarHitTime > 0) {
                if (currentHitLifeBar != null) {
                    currentHitLifeBar.Render(holderPos, barPos, portraitPos, effects);
                }
            }
        }

        public void RenderHitName(Vector2 pos) {
            if (lifeBarHitTime > 0) {
                if (GetAttackInfo().victim != null) {
                    GetNameFont().Draw("" + GetAttackInfo().victim.GetName() + " X" + GetAttackInfo().victim.GetLives(), pos);
                }
            }
        }
    }
}
