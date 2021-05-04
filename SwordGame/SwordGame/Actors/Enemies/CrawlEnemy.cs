using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class CrawlEnemy : Enemy
    {
      

        public CrawlEnemy(Level level, Vector2 position, int direction)
            : base(level, position)
        {
            Position = new Vector2(position.X, position.Y + 12);
            this.healthDrop = direction;
            this.direction = direction < 0 ? FaceDirection.Left : FaceDirection.Right;
            this.Health = 10;
            maxHealth = Health;

            LoadContent();
        }

        protected override void LoadContent()
        {
            enemyAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/CrawlEnemy/CrawlEnemy"), 0.1f, 60, true);
            block = level.Content.Load<Texture2D>("square");

            sprite.PlayAnimation(enemyAnimation);
            // Calculate bounds within texture size.            
            int width = (int)(enemyAnimation.FrameWidth);
            int left = (enemyAnimation.FrameWidth - width) / 2;
            int height = (int)(enemyAnimation.FrameHeight);
            int top = enemyAnimation.FrameHeight - height;

            localBounds = new Rectangle(left, top, width, height);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            velocity = Vector2.Zero;

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            // If we are about to run into a wall or off a cliff, start waiting.
            if (level.GetCollision(tileX + (int)direction, tileY) == TileCollision.Impassable
             || level.GetCollision(tileX + (int)direction, tileY + 1) == TileCollision.Passable)
            {
                // Then turn around.
                direction = (FaceDirection)(-(int)direction);
            }
          
            velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
            Position = Position + velocity;

            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            HandleCollisions(gameTime);
        }

        public override void HandleCollisions(GameTime gameTime)
        {
            Rectangle bounds = BoundingRectangle;
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, level.Player.BoundingRectangle);


            if (depth != Vector2.Zero)
            {
                velocity = Vector2.Zero;

                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);


                if (depth.X > 0 && level.Player.StopPlayerCollision == false)
                {
                    if (StopEnemy == false)
                    {
                        level.Rumble.RumbleSetup(500.0f, 0.5f);
                        direction = FaceDirection.Left;
                        level.Player.Strikes -= 1;
                        level.Player.IsHit = true;
                    }
                }
                else if (depth.X < 0 && level.Player.StopPlayerCollision == false)
                {
                    if (StopEnemy == false)
                    {
                        level.Rumble.RumbleSetup(500.0f, 0.5f);
                        direction = FaceDirection.Right;
                        level.Player.Strikes -= 1;
                        level.Player.IsHit = true;
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.PlayAnimation(enemyAnimation);


         
            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, 0 ,flip);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);
        }
    }
}
