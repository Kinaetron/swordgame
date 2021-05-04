using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{

    class FlyingEnemy : Enemy
    {
        public Circle EncounterRadius
        {
            get { return new Circle(new Vector2(Position.X, Position.Y + 170), 300); }
        }

        private Vector2 startingPoint;

        public FlyingEnemy(Level level, Vector2 position, int lootDrop)
            : base(level, position)
        {


            Position = new Vector2(position.X, position.Y);
            this.healthDrop = lootDrop;
            startingPoint = Position;
            this.Health = 20;
            maxHealth = Health;

            LoadContent();
        }

        protected override void LoadContent()
        {
            enemyAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/FlyingEnemy/FlyingEnemy"), 0.1f, 40, true);
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
            Rectangle bounds = BoundingRectangle;

            HandleCollisions();

            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, level.Player.BoundingRectangle);

            if (EncounterRadius.Intersects(level.Player.BoundingRectangle) &&
                level.Player.StopPlayerCollision == false && 
                level.Player.IsOnLadder == false && 
                wallInWay == false) 
            {
                startChase = true;
            }

            if (depth != Vector2.Zero)
            {
                float absDepthX = Math.Abs(depth.X);
                float absDepthY = Math.Abs(depth.Y);

                if (/*startTimer == false &&*/ level.Player.StopPlayerCollision == false)
                {
                    if (StopEnemy == false)
                    {
                        startChase = false;
                        level.Rumble.RumbleSetup(500.0f, 0.5f);
                        level.Player.Strikes -= 1;
                        level.Player.IsHit = true;
                        level.Player.StopPlayerCollision = true;
                    }
                    startTimer = true;
                }

                //if (startTimer == true && level.Player.StopPlayerCollision == false)
                //{
                //    timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                //    if (timer >= 500)
                //    {
                //        if (StopEnemy == false)
                //        {
                //            startChase = false;
                //            level.Rumble.RumbleSetup(500.0f, 0.5f);
                //            level.player.Strikes -= 10;
                //            level.Player.IsHit = true;
                //            level.Player.StopPlayerCollision = true;
                //        }
                //        timer = 0;
                //    }
                //}
             
                if (absDepthY < absDepthX && level.Player.StopPlayerCollision == false)
                {
                    Position = new Vector2(Position.X, Position.Y + depth.Y);
                    bounds = BoundingRectangle;
                }
                else if (level.Player.StopPlayerCollision == false)
                {
                    Position = new Vector2(Position.X + depth.X, Position.Y);
                    bounds = BoundingRectangle;
                }
            }



            if (startChase == true)
            {
                Vector2 targetDirection = new Vector2(level.Player.Position.X, level.Player.Position.Y - 60) - Position;
                targetDirection.Normalize();

                velocity = targetDirection * MoveSpeed * elapsed;
                Position = Position + velocity;
                Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
            }
            else
            {
                Vector2 targetDirection = startingPoint - Position;

                if (targetDirection.X < 0.0f || targetDirection.Y < 0.0f)
                {
                    targetDirection.Normalize();
                    velocity = targetDirection * MoveSpeed * elapsed;
                    Position = Position + velocity;
                    Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
                }
            }


        }

        private void HandleCollisions()
        {
            int xOrigin = (int)(Position.X) / Tile.Width;
            int yOrigin = (int)(Position.Y) / Tile.Height;

            int xStart = xOrigin - 15;
            int yStart = yOrigin - 15;

            int xEnd = xOrigin + 15;
            int yEnd = yOrigin + 15;

            enemyToChar = new LiangBarsky(Position, new Vector2(level.Player.Position.X, level.Player.Position.Y - 32.0f));

            wallInWay = false;

            for (int y = yStart; y < yEnd; ++y)
                for (int x = xStart; x < xEnd; ++x)
                {
                    TileCollision collision = level.GetCollision(x, y);
                    if (collision != TileCollision.Passable)
                    {
                        Rectangle tileBounds = level.GetBounds(x, y);

                        if (enemyToChar.Intersect(tileBounds) == true)
                        {
                            wallInWay = true;
                        }
                    }
                }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            sprite.PlayAnimation(enemyAnimation);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);
            //DebugDrawer.DrawCircle(spriteBatch, EncounterRadius, Color.Red, 2);
            sprite.Draw(gameTime, spriteBatch, Position, 0, SpriteEffects.None);
        }
    }
}