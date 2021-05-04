using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SwordGame
{
  public class Camera
  {
        public Vector2 Position = Vector2.Zero;
        private float cameraLerpFactor = 0.15f;
        private const float cameraLerpFactorUp = 0.06f;
        private float multiplyBy = 0;
        float newX;

        public Rectangle CameraTrap
        {
            get { return cameraTrap; }
            set { cameraTrap = value; }
        }
        private Rectangle cameraTrap;


        public float Zoom
        {
            get { return zoom; }
            set { zoom = value; if (zoom < 0.1f) zoom = 0.1f; } // Negative zoom will flip image
        }
        private float zoom = 1.0f;

        public Matrix TransformMatrix
        {
            get
            {
                return Matrix.CreateTranslation(new Vector3((float)Math.Round(-Position.X),(float)Math.Round(-Position.Y) , 0f)) *
                                                Matrix.CreateScale(new Vector3(Zoom, Zoom, 1));
            }
        }

        public void LockToTarget(Vector2 velocity, Rectangle bounds, int screenWidth, int screenHeight)
        {


            if (bounds.Right > cameraTrap.Right)
            {
                multiplyBy = 0.3f;
                cameraTrap.X = bounds.Right - CameraTrap.Width;
            }

            if (bounds.Left < CameraTrap.Left)
            {
                multiplyBy = 0.7f;
                cameraTrap.X = bounds.Left;
            }

            if (bounds.Bottom > CameraTrap.Bottom)
            {
                cameraTrap.Y = bounds.Bottom - CameraTrap.Height;
            }

            if (bounds.Top < CameraTrap.Top)
            {
                cameraTrap.Y = bounds.Top;
            }


            newX = cameraTrap.X + (cameraTrap.Width * multiplyBy) - (screenWidth * multiplyBy);
            Position.X = (int)Math.Round(MathHelper.Lerp(Position.X, newX, cameraLerpFactor));
            Position.Y = (int)Math.Round((double)cameraTrap.Y + (cameraTrap.Height / 2) - (screenHeight / 2));
        }

        public void MoveTrapUp(float target)
        {
          float moveCamera = (int)target - cameraTrap.Height;
          cameraTrap.Y = (int)MathHelper.Lerp((int)CameraTrap.Y, moveCamera, cameraLerpFactorUp);
        }

        public void ClampToArea(int width, int height)
        {
            if (Position.X > width)
                Position.X = width;
            if (Position.Y > height)
                Position.Y = height;

            if (Position.X < 0)
                Position.X = 0;
            if (Position.Y < 0)
                Position.Y = 0;
        }
    }
}
