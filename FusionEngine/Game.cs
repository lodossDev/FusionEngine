using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using TexturePackerLoader;

namespace FusionEngine
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player leo;
        Entity taskMaster, drum, drum2, drum4, hitSpark1, ryo;
        Drum drum3;
        CLNS.BoundingBox box1;
        SpriteFont font1;
        InputControl control;
        Camera camera;
        float ticks = 0f;
        Stage1 level1;
        LifeBar bar;
        float barHealth = 100f;
        Enemy_Bred bred, bred2;

        InputHelper.CommandMove command;
        static int padCount = 0;

        SpriteRender spriteRender;
        SpriteSheet ryoSheet;
        BitmapFont testFOnt;
        MugenFont gg;
        Vector2 xScroll = Vector2.Zero;

        public static Texture2D sand;
        Vector2 x_finder;
        List<Line> lines;
        float x = 0, y = 0;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = GameManager.RESOLUTION_X;
            graphics.PreferredBackBufferHeight = GameManager.RESOLUTION_Y;

            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            GameManager.SetupDevice(graphics, Content, spriteBatch);
            GameManager.SetupCamera();

            //camera = new Camera(GraphicsDevice.Viewport);
            //camera.Parallax = new Vector2(0.8f, 0.8f);
            
            base.Initialize();
        }

        public static Vector2 RotateVector2(Vector2 point, float radians, Vector2 pivot)
        {
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


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            //float screenscaleX = (float)500 / 1280;
            //float screenscaleY = (float)300 / 700;
            // Create the scale transform for Draw. 
            // Do not scale the sprite depth (Z=1).

            ryo = new Player_Ryo();

            sand = Content.Load<Texture2D>("Sprites//sand");

            lines    = new List<Line>();

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
            
            font1 = Content.Load<SpriteFont>("Fonts/Texture");
            testFOnt = Content.Load<BitmapFont>("Fonts/test");

            gg = new MugenFont("Fonts/combo.xFont", new Vector2(200, 200));

            bred = new Enemy_Bred();
            bred2 = new Enemy_Bred();

            //var spriteSheetLoader = new SpriteSheetLoader(Content);
            //ryoSheet = spriteSheetLoader.Load("Sprites/Actors/Ryo/ryo.png");
            //spriteRender = new SpriteRender(spriteBatch);

            level1 = new Stage1();
            bar = new LifeBar(0, 0);

            bred2.SetPostion(600, 0, 600);
           
            //renderManager.AddEntity(leo);
            //renderManager.AddEntity(taskMaster);
            GameManager.GetInstance().AddEntity(ryo);
            GameManager.GetInstance().AddEntity(drum);
            GameManager.GetInstance().AddEntity(drum2);
            GameManager.GetInstance().AddEntity(drum3);
            //renderManager.AddEntity(drum4);
            GameManager.GetInstance().SetLevel(level1);
            //renderManager.AddEntity(hitSpark1);
            GameManager.GetInstance().AddEntity(bred);
            GameManager.GetInstance().AddEntity(bred2);
           
            bred2.SetName("BRED2");
            //control = new InputControl(leo, PlayerIndex.One);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private KeyboardState oldKeyboardState, currentKeyboardState;
        private int dir = 10;

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            currentKeyboardState = Keyboard.GetState();
            GamePadState padState = GamePad.GetState(PlayerIndex.One);

            if (Keyboard.GetState().IsKeyDown(Keys.P) && oldKeyboardState.IsKeyUp(Keys.P)) {
                GameManager.PauseGame();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T) && oldKeyboardState.IsKeyUp(Keys.T)) {
                //level1.GetEntities()[0].SetAnimationState(Animation.State.DIE1);
                //level1.GetEntities()[0].GetCurrentSprite().SetCurrentFrame(2);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Q) && oldKeyboardState.IsKeyUp(Keys.Q)) {
                GameManager.GetInstance().RenderManager.RenderBoxes();
            }

            if (currentKeyboardState.IsKeyDown(Keys.NumPad6)) {
                x += 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (currentKeyboardState.IsKeyDown(Keys.NumPad4)) {
                x -= 5 * (float)gameTime.ElapsedGameTime.TotalSeconds;
            } else {
                x = 0;
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                //level1.GetEntities()[0].SetAnimationState(Animation.State.DIE1);
                //GameManager.GetInstance().GetRenderManager().GetLevels()[0].GetEntities()[1].SetAliveTime(55);
                //ryo.SetAnimationState(Animation.State.PICKUP1);
                //Setup.rotate += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleY += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //barHealth -= (50.05f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //leo.SetColor(255, 0, 0);
                //leo.Flash(2);
                bred.SetAnimationState(Animation.State.BLOCK1);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X)) {
                GameManager.TakeScreenshot(this);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                GameManager.Camera.Zoom += 0.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width / 2, Setup.graphicsDevice.Viewport.Height / 2);
                //Vector2 pos = new Vector2(-(camera.Zoom * 3f), 0);
                //camera.Move(pos);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                GameManager.Camera.Zoom -= 0.8f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width/2, Setup.graphicsDevice.Viewport.Height/2);
                //Vector2 pos = new Vector2((camera.Zoom * 3f), 0);
                //camera.Move(pos);
            }

            bar.Percent((int)barHealth);

            if (!GameManager.IsPause())
            {
                if (bred.GetGrabInfo().isGrabbed == false 
                        && !bred.IsToss() 
                        && !bred.IsInAnimationAction(Animation.Action.INPAIN)
                        && !bred.IsInAnimationAction(Animation.Action.KNOCKED)
                        //&& !bred.IsInAnimationAction(Animation.Action.RISING)
                        && !bred.IsRise()
                        && !bred.InHitPauseTime()
                        && bred.GetHealth() > 0
                        && !bred.IsDying()) {

                    if(!bred.IsInAnimationAction(Animation.Action.RISING))bred.UpdateAI(gameTime, GameManager.GetInstance().Players);
                    bred.ResetToIdle(gameTime);
                }

                if (bred2.GetGrabInfo().isGrabbed == false 
                        && !bred2.IsToss() 
                        && !bred2.IsInAnimationAction(Animation.Action.INPAIN)
                        && !bred2.IsInAnimationAction(Animation.Action.KNOCKED)
                        //&& !bred2.IsInAnimationAction(Animation.Action.RISING)
                        && !bred2.IsRise()
                        && !bred2.InHitPauseTime()
                        && bred2.GetHealth() > 0
                         && !bred2.IsDying()) {

                    if(!bred2.IsInAnimationAction(Animation.Action.RISING))bred2.UpdateAI(gameTime, GameManager.GetInstance().Players);
                    bred2.ResetToIdle(gameTime);
                }

                if (bred.IsInAnimationAction(Animation.Action.INPAIN)) {
                    bred.StopMovement();
                }

                if (bred2.IsInAnimationAction(Animation.Action.INPAIN)) {
                    bred2.StopMovement();
                }
                
                GameManager.GetInstance().Update(gameTime);
            }

            // TODO: Add your update logic here
            bar.Update(gameTime);
            
            GameManager.Camera.LookAt(x, y);

            foreach (Line line in lines)
            {
                line.Update();
            }

            oldKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        public void Render(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        GameManager.SAMPLER_STATE,
                        null,
                        null,
                        null,
                        /*camera.ViewMatrix*//*SpriteScale*/ /*/*camera.ViewMatrix*/GameManager.Camera.ViewMatrix);

            //GraphicsDevice.BlendState =  BlendState.Opaque;
            GameManager.GetInstance().Render(gameTime);
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame1), new Vector2(200, 200), Color.White, 0, 1);
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame2), new Vector2(200, 400), Color.White, 0, 1);
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

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

            spriteBatch.End();

            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        GameManager.SAMPLER_STATE,
                        null,
                        null,
                        null,
                        GameManager.Resolution.ViewMatrix);

            Vector2 pos = Vector2.Transform(ryo.GetConvertedPosition(), GameManager.Camera.ViewMatrix);
            float viewPort = (GameManager.Camera.ViewPort.Width);

            //gg.Draw("077128 000\nh878 78787\n343525 23432");
            spriteBatch.DrawString(font1, "RYO Z1 " + (GameManager.Camera.Position), new Vector2(20, 0), Color.White);
            //spriteBatch.DrawString(font1, "RYO POS1 " + (pos.X), new Vector2(20, 50), Color.White);
            spriteBatch.DrawString(font1, "BRED Z2 " +  bred.GetCurrentAnimationState(), new Vector2(20, 90), Color.White);
            //spriteBatch.DrawString(font1, "BRED1 " + bred.GetPosY(), new Vector2(20, 130), Color.White);
            /*spriteBatch.DrawString(font1, "BRED2  " + (bred2.GetDepthBox().GetRect().Bottom), new Vector2(20, 180), Color.White);*/
            //spriteBatch.DrawString(testFOnt, "BRED2 GRABBED: " + (ryo.GetCurrentAnimationAction()), new Vector2(20, 100), Color.Red);
            //spriteBatch.DrawString(testFOnt, "ABZ: " +  ryo.GetAbsoluteVelY(), new Vector2(20, 160), Color.Red);

            //spriteBatch.DrawString(font1, "DISTX: " + (distX), new Vector2(20, 80), Color.Blue);
            //spriteBatch.DrawString(font1, "DISTZ: " + (distZ), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "ACTION: " + (ryo.GetKeyboardKey(InputHelper.ActionButton.ATTACK1)), new Vector2(20, 140), Color.Blue);

            //spriteBatch.DrawString(font1, "X - NEW: " + (inputManager.GetInputControl(ryo).currentKeyboardState.IsKeyDown(Keys.X)), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "X - OLD: " + (inputManager.GetInputControl(ryo).oldKeyboardState.IsKeyUp(Keys.X)), new Vector2(20, 140), Color.Blue);


            spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Render(gameTime);
        }
    }
}
