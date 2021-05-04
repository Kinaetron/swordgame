using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class Health
    {
        protected Texture2D texture;
        protected Vector2 origin;

        // The gem is animated from a base position along the Y axis.
        protected Vector2 basePosition;
        protected float bounce;

        protected Level level;

        public Vector2 Position
        {
            get
            {
                return basePosition /*+ new Vector2(0.0f, bounce)*/;
            }
            set
            {
                basePosition = value /*+ new Vector2(0.0f, bounce)*/;
            }
        }

        // gravity variables 
        protected const float GravityAcceleration = 1000.0f;
        protected const float MaxFallSpeed = 500.0f;

        protected Vector2 velocity;

        public int HealthAmount
        {
            get { return healthAmount; }
        }
        protected int healthAmount;

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, Tile.Width / 3.0f);
            }
        }

        protected Rectangle boundingRectangle;

        public Health(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;
        }

        protected virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
           
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
    }
}
