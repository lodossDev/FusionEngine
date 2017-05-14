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
            _origin = new Vector2(viewport.Width / 2, viewport.Height / 2);
            _lastPosition = Vector2.Zero;
            _parallax = Vector2.Zero;
            _moveSpeed = 1.35f;
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

        public float MoveSpeed {
            get {
                return _moveSpeed;
            }

            set {
                _moveSpeed = value;
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
        
        public void LookAt(GameTime gameTime, float velX, float velY, float velZ, Entity entity) {
            _lastPosition.X += velX;
            _lastPosition.Y += (velY + velZ);

            if (_zoom > 1.0) {
                Vector2 sx = WorldToScreen(new Vector2((entity.GetPosX() + (entity.GetBoundsBox().GetRect().Width / 2)), (entity.GetPosZ() + (entity.GetBoundsBox().GetRect().Height / 2))));
                _lastPosition.X = sx.X;
                _lastPosition.Y = sx.Y / 2;
            }

            Vector2 pos = Vector2.Lerp(_position, _lastPosition, _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            _position.X = pos.X;
            _position.Y = pos.Y;

            Debug.WriteLine("_lastPosition.Y: " + _lastPosition.Y);

            if (_lastPosition.X < GameManager.GetInstance().CurrentLevel.X_MIN)_lastPosition.X = GameManager.GetInstance().CurrentLevel.X_MIN;
            if (_lastPosition.X > GameManager.GetInstance().CurrentLevel.X_MAX)_lastPosition.X = GameManager.GetInstance().CurrentLevel.X_MAX;

            if (_lastPosition.Y < -(GameManager.GetInstance().CurrentLevel.Z_MAX / 2))_lastPosition.Y = -(GameManager.GetInstance().CurrentLevel.Z_MAX / 2);
            if (_lastPosition.Y > GameManager.GetInstance().CurrentLevel.Z_MIN)_lastPosition.Y = GameManager.GetInstance().CurrentLevel.Z_MIN;
        }

        /// <summary>
        /// Calculates a view matrix for this camera.
        /// </summary>
        public Matrix ViewMatrix {
            get {
                _viewport = GameManager.GraphicsDevice.Viewport;
                _origin.X = 0;
                _origin.Y = 0;

                return Matrix.CreateTranslation(new Vector3(-_position.X * _parallax.X, -_position.Y * _parallax.X, 0f)) *
                       Matrix.CreateTranslation(new Vector3(-_origin.X, -_origin.Y, 0f)) *
                       (GameManager.Resolution.ViewMatrix * Matrix.CreateScale(_zoom, _zoom, 1f)) * 
                       Matrix.CreateTranslation(new Vector3(_origin.X, _origin.Y, 0f));
            }
        }

        private const float MinZoom = 0.01f;
        private float _zoom = 1f;
        private Viewport _viewport;
        public Vector2 _origin;

        private Vector2 _position;
        public Vector2 _lastPosition;
        private Vector2 _parallax;
        private float _moveSpeed;
    }
}
