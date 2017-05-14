using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FusionEngine {

    public class Resolution {
        public Vector3 _scalingFactor;
        private int _preferredBackBufferWidth;
        private int _preferredBackBufferHeight;

        /// <summary>
        /// The virtual screen size. Default is 1280x800. See the non-existent documentation on how this works.
        /// </summary>
        private Vector2 _virtualScreen;

        /// <summary>
        /// The screen scale
        /// </summary>
        private Vector2 _screenAspectRatio;

        /// <summary>
        /// The scale used for beginning the SpriteBatch.
        /// </summary>
        private Matrix _scale;

        /// <summary>
        /// The scale result of merging VirtualScreen with WindowScreen.
        /// </summary>
        public Vector2 _screenScale;

        
        public Resolution(int width, int height) {
            _virtualScreen = new Vector2(width, height);
            _screenAspectRatio = new Vector2(1, 1);
        }

        /// <summary>
        /// Updates the specified graphics device to use the configured resolution.
        /// </summary>
        /// <param name="device">The device.</param>
        /// <exception cref="Globals.ArgumentNullException">device</exception>
        public void Update(GraphicsDeviceManager device) {
            if (device == null) throw new ArgumentNullException("device");

            //Calculate ScalingFactor
            _preferredBackBufferWidth = device.PreferredBackBufferWidth;
            float widthScale = _preferredBackBufferWidth / _virtualScreen.X;

            _preferredBackBufferHeight = device.PreferredBackBufferHeight;
            float heightScale = _preferredBackBufferHeight / _virtualScreen.Y;

            _screenScale = new Vector2(widthScale, heightScale);

            _screenAspectRatio = new Vector2(widthScale / heightScale);
            _scalingFactor = new Vector3(widthScale, heightScale, 1);
            _scale = Matrix.CreateScale(_scalingFactor);
            device.ApplyChanges();
        }

        public Matrix ViewMatrix {
            get {
                return _scale;
            }
        }

        public Vector2 VirtualScreen {
            get {
                return _virtualScreen;
            }
        }

        /// <summary>
        /// <para>Determines the draw scaling.</para>
        /// <para>Used to make the mouse scale correctly according to the virtual resolution,
        /// no matter the actual resolution.</para>
        /// <para>Example: 1920x1080 applied to 1280x800: new Vector2(1.5f, 1,35f)</para>
        /// </summary>
        /// <returns></returns>
        public Vector2 DetermineDrawScaling() {
            var x = _preferredBackBufferWidth / _virtualScreen.X;
            var y = _preferredBackBufferHeight / _virtualScreen.Y;
            return new Vector2(x, y);
        }
    }
}
