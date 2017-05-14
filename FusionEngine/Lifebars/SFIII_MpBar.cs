using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace FusionEngine {

    public class SFIII_MPbar : LifeBar {
        private int levels;
        private int currentLevel;
        private MugenFont mpLevelFont;
        private MugenFont mpMaxLevelFont;

        public SFIII_MPbar(int posx, int posy, int ox, int oy, float sx, float sy, SpriteEffects spriteEffect = SpriteEffects.None) : base(posx, posy, ox, oy, sx, sy, spriteEffect) {
            levels = 1;
            currentLevel = 0;
        }

        public override void Load(int posx, int posy, int ox, int oy, float sx, float sy) {
            AddSprite(SpriteType.PLACEHOLDER, "Sprites/LifeBars/SFIII/MP/PLACEHOLDER", posx, posy, 0, 0, sx, sy);
            //AddSprite(SpriteType.CONTAINER, "Sprites/LifeBars/SFIII/MP/CONTAINER", posx, posy, ox, oy, sx, sy);
            AddSprite(SpriteType.BAR, "Sprites/LifeBars/SFIII/MP/BAR", posx, posy, ox, oy, sx, sy);
            sprites[SpriteType.BAR].SetFrameDelay(Animation.State.NONE, 6);
        }

        public void SetMpLevelFont(float x, float y, float scale) {
            if (spriteEffect == SpriteEffects.None) {
                mpLevelFont = new MugenFont("Fonts/sfiii_mp_state_p1.xFont", new Vector2(x, y), 0, 0, scale);
            } else {
                mpLevelFont = new MugenFont("Fonts/sfiii_mp_state_p2.xFont", new Vector2(x, y), 0, 0, scale);
            }
        }

        public void SetMpMaxLevelFont(float x, float y, float scale) {
             mpMaxLevelFont = new MugenFont("Fonts/power_16.xFont", new Vector2(x, y), 0, 0, scale);
        }

        public void SetCurrentLevel(int level) {
            currentLevel = level;
        }

        public void SetMaxLevel(int level) {
            levels = level;
        }

        public int GetCurrentLevel() {
            return currentLevel;
        }

        public int GetMaxLevel() {
            return levels;
        }

        public void IncreaseLevel() {
            currentLevel++;

            if (currentLevel > levels - 1) {
                currentLevel = levels;
            }
        }

        public void DecreaseLevel() {
            currentLevel--;

            if (currentLevel < 0) {
                currentLevel = 0;
            }
        }

        public override void Render() {
            base.Render();

            if (mpLevelFont != null) {
                mpLevelFont.Draw("" + currentLevel);
            }

            if (mpMaxLevelFont != null) {
                mpMaxLevelFont.Draw("" + levels);
            }
        }
    }
}
