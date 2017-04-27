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

        public Vector2 WorldToScreen(Vector2 worldPosition) {
            return Vector2.Transform(worldPosition, ViewMatrix);
        }
 
        public Vector2 ScreenToWorld(Vector2 screenPosition) {
            return Vector2.Transform(screenPosition, Matrix.Invert(ViewMatrix));
        }
        
        public void LookAt(Entity entity) {
            float velX = (entity.GetAccelX() / GameManager.GAME_VELOCITY) * entity.GetDirX();

            if (entity.GetCollisionInfo().GetCollideX() == Attributes.CollisionState.NO_COLLISION) { 
                _position.X += velX + (entity.GetTossInfo().velocity.X / 2);
            }

            if (_position.X < -30 * (0.8 / _parallax.X))_position.X = -30 * (0.8f / _parallax.X);
            if (_position.X > 8400 * (0.8 / _parallax.X))_position.X = 8400 * (0.8f / _parallax.X);
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
