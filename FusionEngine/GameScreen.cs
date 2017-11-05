using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace FusionEngine
{
    public abstract class GameScreen : IGameScreen
    {
        protected KeyboardState oldKeyboardState, currentKeyboardState;

        public virtual void LoadContent()
        {
        }

        protected virtual void Actions(GameTime gameTime)
        {
        }

        public virtual void Update(GameTime gameTime)
        {
            currentKeyboardState = Keyboard.GetState();

            Actions(gameTime);
            GameManager.GetInstance().Update(gameTime);

            oldKeyboardState = currentKeyboardState;
        }

        protected virtual void DrawBack(GameTime gameTime)
        {
        }

        protected virtual void DrawMain(GameTime gameTime)
        {
            GameManager.GetInstance().Render(gameTime);
        }

        protected virtual void DrawFront(GameTime gameTime)
        {
        }

        public virtual void Render(GameTime gameTime)
        {
            GameManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, GameManager.SAMPLER_STATE, null, null, null, GameManager.Camera.ViewMatrix);
                DrawBack(gameTime);
                DrawMain(gameTime);
            GameManager.SpriteBatch.End();

            GameManager.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, GameManager.SAMPLER_STATE, null, null, null, Resolution.getTransformationMatrix());
                DrawFront(gameTime);
            GameManager.SpriteBatch.End();
        }

        public virtual void Dispose()
        {
        }

        public bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        public bool IsKeyPressed(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) && oldKeyboardState.IsKeyUp(key));
        }
    }
}
