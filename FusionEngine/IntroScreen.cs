using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace FusionEngine {

    public class IntroScreen : GameScreen
    {
        private ScreenManager screenManager;
        private Entity logo;

        public IntroScreen(ScreenManager manager)
        {
            this.screenManager = manager;
        }

        public override void LoadContent()
        {
            logo = new Entity(Entity.ObjectType.OTHER, "CAPCOM_LOGO");
            logo.AddSprite(Animation.State.NONE, new Sprite("Intro/Sprites/Main"), true);
            logo.SetFrameDelay(Animation.State.NONE, 3);

            logo.SetAnimationState(Animation.State.NONE);
            logo.SetOnLoadScale(3.3f, 3.2f);
            logo.SetPostion(650, 0, 0);

            GameManager.GetInstance().AddEntity(logo);
        }
    }
}
