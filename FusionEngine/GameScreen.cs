using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace FusionEngine {
    public class GameScreen : IGameScreen {
        private Player ryo;
        private Enemy_Bred bred, bred2;
        private GameSystem system;
        private Stage1 level1;

        private List<Enemy_Bred> breds = new List<Enemy_Bred>();

        public static Texture2D sand;
        private Vector2 x_finder;
        private List<Line> lines;
        private float x = 0, y = 0, z = 0;
        private ScreenManager manager;
        private MugenFont font1;


        public GameScreen(ScreenManager manager) {
            this.manager = manager;
        }

        public void LoadContent() {
            ryo = new Player_Ryo();

            sand = GameManager.ContentManager.Load<Texture2D>("Sprites//sand");

            lines = new List<Line>();

            //this vector is rotated around vector.zero, so we get an x value that waves up and down...
            // CHANGE this value to determine how far and fast the waves move
            x_finder = new Vector2(5, 0);

            //here we add a rectangle for each line of image we want wobbled.
            //the rectangle will be modified dynamically by the x_finder above.

            for (int i = 0; i < 1080; i++)
            {
                lines.Add(new Line(new Rectangle(0, i, 1920, 1)));              
                x_finder = (RotateVector2(x_finder, 0.02f, Vector2.Zero));
                lines[i].my_destination = new Rectangle(  (int)x_finder.X , lines[i].my_destination.Y, lines[i].my_destination.Width, lines[i].my_destination.Height);
                lines[i].x_finder += x_finder;
                lines[i].my_from = (new Rectangle(0,i,1920,1));
            }
     
            system = new GameSystem();
            system.AddPlayer(ryo);

            bred = new Enemy_Bred();

            bred2 = new Enemy_Bred();
            bred2.SetPostion(600, 0, 200);
            bred2.SetName("BRED2");

            for (int i = 0; i < 10; i++)
            {
                Enemy_Bred bredEnemy = new Enemy_Bred();
                bredEnemy.SetPostion(100 + (i * 20), 0, -55 + (i * 100));
                breds.Add(bredEnemy);
            }

            level1 = new Stage1();

            font1 = new MugenFont("Fonts/bigfont.xFont", -8, 32, 0.5f);

            GameManager.GetInstance().AddEntity(ryo);
            GameManager.GetInstance().SetLevel(level1);
            GameManager.GetInstance().AddEntity(bred);
            //GameManager.GetInstance().AddEntity(bred2);

            for (int i = 0; i < 10; i++)
            {
                GameManager.GetInstance().AddEntity(breds[i]);
            }
        }

        public void Dispose() {
            //throw new NotImplementedException();
        }

        public void Update(GameTime gameTime) {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
               // Exit();

            if (manager.IsKeyPressed(Keys.P)) {
                GameManager.PauseGame();
            }

            if (manager.IsKeyPressed(Keys.T)) {
                GameManager.ChangeResolution(600, 420);
            }

            if (manager.IsKeyPressed(Keys.Y)) {
                GameManager.ChangeResolution(GameManager.RESOLUTION_X, GameManager.RESOLUTION_Y);
            }

            if (manager.IsKeyPressed(Keys.Q)) {
                GameManager.GetInstance().RenderManager.RenderBoxes();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X)) {
                //GameManager.TakeScreenshot(this);
                GameManager.GetInstance().DropAllEnemies();
                ryo.SetAnimationState(Animation.State.SPECIAL3);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                //GameManager.Camera.Zoom += 0.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width / 2, Setup.graphicsDevice.Viewport.Height / 2);
                //Vector2 pos = new Vector2(-(camera.Zoom * 3f), 0);
                //camera.Move(pos);
                //ryo.DecreaseHealth(1);
                GameManager.GetInstance().SetSlowMotion(5);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                //GameManager.Camera.Zoom -= 0.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width/2, Setup.graphicsDevice.Viewport.Height/2);
                //Vector2 pos = new Vector2((camera.Zoom * 3f), 0);
                //camera.Move(pos);
                //ryo.SetHealth(100);
                //ryo.IncreaseHealth(10);
            }

            //bar.Percent((int)barHealth);

            if (!GameManager.IsPause())
            {
                GameManager.GetInstance().Update(gameTime);

                if (!bred.IsDying()) { 
                    bred.UpdateAI(gameTime, GameManager.GetInstance().Players);
                }

                if (!bred2.IsDying()) { 
                    //bred2.UpdateAI(gameTime, GameManager.GetInstance().Players);
                }

                for (int i = 0; i < 10; i++)
                {
                    if (!breds[i].IsDying())
                    {
                        breds[i].UpdateAI(gameTime, GameManager.GetInstance().Players);
                    }
                }
            }

            system.Update(gameTime);

            /*foreach (Line line in lines){
                line.Update();
            }*/

            if (!GameManager.GetInstance().IsSlowMotion()) {
                UpdateCamera(gameTime);
            }
        }

        private void UpdateCamera(GameTime gameTime) {
            x = 0;
            float velX = (ryo.GetAccelX() / GameManager.GAME_VELOCITY) * ryo.GetDirX();

            if (ryo.GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION) { 
                x = (float)Math.Round((double)(velX + (ryo.GetTossInfo().velocity.X * 1f)));
            }

            //Debug.WriteLine("X: " + (ryo.GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION));

            z = 0;
            float velZ = (ryo.GetAccelZ() / GameManager.GAME_VELOCITY) * ryo.GetDirZ();

            if (ryo.GetDirZ() < 0 && ryo.GetPosZ() > 420) {
                velZ = 0;
            }

            if (ryo.GetCollisionInfo().GetCollideZ() == Attributes.CollisionState.NO_COLLISION) { 
                z = (float)Math.Round((double)(velZ + (ryo.GetTossInfo().velocity.Z * 1f)));;
            }
            
            GameManager.Camera.LookAt(gameTime, x, y, z);
        }

        public void Render(GameTime gameTime) {
            GameManager.SpriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        GameManager.SAMPLER_STATE,
                        null,
                        null,
                        null,
                        /*camera.ViewMatrix*//*SpriteScale*/ /*/*camera.ViewMatrix*/GameManager.Camera.ViewMatrix/* ccamera.Transform*/);

            //GraphicsDevice.BlendState =  BlendState.Opaque;
            GameManager.GetInstance().Render(gameTime);
            
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame1), new Vector2(200, 200), Color.White, 0, 1);
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame2), new Vector2(200, 400), Color.White, 0, 1);
            GameManager.GraphicsDevice.BlendState = BlendState.NonPremultiplied;

           // List<CLNS.BoundingBox> targetBoxes = taskMaster.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);

            //entity.HorizontalCollisionLeft(target, 5)

            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            //Entity obs = level1.GetEntities()[1];
            //Entity tp = ryo.GetCollisionInfo().GetItem();
            //Vector2 sx = new Vector2((float)(obs.GetDepthBox().GetRect().X + (obs.GetDepthBox().GetRect().Width / 2)), obs.GetDepthBox().GetRect().Y);

            /*int i = 1;
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                spriteBatch.DrawString(font1, "PRESS: " + key, new Vector2(20, 15*i), Color.Black);
                i++;
            }*/

            //bar.Render();

            //spriteBatch.Draw(sand,Vector2.Zero,Color.White);

            foreach (Line line in lines)
            {
                //line.Draw(spriteBatch);
            }

            GameManager.SpriteBatch.End();

            GameManager.SpriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        GameManager.SAMPLER_STATE,
                        null,
                        null,
                        null,
                        GameManager.Resolution.ViewMatrix);

            //bar.Render();
            //bar2.Render();
            //timer += gameTime.TotalGameTime.Seconds;
            //gg.Draw("" + timer);
            system.Render(gameTime);

            Attributes.CollisionState closeObstacle = GameManager.GetInstance().CollisionManager.FindObstacle(bred);

            font1.Draw("BRED COLL - " + (closeObstacle), new Vector2(80, 100));
            font1.Draw("COLLIDE STATE  - " + bred.GetAiStateMachine().GetCurrentStateId(), new Vector2(80, 150));
            font1.Draw("COLLIDE COMPLETE- " + bred.GetCurrentSprite().IsAnimationComplete(), new Vector2(80, 200));
            //font1.Draw("DEATH - " + level1.Obstacles[1].GetDeathStep(), new Vector2(80, 200));
            //spriteBatch.DrawString(font1, "HEALTH " + (ryo.GetMP()), new Vector2(20, 0), Color.White);
            //spriteBatch.DrawString(font1, "LEVEL MIN Z " + (ryo.GetCurrentSpriteHeight()), new Vector2(20, 50), Color.White);
            //spriteBatch.DrawString(font1, "SCALE " +  bred.GetPosZ(), new Vector2(20, 90), Color.White);
            //spriteBatch.DrawString(font1, "BRED1 " + bred.GetPosY(), new Vector2(20, 130), Color.White);
            /*spriteBatch.DrawString(font1, "BRED2  " + (bred2.GetDepthBox().GetRect().Bottom), new Vector2(20, 180), Color.White);*/
            //spriteBatch.DrawString(testFOnt, "BRED2 GRABBED: " + (ryo.GetCurrentAnimationAction()), new Vector2(20, 100), Color.Red);
            //spriteBatch.DrawString(testFOnt, "ABZ: " +  ryo.GetAbsoluteVelY(), new Vector2(20, 160), Color.Red);

            //spriteBatch.DrawString(font1, "DISTX: " + (distX), new Vector2(20, 80), Color.Blue);
            //spriteBatch.DrawString(font1, "DISTZ: " + (distZ), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "ACTION: " + (ryo.GetKeyboardKey(InputHelper.ActionButton.ATTACK1)), new Vector2(20, 140), Color.Blue);

            //spriteBatch.DrawString(font1, "X - NEW: " + (inputManager.GetInputControl(ryo).currentKeyboardState.IsKeyDown(Keys.X)), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "X - OLD: " + (inputManager.GetInputControl(ryo).oldKeyboardState.IsKeyUp(Keys.X)), new Vector2(20, 140), Color.Blue);


            GameManager.SpriteBatch.End();
        }

        public static Vector2 RotateVector2(Vector2 point, float radians, Vector2 pivot) {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            Vector2 translatedPoint = new Vector2();
            translatedPoint.X = point.X - pivot.X;
            translatedPoint.Y = point.Y - pivot.Y;

            Vector2 rotatedPoint = new Vector2();
            rotatedPoint.X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + pivot.X;
            rotatedPoint.Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + pivot.Y;

            return rotatedPoint;
        }
    }
}
