using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace FusionEngine {

    public static class System {
        private static bool _isPause = false;

        public static void CallPause() {
            _isPause = !_isPause;
        }

        public static bool IsPause() {
            return _isPause;
        }

        public static GraphicsDevice graphicsDevice;
        public static SpriteBatch spriteBatch;
        public static ContentManager contentManager;

        public static readonly int RESOLUTION_X = 1280;
        public static readonly int RESOLUTION_Y = 700;

        public static readonly float GAME_VELOCITY = 60;

        public static float rotate = 0f;
        public static float scaleX, scaleY = 0f;
    }
}
