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

        public Vector2 ScreenCenter {
            get { return _screenCenter;}
            protected set { _screenCenter = value; }
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

            Vector2 pos = Vector2.Lerp(_position, _lastPosition, _moveSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds);
            
            _position.X = pos.X;
            _position.Y = pos.Y;

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

                return Matrix.CreateTranslation(new Vector3(-_position.X * _parallax.X, -_position.Y * _parallax.X, 0f)) *
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
        private Vector2 _parallax;
        private float _moveSpeed;
        private Vector2 _screenCenter;
    }
}
