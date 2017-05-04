using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace FusionEngine {

    public class Camera {
        
        public Camera(Viewport viewport) {
            _viewport = viewport;
            _origin = new Vector2(0, 0);
            _lastPosition = _l2 = Vector2.Zero;
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

        public Vector2 WorldToScreen(Vector2 worldPosition) {
            return Vector2.Transform(worldPosition, ViewMatrix);
        }
 
        public Vector2 ScreenToWorld(Vector2 screenPosition) {
            return Vector2.Transform(screenPosition, Matrix.Invert(ViewMatrix));
        }
        
        public void LookAt(GameTime gameTime, float velX, float velY, float velZ) {
            _lastPosition.X += velX;
            _lastPosition.Y += velY + velZ;

            Vector2 pos1 = Vector2.Lerp(_position, _lastPosition, 1.85f * (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            _position.X = pos1.X;
            _position.Y = pos1.Y;

            if (_position.X < GameManager.GetInstance().CurrentLevel.X_MIN)_position.X = GameManager.GetInstance().CurrentLevel.X_MIN;
            if (_position.X > GameManager.GetInstance().CurrentLevel.X_MAX)_position.X = GameManager.GetInstance().CurrentLevel.X_MAX;

            if (_position.Y < -(GameManager.GetInstance().CurrentLevel.Z_MAX / 2))_position.Y = -(GameManager.GetInstance().CurrentLevel.Z_MAX / 2);
            if (_position.Y > GameManager.GetInstance().CurrentLevel.Z_MIN)_position.Y = GameManager.GetInstance().CurrentLevel.Z_MIN;
        }

        /// <summary>
        /// Calculates a view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix {
            get {
                _viewport = GameManager.GraphicsDevice.Viewport;

                return Matrix.CreateTranslation(new Vector3(-_position.X * _parallax.X, -_position.Y, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-_origin.X, -_origin.Y, 0f)) *
                       (GameManager.Resolution.ViewMatrix * Matrix.CreateScale(_zoom, _zoom, 1f)) * 
                       Matrix.CreateTranslation(new Vector3(_origin.X, _origin.Y, 0f));
            }
        }

        private const float MinZoom = 0.01f;
        private float _zoom = 1f;
        private Viewport _viewport;
        private readonly Vector2 _origin;

        private Vector2 _position;
        private Vector2 _lastPosition;
        private Vector2 _l2;
        private Vector2 _parallax = Vector2.Zero;
    }
}
