using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class Health100 : Health
    {
        public Health100(Level level, Vector2 position)
        : base(level, position)
        {
            healthAmount = 2;

            LoadContent();
        }

        protected override void LoadContent()
        {
            texture = level.Content.Load<Texture2D>("Sprites/Pickups/Heart");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);

            boundingRectangle = new Rectangle((int)(Position.X - origin.X), (int)(Position.X - origin.Y),
                            (int)texture.Width, (int)texture.Height);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;

            velocity.Y = MathHelper.Clamp(
                  velocity.Y + GravityAcceleration * elapsed,
                  -MaxFallSpeed,
                  MaxFallSpeed);

            Position += velocity * elapsed;

            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            HandleCollision();
        }

        private void HandleCollision()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = new Rectangle((int)Position.X - (int)origin.X, (int)Position.Y - (int)origin.Y,
                                                  texture.Width, texture.Height);

            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                      // If this tile is collidable,
                    TileCollision collision = level.GetCollision(x, y);

                    if (collision != TileCollision.Passable)
                    {
                        Rectangle tileBounds = level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);

                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX
                             || collision == TileCollision.Platform)
                            {
                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = boundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = boundingRectangle;
                            }

                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color.Blue, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}