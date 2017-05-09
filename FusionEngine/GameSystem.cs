using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class GameSystem {
        private List<LifeBar> playerLifeBars;
        private List<LifeBar> playerMPBars;
        private List<Entity> timePlaceHolders;
        private Entity currentTimePlaceHolder;
        private int time;
        private int currentTime;
        private MugenFont timeFont;


        public GameSystem() {
            playerLifeBars = new List<LifeBar>();
            playerMPBars = new List<LifeBar>();
            timePlaceHolders = new List<Entity>();

            Load();
        }

        private void Load() {
            LifeBar bar = new LifeBar(46, 20, 19, 18, 3.8f, 3f);
            bar.SetPortrait("Sprites/Actors/Ryo/PORTRAIT", 69, 65, 0, 0, 3.8f, 3f);
            playerLifeBars.Add(bar);

            bar = new LifeBar(639, 20, 85, 18, 3.8f, 3f, SpriteEffects.FlipHorizontally);
            playerLifeBars.Add(bar);

            Entity timePlaceHolder = new Entity(Entity.ObjectType.SYSTEM, "TIME_PLACEHOLDER");
            timePlaceHolder.AddSprite(Animation.State.STANCE, "Sprites/LifeBars/SFIII/TIMER2", true);
            timePlaceHolder.SetScale(3.8f, 3f);
            timePlaceHolder.SetPostion(550, 20);
            timePlaceHolders.Add(timePlaceHolder);

            timeFont = new MugenFont("Fonts/sfiii_timer.xFont", new Vector2(619, 1), 4, 0, 2.8f);

            time = 0;
            currentTime = 0;
        }

        public void Update(GameTime gameTime) {

            if (GameManager.IsPause())return;

            foreach (LifeBar bar in playerLifeBars) {
                bar.Update(gameTime);
            }

            foreach (LifeBar bar in playerMPBars) {
                bar.Update(gameTime);
            }

            foreach (Entity entity in timePlaceHolders) {
                entity.Update(gameTime);
            }

            if (time < 999) {
                if ((currentTime += (gameTime.ElapsedGameTime.Seconds) + 1) > 60) {
                    time += 1;
                    currentTime = 0;
                }
            }
        }

        public void Render(GameTime gameTime) {
            foreach (LifeBar bar in playerLifeBars) {
                bar.Render();
            }

            foreach (LifeBar bar in playerMPBars) {
                bar.Render();
            }

            if (timePlaceHolders.Count > 0) {
                foreach (Entity entity in timePlaceHolders) {
                    GameManager.SpriteBatch.Draw(entity.GetCurrentSprite().GetCurrentTexture(), entity.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, entity.GetScale(), SpriteEffects.None, 0f);
                }
            }

            if (timeFont != null) {
                if (time < 10) {
                    timeFont.SetPosX(619);

                } else if (time < 100) {
                    timeFont.SetPosX(594);

                } else if (time >= 200) {
                    timeFont.SetPosX(573);

                }  else {
                    timeFont.SetPosX(570);
                }

                timeFont.Draw("" + time);
            }
        }
    }
}
