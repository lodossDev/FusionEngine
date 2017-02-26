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
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Player leo;
        Entity taskMaster, drum, drum2, drum3, drum4, hitSpark1, ryo;
        RenderManager renderManager;
        CollisionManager collisionManager;
        CLNS.BoundingBox box1;
        SpriteFont font1;
        InputControl control;
        Camera camera;
        float ticks = 0f;
        Stage1 level1;
        LifeBar bar;
        float barHealth = 100f;


        InputManager inputManager;
        InputHelper.CommandMove command;
        static int padCount = 0;

        SpriteRender spriteRender;
        SpriteSheet ryoSheet;
        BitmapFont testFOnt;
        MugenFont gg;
        Vector2 xScroll = Vector2.Zero;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = System.RESOLUTION_X;
            graphics.PreferredBackBufferHeight = System.RESOLUTION_Y;
            //graphics.IsFullScreen = true;
            Content.RootDirectory = "Content";
            Resolution.Update(graphics);
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

            System.graphicsDevice = GraphicsDevice;
            System.contentManager = Content;
            System.spriteBatch = spriteBatch;

            camera = new Camera(GraphicsDevice.Viewport);
            //camera.Parallax = new Vector2(1.0f, 1.0f);
           
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            renderManager = new RenderManager();
            System._renderManager = renderManager;

            //float screenscaleX = (float)500 / 1280;
            //float screenscaleY = (float)300 / 700;
            // Create the scale transform for Draw. 
            // Do not scale the sprite depth (Z=1).
            
            font1 = Content.Load<SpriteFont>("Fonts/Texture");
            testFOnt = Content.Load<BitmapFont>("Fonts/test");

            gg = new MugenFont("Fonts/combo.xFont", new Vector2(200, 200));

            var spriteSheetLoader = new SpriteSheetLoader(Content);
            ryoSheet = spriteSheetLoader.Load("Sprites/Actors/Ryo/ryo.png");
            spriteRender = new SpriteRender(spriteBatch);

            leo = new Player("Leo1");
            taskMaster = new Boss_TaskMaster();
            ryo = new Player_Ryo();

            leo.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Actors/Leo/Stance"));
            leo.SetFrameDelay(Animation.State.STANCE, 5);


            leo.AddSprite(Animation.State.WALK_TOWARDS, new Sprite("Sprites/Actors/Leo/Walk", Animation.Type.REPEAT));
            leo.SetSpriteOffSet(Animation.State.WALK_TOWARDS, 0, 240f);

            leo.AddSprite(Animation.State.JUMP_START, new Sprite("Sprites/Actors/Leo/JumpStart", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.JUMP_START, 20, 10);
            leo.SetFrameDelay(Animation.State.JUMP_START, 5);

            leo.AddSprite(Animation.State.LAND1, new Sprite("Sprites/Actors/Leo/Land", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.LAND1, 30, -11);
            leo.SetFrameDelay(Animation.State.LAND1, 5);

            leo.AddSprite(Animation.State.JUMP, new Sprite("Sprites/Actors/Leo/Jump", Animation.Type.REPEAT, 13));
            leo.SetFrameDelay(Animation.State.JUMP, 5);
            leo.SetSpriteOffSet(Animation.State.JUMP, 10, -80);

            leo.AddSprite(Animation.State.JUMP_TOWARDS, new Sprite("Sprites/Actors/Leo/JumpTowards", Animation.Type.REPEAT, 13));
            leo.SetFrameDelay(Animation.State.JUMP_TOWARDS, 5);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARDS, 1, 6);
            leo.SetSpriteOffSet(Animation.State.JUMP_TOWARDS, 10, -80);

            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_START, Animation.State.JUMP, leo.GetSprite(Animation.State.JUMP_START).GetFrames()));
            leo.SetTossFrame(Animation.State.JUMP, 1);
            leo.SetTossFrame(Animation.State.JUMP_TOWARDS, 1);

            leo.AddSprite(Animation.State.FALL1, new Sprite("Sprites/Actors/Leo/Falling", Animation.Type.REPEAT, 5));
            leo.SetSpriteOffSet(Animation.State.FALL1, 10, -80);

            leo.SetAnimationState(Animation.State.STANCE);
            
            
            leo.SetSpriteOffSet(Animation.State.WALK_TOWARDS, 30, -5);
            leo.SetResetFrame(Animation.State.WALK_TOWARDS, 3);
            leo.SetMoveFrame(Animation.State.WALK_TOWARDS, 3);
           

            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 5);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 1, 6);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 2, 6);
            leo.SetFrameDelay(Animation.State.WALK_TOWARDS, 3, 6);

            leo.AddSprite(Animation.State.ATTACK1, new Sprite("Sprites/Actors/Leo/Attack1", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK1, 40, -35);
            leo.SetFrameDelay(Animation.State.ATTACK1, 4);
            leo.SetFrameDelay(Animation.State.ATTACK1, 1, 5);
            leo.SetFrameDelay(Animation.State.ATTACK1, 2, 5);

            leo.AddBox(Animation.State.ATTACK1, 6, new CLNS.AttackBox(100, 80, 132, 45));
            //leo.GetAttackBox(Animation.State.ATTACK1, 6).SetComboStep(0);

            leo.AddBox(Animation.State.ATTACK1, 6, new CLNS.AttackBox(100, 80, 59, 99, 1));
            leo.AddBox(Animation.State.ATTACK1, 6, new CLNS.AttackBox(100, 80, 159, 99, 1));
            //leo.GetAttackBox(Animation.State.ATTACK1, 6).SetComboStep(1);

            leo.AddBox(Animation.State.ATTACK1, 7, new CLNS.AttackBox(150, 50, 10, 250, 1));
            //leo.AddBox(Animation.State.ATTACK1, 7, new CLNS.AttackBox(150, 50, -60, 190, 1));
            leo.GetAttackBox(Animation.State.ATTACK1, 7).SetComboStep(0);


            leo.AddSprite(Animation.State.ATTACK2, new Sprite("Sprites/Actors/Leo/Attack2", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK2, 25, -37);
            leo.SetFrameDelay(Animation.State.ATTACK2, 4);
            leo.AddBox(Animation.State.ATTACK2, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK2, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK2, 6, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK3, new Sprite("Sprites/Actors/Leo/Attack3", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK3, 60, -10);
            leo.SetFrameDelay(Animation.State.ATTACK3, 4);
            leo.AddBox(Animation.State.ATTACK3, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK3, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK3, 5, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK4, new Sprite("Sprites/Actors/Leo/Attack4", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK4, 30, 3);
            leo.SetFrameDelay(Animation.State.ATTACK4, 4);
            leo.AddBox(Animation.State.ATTACK4, 3, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK4, 5, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK5, new Sprite("Sprites/Actors/Leo/Attack5", Animation.Type.ONCE));
            leo.SetSpriteOffSet(Animation.State.ATTACK5, 50, -25);
            leo.SetFrameDelay(Animation.State.ATTACK5, 4);
            leo.AddBox(Animation.State.ATTACK5, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK5, 6, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.ATTACK6, new Sprite("Sprites/Actors/Leo/Attack6", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.ATTACK6, 35, -41);
            leo.SetFrameDelay(Animation.State.ATTACK6, 4);
            leo.AddBox(Animation.State.ATTACK6, 5, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 6, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 5, new CLNS.AttackBox(220, 230, 20, 30));
            leo.AddBox(Animation.State.ATTACK6, 5, new CLNS.AttackBox(220, 230, 20, 30));

            leo.AddSprite(Animation.State.JUMP_ATTACK1, new Sprite("Sprites/Actors/Leo/JumpAttack1", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.JUMP_ATTACK1, 60, -70);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 4);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 1, 4);
            leo.SetFrameDelay(Animation.State.JUMP_ATTACK1, 2, 4);

            leo.AddSprite(Animation.State.JUMP_TOWARD_ATTACK1, new Sprite("Sprites/Actors/Leo/JumpAttack2", Animation.Type.ONCE), true);
            leo.SetSpriteOffSet(Animation.State.JUMP_TOWARD_ATTACK1, 30, -60);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 4);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 1, 4);
            leo.SetFrameDelay(Animation.State.JUMP_TOWARD_ATTACK1, 2, 4);

            leo.SetTossFrame(Animation.State.JUMP_ATTACK1, 1);
            leo.SetTossFrame(Animation.State.JUMP_TOWARD_ATTACK1, 1);

            leo.AddSprite(Animation.State.JUMP_RECOVER1, new Sprite("Sprites/Actors/Leo/JumpRecover1", Animation.Type.REPEAT, 3));
            leo.SetSpriteOffSet(Animation.State.JUMP_RECOVER1, 20, -80);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 5);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 1, 6);
            leo.SetFrameDelay(Animation.State.JUMP_RECOVER1, 2, 6);

            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_ATTACK1, Animation.State.JUMP_RECOVER1, 8));
            leo.AddAnimationLink(new Animation.Link(Animation.State.JUMP_TOWARD_ATTACK1, Animation.State.JUMP_RECOVER1, 9));

            leo.SetDefaultAttackChain(new ComboAttack.Chain(new List<ComboAttack.Move>{
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK1, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 4),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 4),
                new ComboAttack.Move(Animation.State.ATTACK4, 222000, 4),
                new ComboAttack.Move(Animation.State.ATTACK2, 222000, 8),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK3, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK5, 222000, 7),
                new ComboAttack.Move(Animation.State.ATTACK6, 222000, 8)
            }));

            /*leo.SetFrameDelay(Animation.State.ATTACK1, 1);
            leo.SetFrameDelay(Animation.State.ATTACK2, 1);
            leo.SetFrameDelay(Animation.State.ATTACK3, 1);
            leo.SetFrameDelay(Animation.State.ATTACK4, 1);
            leo.SetFrameDelay(Animation.State.ATTACK5, 1);
            leo.SetFrameDelay(Animation.State.ATTACK6, 1);*/

            leo.AddBoundsBox(125, 283, -30, 80, 50);
            leo.SetOnLoadScale(1.6f, 2.2f);
            leo.SetPostion(400, 0, 200);


            drum = new Entity(Entity.ObjectType.OBSTACLE, "DRUM1");
            drum.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum/Stance"));
            drum.SetAnimationState(Animation.State.STANCE);
            //drum.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 125, 210, -63, -15));
            drum.AddBoundsBox(125, 210, -63, -15, 40);
            drum.SetOnLoadScale(2.2f, 2.6f);
            drum.SetPostion(700, 0, 400);

            drum2 = new Entity(Entity.ObjectType.OBSTACLE, "DRUM2");
            drum2.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum/Stance"));
            drum2.SetAnimationState(Animation.State.STANCE);
            //drum2.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 125, 210, -63, -15));
            drum2.AddBoundsBox(125, 210, -63, -15, 40);
            drum2.SetOnLoadScale(2.2f, 2.6f);

            drum2.SetPostion(500, 0, 200);
           

            drum3 = new Entity(Entity.ObjectType.OBSTACLE, "DRUM3");
            drum3.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum/Stance"));
            drum3.SetAnimationState(Animation.State.STANCE);
            //drum3.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 125, 210, -63, -15));
            drum3.AddBoundsBox(125, 210, -63, -15, 40);
            drum3.SetOnLoadScale(2.2f, 2.6f);
            drum3.SetPostion(100, 0, 200);

            drum4 = new Entity(Entity.ObjectType.OBSTACLE, "DRUM4");
            drum4.AddSprite(Animation.State.STANCE, new Sprite("Sprites/Misc/Drum/Stance"));
            drum4.SetAnimationState(Animation.State.STANCE);

            //drum4.AddBox(new CLNS.BoundingBox(CLNS.BoxType.BODY_BOX, 125, 210, -63, -15));
            drum4.AddBoundsBox(125, 210, -63, -15, 40);

            drum2.SetOnLoadScale(2.2f, 2.6f);
            drum2.SetPostion(500, -280, 200);
            drum2.SetGroundBase(-280);
           
            leo.SetOnLoadScale(1.8f, 2.6f);

            level1 = new Stage1();
            bar = new LifeBar(0, 0);

           

            //renderManager.AddEntity(leo);
            renderManager.AddEntity(taskMaster);
            renderManager.AddEntity(drum);
            renderManager.AddEntity(drum2);
            renderManager.AddEntity(drum3);
            //renderManager.AddEntity(drum4);
            renderManager.AddLevel(level1);
            //renderManager.AddEntity(hitSpark1);

            collisionManager = new CollisionManager(renderManager);
            //collisionManager.AddEntity(leo);
            collisionManager.AddEntity(taskMaster);
            collisionManager.AddEntity(drum);
            collisionManager.AddEntity(drum2);
            collisionManager.AddEntity(drum3);
            //collisionManager.AddEntity(drum4);


            collisionManager.AddEntity(ryo);
            collisionManager.AddEntity(level1.GetMisc()[0]);
            renderManager.AddEntity(ryo);

           
            leo.SetAnimationState(Animation.State.STANCE);
            leo.SetBaseOffset(-60, -30f);
            //control = new InputControl(leo, PlayerIndex.One);

            inputManager = new InputManager();
            inputManager.AddControl(ryo, PlayerIndex.One);

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
                System.CallPause();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.T) && oldKeyboardState.IsKeyUp(Keys.T)) {
                level1.GetMisc()[0].SetAnimationState(Animation.State.DIE1);
                level1.GetMisc()[0].GetCurrentSprite().SetCurrentFrame(2);
            }

            if (currentKeyboardState.IsKeyDown(Keys.Q) && oldKeyboardState.IsKeyUp(Keys.Q)) {
                renderManager.RenderBoxes();
            }

            
            if (Keyboard.GetState().IsKeyDown(Keys.Z))
            {
                ryo.SetAnimationState(Animation.State.THROW1);
                //Setup.rotate += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleY += 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //barHealth -= (50.05f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //leo.SetColor(255, 0, 0);
                //leo.Flash(2);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.X))
            {
                //Setup.rotate -= 2.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleY += 5.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //Setup.scaleX -= 5.5f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //barHealth += (50.05f * (float)gameTime.ElapsedGameTime.TotalSeconds);
                //xScroll.X += -1 * (1 * (float)gameTime.ElapsedGameTime.TotalSeconds);

                //camera.Parallax = new Vector2(xScroll.X, 0);
                ryo.SetScale(5, 5);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.N))
            {
                camera.Zoom += 0.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width / 2, Setup.graphicsDevice.Viewport.Height / 2);
                //Vector2 pos = new Vector2(-(camera.Zoom * 3f), 0);
                //camera.Move(pos);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.M))
            {
                camera.Zoom -= 0.2f * (float)gameTime.ElapsedGameTime.TotalSeconds;
                //camera._origin = new Vector2(Setup.graphicsDevice.Viewport.Width/2, Setup.graphicsDevice.Viewport.Height/2);
                //Vector2 pos = new Vector2((camera.Zoom * 3f), 0);
                //camera.Move(pos);
            }

            bar.Percent((int)barHealth);

            if (Keyboard.GetState().IsKeyDown(Keys.J))
            {
                if (Keyboard.GetState().IsKeyDown(Keys.K))
                {
                    level1.GetMisc()[0].Toss(-15, -5);
                }
                else if (Keyboard.GetState().IsKeyDown(Keys.L))
                {
                    level1.GetMisc()[0].Toss(-15, 5);
                }
                else
                {
                    level1.GetMisc()[0].Toss(-15);
                }
            }

            /*if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                drum3.VelX(-0.03f);
                //drum3.SetIsLeft(true);
                taskMaster.SetAnimationState(Animation.State.ATTACK2);
            }
            else if (Keyboard.GetState().IsKeyDown(Keys.L))
            {
                drum3.VelX(0.03f);
                //drum3.SetIsLeft(false);
            }
            else
            {
                drum3.VelX(0);
            }*/

            /*if (Keyboard.GetState().IsKeyDown(Keys.U))
            {
                level1.ScrollX(-5/2f);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Y))
            {
                level1.ScrollX(5/2f);
            } */

            if (!System.IsPause())
            {
                //control.Update(gameTime);
               
                //Toss needs to be updated before collision
                /*leo.Update(gameTime);
                leo2.Update(gameTime);
                drum.Update(gameTime);
                drum2.Update(gameTime);
                drum3.Update(gameTime);
                drum4.Update(gameTime);
                */

                inputManager.Update(gameTime);

                collisionManager.Update(gameTime);

                if (Keyboard.GetState().IsKeyDown(Keys.NumPad4)) {
                    drum3.MoveX(5, -1);
                    //level1.ScrollX(-12);
                } else if (Keyboard.GetState().IsKeyDown(Keys.NumPad6)) {
                    drum3.MoveX(5, 1);
                    //level1.ScrollX(12);
                } else {
                    drum3.MoveX(0, 0);
                    drum3.VelX(0);
                }

                if(Keyboard.GetState().IsKeyDown(Keys.NumPad8)) {
                    drum3.MoveZ(5, -1);
                    //level1.ScrollY(-5);
                } else if(Keyboard.GetState().IsKeyDown(Keys.NumPad2)) {
                    //ryo.MoveX(5, 1);
                    drum3.MoveZ(5, 1);
                    //level1.ScrollY(5);
                } else {
                    drum3.MoveZ(0, 0);
                    drum3.VelZ(0);
                }


                if ((int)drum.GetPosY() >= 0) {
                    dir = -1 * dir;

                } else if ((int)drum.GetPosY() <= -500)
                {
                    dir = -dir;
                }

                drum.MoveY(0.3f * dir);
                //level1.ScrollY(leo.GetVelocity().Y/2);

                if (taskMaster.GetGrabInfo().isGrabbed == false && !taskMaster.IsToss()) {
                    //((Character)taskMaster).UpdateAI(gameTime, collisionManager.GetPlayers());
                    ((Character)taskMaster).ResetToIdle(gameTime);
                }

                renderManager.Update(gameTime);

                /*level1.ScrollX(-leo.GetVelocity().X);
                drum.MoveX(-leo.GetVelocity().X);
                drum2.MoveX(-leo.GetVelocity().X);
                drum3.MoveX(-leo.GetVelocity().X);
                drum4.MoveX(-leo.GetVelocity().X);*/
            }

            // TODO: Add your update logic here
            bar.Update(gameTime);
            camera.LookAt(ryo.GetConvertedPosition());

            oldKeyboardState = currentKeyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            //GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            // TODO: Add your drawing code here
            spriteBatch.Begin(SpriteSortMode.Immediate,
                        BlendState.NonPremultiplied,
                        System.SAMPLER_STATE,
                        null,
                        null,
                        null,
                        /*camera.ViewMatrix*//*SpriteScale*/ /*Resolution.Scale*/camera.ViewMatrix);

            //GraphicsDevice.BlendState =  BlendState.Opaque;
            renderManager.Draw(gameTime);
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame1), new Vector2(200, 200), Color.White, 0, 1);
            //spriteRender.Draw(ryoSheet.Sprite(TexturePackerMonoGameDefinitions.Ryo.Attack4_Frame2), new Vector2(200, 400), Color.White, 0, 1);
            GraphicsDevice.BlendState = BlendState.NonPremultiplied;

            List<CLNS.BoundingBox> targetBoxes = taskMaster.GetCurrentBoxes(CLNS.BoxType.BODY_BOX);

            //entity.HorizontalCollisionLeft(target, 5)

            GamePadCapabilities capabilities = GamePad.GetCapabilities(PlayerIndex.One);

            /*spriteBatch.DrawString(font1, "IS AIR: " + (ryo.InAir()), new Vector2(20, 20), Color.Blue);
             * 
             * Vector
            */

           
            //gg.Draw("077128 000\nh878 78787\n343525 23432");
            spriteBatch.DrawString(font1, "TASK OBS: " + (ryo.GetAfterImageData().imageData.Count), new Vector2(20, 50), Color.White);
            spriteBatch.DrawString(testFOnt, "FRAME: " + (ryo.GetCurrentFrame() + 1), new Vector2(20, 100), Color.Red);
            spriteBatch.DrawString(testFOnt, "PHONE X: " +  level1.GetMisc()[0].GetCurrentSprite().GetCurrentScaleFrame().Y, new Vector2(20, 160), Color.Red);

            //spriteBatch.DrawString(font1, "DISTX: " + (distX), new Vector2(20, 80), Color.Blue);
            //spriteBatch.DrawString(font1, "DISTZ: " + (distZ), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "ACTION: " + (ryo.GetKeyboardKey(InputHelper.ActionButton.ATTACK1)), new Vector2(20, 140), Color.Blue);

            //spriteBatch.DrawString(font1, "X - NEW: " + (inputManager.GetInputControl(ryo).currentKeyboardState.IsKeyDown(Keys.X)), new Vector2(20, 110), Color.Blue);
            //spriteBatch.DrawString(font1, "X - OLD: " + (inputManager.GetInputControl(ryo).oldKeyboardState.IsKeyUp(Keys.X)), new Vector2(20, 140), Color.Blue);


            /*int i = 1;
            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                spriteBatch.DrawString(font1, "PRESS: " + key, new Vector2(20, 15*i), Color.Black);
                i++;
            }*/
           
            //bar.Render();
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
