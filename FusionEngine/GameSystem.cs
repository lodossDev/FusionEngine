﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class GameSystem {
        private List<SFIII_Lifebar> playerLifeBars;
        private List<SFIII_MPbar> playerMPBars;
        private List<Entity> timePlaceHolders;
        private Entity currentTimePlaceHolder;
        private int time;
        private int currentTime;
        private MugenFont timeFont;
        private MugenFont nameFont;
        private List<MugenFont> comboFonts;
        private MugenFont numberFont;


        public GameSystem() {
            playerLifeBars = new List<SFIII_Lifebar>();
            playerMPBars = new List<SFIII_MPbar>();
            timePlaceHolders = new List<Entity>();

            Load();
        }

        private void Load() {
            SFIII_Lifebar bar = new SFIII_Lifebar(5, 20, 21, 18, 4.08f, 3f);
            bar.SetPortrait("Sprites/Actors/Ryo/PORTRAIT", 29, 65, 0, 0, 4.08f, 3f);
            playerLifeBars.Add(bar);

            bar = new SFIII_Lifebar(635, 20, 93, 18, 4.08f, 3f, SpriteEffects.FlipHorizontally);
            //playerLifeBars.Add(bar);

            SFIII_MPbar mpbar = new SFIII_MPbar(5, 725, 163, 45, 3.8f, 3.8f);
            mpbar.SetPercent(0);
            mpbar.SetMpLevelFont(15, 731, 3.8f);
            mpbar.SetMpMaxLevelFont(11, 763, 1.5f);
            mpbar.SetMaxLevel(5);
            mpbar.SetCurrentLevel(5);
            playerMPBars.Add(mpbar);

            mpbar = new SFIII_MPbar(792, 725, 15, 45, 3.8f, 3.8f, SpriteEffects.FlipHorizontally);
            mpbar.SetPercent(0);
            mpbar.SetMpLevelFont(1155, 732, 3.8f);
            playerMPBars.Add(mpbar);

            Entity timePlaceHolder = new Entity(Entity.ObjectType.SYSTEM, "TIME_PLACEHOLDER1");
            timePlaceHolder.AddSprite(Animation.State.NONE, "Sprites/LifeBars/SFIII/TIMER1", true);
            timePlaceHolder.SetScale(3.8f, 3f);
            timePlaceHolder.SetPostion(550, 20);
            timePlaceHolders.Add(timePlaceHolder);

            timePlaceHolder = new Entity(Entity.ObjectType.SYSTEM, "TIME_PLACEHOLDER2");
            timePlaceHolder.AddSprite(Animation.State.NONE, "Sprites/LifeBars/SFIII/TIMER2", true);
            timePlaceHolder.SetScale(3.8f, 3f);
            timePlaceHolder.SetPostion(550, 20);
            timePlaceHolders.Add(timePlaceHolder);

            currentTimePlaceHolder = timePlaceHolders[0];

            timeFont = new MugenFont("Fonts/sfiii_timer.xFont", 4, 0, 2.8f);
            nameFont = new MugenFont("Fonts/sfiii_name.xFont", 4, 0, 2.8f);
            numberFont = new MugenFont("Fonts/sfiii_number.xFont", 4, 0, 2.8f);

            comboFonts = new List<MugenFont>();
            comboFonts.Add(new MugenFont("Fonts/sfiii_combo.xFont", new Vector2(-200, 280), 4, 0, 2.8f));
            comboFonts[0].Translate(100, 0, 10, 0, 100);

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
            /*if (comboFont != null) {
                comboFont.Draw("999 H\n99 H");
            }*/

            int playerIndex = 0;

            comboFonts[0].Draw("999 H\n99 H");

            foreach (MugenFont font in comboFonts) {

            }

            foreach (LifeBar bar in playerLifeBars) {
                bar.Render();
            }

            foreach (LifeBar bar in playerMPBars) {
                bar.Render();
            }

            if (currentTimePlaceHolder != null) {
                GameManager.SpriteBatch.Draw(currentTimePlaceHolder.GetCurrentSprite().GetCurrentTexture(), currentTimePlaceHolder.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, Vector2.Zero, currentTimePlaceHolder.GetScale(), SpriteEffects.None, 0f);
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

            if (nameFont != null) {
                nameFont.Draw("RYO1", new Vector2(300, 56));
                nameFont.Draw("RYO2", new Vector2(900, 56));
            }

            if (numberFont != null) {
                numberFont.Draw("0000000", new Vector2(15, 0));
                numberFont.Draw("0000000", new Vector2(1106, 0));
            }
        }
    }
}
