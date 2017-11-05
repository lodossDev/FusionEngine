using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace FusionEngine
{
    public class MainMenuScreen : GameScreen
    {
        private ScreenManager screenManager;
        private Entity background;
        private Entity modeMenu;
        private Entity arcade;
        private Entity versus;
        private Entity option;
        private Entity exit;
        private Entity select;

        public MainMenuScreen(ScreenManager screenManager)
        {
            this.screenManager = screenManager;
        }

        public override void LoadContent()
        {
            background = new Entity(Entity.ObjectType.OTHER, "BACKGROUND");
            background.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Background"), true);
            background.SetOnLoadScale(4.2f, 4.4f);
            background.SetPostion(650, 0, 0);
            GameManager.GetInstance().AddEntity(background);

            modeMenu = new Entity(Entity.ObjectType.OTHER, "MODE_MENU");
            modeMenu.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Menu"), true);
            modeMenu.SetOnLoadScale(4.2f, 3.8f);
            modeMenu.SetPostion(650, 50, 0);
            GameManager.GetInstance().AddEntity(modeMenu);

            select = new Entity(Entity.ObjectType.OTHER, "SELECT");
            select.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Select"), true);
            select.SetOnLoadScale(2.8f, 3.8f);
            select.SetPostion(700, 255, 0);
            GameManager.GetInstance().AddEntity(select);

            arcade = new Entity(Entity.ObjectType.OTHER, "ARCADE");
            arcade.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Arcade"), true);
            arcade.SetOnLoadScale(4.2f, 3.8f);
            arcade.SetPostion(620, 250, 0);
            GameManager.GetInstance().AddEntity(arcade);

            versus = new Entity(Entity.ObjectType.OTHER, "VERSUS");
            versus.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Versus"), true);
            versus.SetOnLoadScale(4.2f, 3.8f);
            versus.SetPostion(620, 350, 0);
            GameManager.GetInstance().AddEntity(versus);

            option = new Entity(Entity.ObjectType.OTHER, "OPTION");
            option.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Option"), true);
            option.SetOnLoadScale(4.2f, 3.8f);
            option.SetPostion(620, 450, 0);
            GameManager.GetInstance().AddEntity(option);

            exit = new Entity(Entity.ObjectType.OTHER, "EXIT");
            exit.AddSprite(Animation.State.NONE, new Sprite("Main Menu/Sprites/Exit"), true);
            exit.SetOnLoadScale(4.2f, 3.8f);
            exit.SetPostion(620, 550, 0);
            GameManager.GetInstance().AddEntity(exit);
        }

        private void CheckSelected()
        {
            if (select.GetPosY() == 255)
            {
                screenManager.SetScreen("GAME_SCREEN");
            }
        }

        private void PlaySelectedSFX()
        {
            if (select.GetPosY() == 255)
            {
                GameManager.GetInstance().PlaySFX("selected");
            }
            else if(select.GetPosY() == 555)
            {
                GameManager.GetInstance().PlaySFX("menu_exit");
            }
            else
            {
                GameManager.GetInstance().PlaySFX("selected2");
            }
        }

        protected override void Actions(GameTime gameTime)
        {
            if (IsKeyPressed(Keys.Enter))
            {
                PlaySelectedSFX();
                CheckSelected();
            }

            if (IsKeyPressed(Keys.Up))
            {
                GameManager.GetInstance().PlaySFX("selecting");
                select.MoveY(-100);

                if (select.GetPosY() < 255)
                {
                    select.SetPosY(555);
                }
            } 
            else if (IsKeyPressed(Keys.Down))
            {
                GameManager.GetInstance().PlaySFX("selecting");
                select.MoveY(100);

                if (select.GetPosY() > 555)
                {
                    select.SetPosY(255);
                }
            }

            if (select.GetPosY() < 255)
            {
                select.SetPosY(255);
            }
            else if (select.GetPosY() > 555)
            {
                select.SetPosY(555);
            }
        }

        public override void Dispose()
        {
            GameManager.GetInstance().RemoveEntity(background);
            GameManager.GetInstance().RemoveEntity(modeMenu);
            GameManager.GetInstance().RemoveEntity(select);
            GameManager.GetInstance().RemoveEntity(arcade);
            GameManager.GetInstance().RemoveEntity(versus);
            GameManager.GetInstance().RemoveEntity(option);
            GameManager.GetInstance().RemoveEntity(exit);
        }
    }
}
 