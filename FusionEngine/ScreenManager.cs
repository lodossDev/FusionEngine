using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine 
{
    public class ScreenManager 
    {
        private IGameScreen currentScreen;
        private Dictionary<string, IGameScreen> screens;
        private string lastScreenName;

        public ScreenManager() 
        {
            screens = new Dictionary<string, IGameScreen>();
        }

        public void AddScreen(string name, IGameScreen screen) 
        {
            screens.Add(name, screen);
        }

        public void SetScreen(string name)
        {
            if (lastScreenName != null)
            {
                currentScreen = screens[lastScreenName];
                currentScreen.Dispose();
                screens.Remove(lastScreenName);
            }

            lastScreenName = name;
            currentScreen = screens[name];
            currentScreen.LoadContent();
        }

        public void Update(GameTime gameTime) {
            if (currentScreen != null) { 
                currentScreen.Update(gameTime);
            }
        }

        public void Render(GameTime gameTime) {
            if (currentScreen != null) { 
                currentScreen.Render(gameTime);
            }
        }
    }
}
