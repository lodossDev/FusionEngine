using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class GameSystem {
        private List<Player> players;
        private List<SFIII_Lifebar> playerLifeBars;
        private List<SFIII_MPbar> playerMPBars;
        private List<Entity> timePlaceHolders;
        private Entity currentTimePlaceHolder;
        private int time;
        private int currentTime;
        private MugenFont timeFont;
        private MugenFont nameFont;
        private MugenFont numberFont;
        private Vector2 p1NamePos, p2NamePos, p1LivesPos, p2LivesPos, p1PointsPos, p2PointsPos;
        private Vector2 hitHolderPos1, hitBarPos1, hitPortraitPos1, hitHolderPos2, hitBarPos2, hitPortraitPos2; 
             

        public GameSystem() {
            players = new List<Player>();
            playerLifeBars = new List<SFIII_Lifebar>();
            playerMPBars = new List<SFIII_MPbar>();
            timePlaceHolders = new List<Entity>();
            p1NamePos = p2NamePos = p1LivesPos = p2LivesPos = p1PointsPos = p2PointsPos = Vector2.Zero;
            hitHolderPos1 = hitBarPos1 = hitPortraitPos1 = hitHolderPos2 = hitBarPos2 = hitPortraitPos2 = Vector2.Zero;

            Load();
        }

        public void AddPlayer(Player player) {
            players.Add(player);
        }

        public void AddPlayer(List<Player> players) {
            players.AddRange(players);
        }

        public void RemovePlayer(Player player) {
            players.Remove(player);
        }

        public void RemovePlayer(List<Player> players) {
            foreach (Player player in players) {
                RemovePlayer(player);
            }
        }

        private void SetPortrait(SFIII_Lifebar bar, Player player) {
            if (bar != null && player != null && player.GetPortrait() != null) {
                Attributes.Portrait portrait = player.GetPortrait();

                if (portrait != null) { 
                    bar.SetPortrait(portrait.location, portrait.posx, portrait.posy, portrait.offx, portrait.offy, portrait.sx, portrait.sy);
                }
            }
        }

        public void Load() {
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

            currentTimePlaceHolder = timePlaceHolders[1];

            SFIII_Lifebar bar = new SFIII_Lifebar(5, 20, 21, 18, 4.08f, 3f);
            playerLifeBars.Add(bar);

            SFIII_MPbar mpbar = new SFIII_MPbar(5, 725, 163, 45, 3.8f, 3.8f);
            mpbar.SetPercent(0);
            mpbar.SetMpLevelFont(15, 731, 3.8f);
            mpbar.SetMpMaxLevelFont(16, 765, 1.5f);
            playerMPBars.Add(mpbar);

            bar = new SFIII_Lifebar(635, 20, 93, 18, 4.08f, 3f, SpriteEffects.FlipHorizontally);
            playerLifeBars.Add(bar);

            mpbar = new SFIII_MPbar(792, 725, 15, 45, 3.8f, 3.8f, SpriteEffects.FlipHorizontally);
            mpbar.SetPercent(0);
            mpbar.SetMpLevelFont(1155, 732, 3.8f);
            mpbar.SetMpMaxLevelFont(1238, 765, 1.5f);
            playerMPBars.Add(mpbar);
               
            timeFont = new MugenFont("Fonts/sfiii_timer.xFont", 4, 0, 2.8f);
            nameFont = new MugenFont("Fonts/sfiii_name.xFont", 4, 0, 2.8f);
            numberFont = new MugenFont("Fonts/sfiii_number.xFont", 4, 0, 2.8f);

            p1NamePos = new Vector2(300, 56);
            p1LivesPos = new Vector2(500, 10); 
            p1PointsPos = new Vector2(15, 0);

            p2NamePos = new Vector2(900, 56);
            p2LivesPos = new Vector2(735, 10);
            p2PointsPos = new Vector2(1085, 0);

            hitHolderPos1 = Vector2.Zero;
            hitBarPos1 = new Vector2(220, 110);
            hitPortraitPos1 = new Vector2(160, 85);

            hitHolderPos2 = Vector2.Zero;
            hitBarPos2 = new Vector2(803, 110);
            hitPortraitPos2 = new Vector2(1055, 85);

            time = 0;
            currentTime = 0;
        }

        public void Update(GameTime gameTime) {
            if (GameManager.IsPause())return;

            Player player1 = (players.Count >= 1 ? players[0] : null); 
            Player player2 = (players.Count >= 2 ? players[1] : null); 

            foreach (LifeBar bar in playerLifeBars) {
                bar.Update(gameTime);
            }

            if (player1 != null) {
                playerLifeBars[0].SetPercent(player1.GetHealth());
                playerMPBars[0].SetMaxLevel(player1.GetMaxMpLevel());
                playerMPBars[0].SetCurrentLevel(player1.GetCurrentMpLevel());
                playerMPBars[0].SetPercent(player1.GetMP());
            }

             if (player2 != null) {
                playerLifeBars[1].SetPercent(player1.GetHealth());
                playerMPBars[1].SetMaxLevel(player2.GetMaxMpLevel());
                playerMPBars[1].SetCurrentLevel(player2.GetCurrentMpLevel());
                playerMPBars[1].SetPercent(player2.GetMP());
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
            Player player1 = (players.Count >= 1 ? players[0] : null); 
            Player player2 = (players.Count >= 2 ? players[1] : null); 
            
            if (player1 != null) {
                SetPortrait(playerLifeBars[0], player1);
                playerLifeBars[0].Render();
                playerLifeBars[1].Render();
                playerMPBars[0].Render();
                playerMPBars[1].Render();
                player1.RenderHitLifebar(hitHolderPos1, hitBarPos1, hitPortraitPos1);
                player1.RenderHitLifebar(hitHolderPos2, hitBarPos2, hitPortraitPos2, SpriteEffects.FlipHorizontally);
                player1.RenderHitName(new Vector2(230, 84));
                player1.RenderHitName(new Vector2(880, 84));
            }  

            if (player2 != null) {
                currentTimePlaceHolder = timePlaceHolders[1];
                SetPortrait(playerLifeBars[1], player2);
                playerLifeBars[1].Render();
                playerMPBars[1].Render();
                player2.RenderHitLifebar();
            } else {
                currentTimePlaceHolder = timePlaceHolders[0];
            }
                     
            foreach (Player player in players) {
                MugenFont comboFont = player.GetComboFont();

                if (comboFont != null) {
                    comboFont.Draw("" + player.GetAttackInfo().comboHits + " H");
                }
            }

            if (currentTimePlaceHolder != null) {
                GameManager.SpriteBatch.Draw(currentTimePlaceHolder.GetCurrentSprite().GetCurrentTexture(), 
                                                currentTimePlaceHolder.GetCurrentSprite().GetPosition(), null, Color.White * 1f, 0f, 
                                                Vector2.Zero, currentTimePlaceHolder.GetScale(), SpriteEffects.None, 0f);
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
                if (player1 != null) { 
                    nameFont.Draw("" + player1.GetName(), p1NamePos);
                    nameFont.Draw("X" + player1.GetLives(), p1LivesPos);
                }

                if (player2 != null) { 
                    nameFont.Draw("" + player2.GetName(), p2NamePos);
                    nameFont.Draw("" + player2.GetLives() + "X", p2LivesPos);
                }
            }

            if (numberFont != null) {
                if (player1 != null) { 
                    numberFont.Draw("" + player1.GetPoints().ToString("D8"), p1PointsPos);
                }

                if (player2 != null) { 
                    numberFont.Draw("" + player2.GetPoints().ToString("D8"), p2PointsPos);
                }
            }
        }
    }
}
