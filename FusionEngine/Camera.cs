using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FusionEngine {

    public class Camera {
        public Vector2 lastPosition = Vector2.Zero;

        public Camera(Viewport viewport) {
            _viewport = viewport;
            _origin = new Vector2(0, 0);
            _position = Vector2.Zero;
        }

        /// <summary>
        /// Gets or sets the position of the camera.
        /// </summary>
        public Vector2 Position {
            get {
                return _position;
            }

            set {
                _position = value;
            }
        }

        public Vector2 Parallax {
            get {
                return _parallax;
            }

            set {
                _parallax = value;
            }
        }

        /// <summary>
        /// Gets or sets the zoom of the camera.
        /// </summary>
        public float Zoom {
            get {
                return _zoom;
            }

            set {
                _zoom = MathHelper.Max(value, MinZoom);
            }
        }

        public Viewport ViewPort {
            get {
                return _viewport;
            }
        }

        public void SetParalax(float x, float y) {
            _parallax.X = x;
            _parallax.Y = y;
        }

        public void SetPosition(float x, float y) {
            _position.X = x;
            _position.Y = y;
        }
        
        public void LookAt(Vector2 position) {
            //Debug.WriteLine("VIEWPORT: " + _viewport.Width);
            Vector2 screenPosition = Vector2.Transform(position, ViewMatrix);

            Vector2 max = Vector2.Transform(new Vector2(ViewPort.Width - 40, 0), Matrix.Invert(ViewMatrix));
            Vector2 min = Vector2.Transform(new Vector2(40, 0), Matrix.Invert(ViewMatrix));

            if (screenPosition.X < ViewPort.Width - 40) {
                _position = (position - lastPosition) - new Vector2(GameManager.GetResolution().VirtualScreen.X / 2, 0);
            } else {
                _position.X += 10f;
                lastPosition = _position;
            }

            if (_position.X < -30)_position.X = -30;
            if (_position.X > 8400)_position.X = 8400;
        }

        /// <summary>
        /// Calculates a view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix {
            get {
                return Matrix.CreateTranslation(new Vector3(-_position.X * _parallax.X, 0f * _parallax.Y, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-_origin.X, -_origin.Y, 0f)) *
                       (GameManager.GetResolution().ViewMatrix * Matrix.CreateScale(_zoom, _zoom, 1f)) * 
                       Matrix.CreateTranslation(new Vector3(_origin.X, _origin.Y, 0f));
            }
        }

        private const float MinZoom = 0.01f;

        private readonly Viewport _viewport;
        private readonly Vector2 _origin;

        private Vector2 _position;
        private float _zoom = 1f;
        private Vector2 _parallax = Vector2.Zero;
    }
}
