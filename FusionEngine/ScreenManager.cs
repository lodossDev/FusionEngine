using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {
    public class ScreenManager {
        private IGameScreen currentScreen;
        private List<IGameScreen> screens;
        private KeyboardState oldKeyboardState, currentKeyboardState;

        public ScreenManager() {
            screens = new List<IGameScreen>();
        }

        public void AddScreen(IGameScreen screen) {
            if (currentScreen != null) {
                currentScreen.Dispose();
                screens.Remove(currentScreen);

                currentScreen = null;
            }

            screens.Add(screen);
            currentScreen = screen;
        }

        public void LoadContent() {
            if (currentScreen != null) { 
                currentScreen.LoadContent();
            }
        }

        public void Update(GameTime gameTime) {
            currentKeyboardState = Keyboard.GetState();

            if (currentScreen != null) { 
                currentScreen.Update(gameTime);
            }

            oldKeyboardState = currentKeyboardState;
        }

        public void Render(GameTime gameTime) {
            if (currentScreen != null) { 
                currentScreen.Render(gameTime);
            }
        }

        public bool IsKeyDown(Keys key) {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key) {
            return (currentKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key));
        }
    }
}
