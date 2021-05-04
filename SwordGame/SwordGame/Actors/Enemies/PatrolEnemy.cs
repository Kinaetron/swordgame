using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class PatrolEnemy : Enemy 
    {

        private bool attackPlayer;

        private float waitTime;
        private const float MaxWaitTime = 1.7f;

        private float attackTime;
        private const float maxAttackTime = 3;

        private Rectangle enemySight;
        private Rectangle enemySense;

        private Animation enemyAnimationWalk;
        private Animation enemyAnimationAttack;

        int tileX;
        int tileY;

        public bool BeingHit
        {
            get { return beingHit; }
            set { beingHit = value; }
        }
        bool beingHit;


        public PatrolEnemy(Level level, Vector2 position, int direction)
         : base(level, position)
            {
                Position = new Vector2(position.X, position.Y + 12);
                healthDrop = direction;
                this.direction = direction < 0 ? FaceDirection.Left : FaceDirection.Right;
                this.Health = 40;
                maxHealth = Health;
                MoveSpeed = 160.0f;

                LoadContent();
            }

        protected override void LoadContent()
        {
            enemyAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/PatrolEnemy/PatrolIdle"), 0.1f, 60, true);
            enemyAnimationWalk = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/PatrolEnemy/EnemyWalking"), 0.1f, 60, true);
            enemyAnimationAttack = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/PatrolEnemy/EnemyAttack"), 0.1f, 80, true);
            block = level.Content.Load<Texture2D>("square");


            // Calculate bounds within texture size.            
            int width = (int)(enemyAnimation.FrameWidth * 0.4);
            int left = (enemyAnimation.FrameWidth - width) / 2;
            int height = (int)(enemyAnimation.FrameHeight * 0.75);
            int top = enemyAnimation.FrameHeight - height;

            localBounds = new Rectangle(left, top, width, height);

            EnemyColor = Color.White;

            sprite.PlayAnimation(enemyAnimation);
        }

        public override void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float enemyDistance = Vector2.Distance(level.Player.Position, Position);
            velocity = Vector2.Zero;
            EnemyColor = Color.White;

            HandleCollisions(gameTime);

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            tileY = (int)Math.Floor(Position.Y / Tile.Height);


            if (enemySight.Intersects(level.Player.BoundingRectangle) &&
               level.GetCollision(tileX + (int)direction, tileY + 1) != TileCollision.Passable && wallInWay == false ||
               enemySense.Intersects(level.Player.BoundingRectangle) && 
               level.GetCollision(tileX + (int)direction, tileY + 1) != TileCollision.Passable && wallInWay == false ||
               StopEnemy == true && wallInWay == false)
            {
                startChase = true;
            }
            else 
            {
                startChase = false;
            }


            if (level.Player.IsOnLadder == true)
                startChase = false;

            // if (
            //    || level.GetCollision(tileX + (int)direction, tileY + 1) == TileCollision.Passable)
            //{
            //    startChase = false;
            //}

            if(startChase == true)
            {
                if (attackPlayer == true)
                {
                    //if (attackTime >= maxAttackTime)
                    //{
                    //    attackPlayer = false;
                    //    attackTime = 0;
                    //}
                }

              

                Vector2 targetDirection = new Vector2(level.Player.Position.X, level.Player.Position.Y - 40) - Position;
                targetDirection.Normalize();

                if (targetDirection.X < 0)
                    direction = FaceDirection.Left;
                else
                    direction = FaceDirection.Right;

                velocity = new Vector2(targetDirection.X, 0.0f) * MoveSpeed * elapsed;

                if (StopEnemy == true || enemyDistance < 70.0f)
                    velocity = Vector2.Zero;

                Position = Position + velocity;
                Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
            }
            else if (waitTime > 0)
            {
                sprite.PlayAnimation(enemyAnimation);
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    direction = (FaceDirection)(-(int)direction);
                }
            }
            else if (level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable
                  || level.GetCollision(tileX + (int)direction, tileY + 1) == TileCollision.Passable)
            {
                waitTime = MaxWaitTime;
            }
            else if (level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Patrol
                     && startChase == false)
            {
                 waitTime = MaxWaitTime;
            }
            else
            {
                velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);

                if (StopEnemy == true || enemyDistance < 70.0f)
                    velocity = Vector2.Zero;


                Position = Position + velocity;
                Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
            }

            EnemyAttack(gameTime);
        }

        private void EnemyAttack(GameTime gameTime)
        {
            float enemyDistance = Vector2.Distance(level.Player.Position, Position);

            if (attackPlayer == true)
                attackTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (attackPlayer == true && sprite.AnimationFinished == true)
                sprite.PlayAnimation(enemyAnimation);

            if (attackTime >= maxAttackTime)
            {
                attackPlayer = false;
                attackTime = 0;
            }

            if (enemyDistance < 70.0f && enemyDistance > 30.0f && BeingHit == false)
            {
                StopEnemy = true;
                velocity = Vector2.Zero;

                if (attackPlayer == false)
                {
                    sprite.PlayAnimation(enemyAnimationAttack);
                }

                if (enemyAnimationAttack.FrameCount-1 == sprite.FrameIndex && attackPlayer == false)
                {
                    level.Rumble.RumbleSetup(500.0f, 0.5f);
                    level.Player.Strikes -= 1;
                    level.Player.IsHit = true;
                    attackPlayer = true;
                }
            }
            else if (Math.Abs(velocity.X) - 0.02f > 0)
            {
                sprite.PlayAnimation(enemyAnimationWalk);
            }
            else
            {
                sprite.PlayAnimation(enemyAnimation);
            }
        }


        public override void HandleCollisions(GameTime gameTime)
        {
           HandlePlayerCollision(gameTime);

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
                   if (collision != TileCollision.Passable &&
                       collision !=  TileCollision.Patrol)
                   {
                       Rectangle tileBounds = level.GetBounds(x, y);

                       if (enemyToChar.Intersect(tileBounds) == true)
                       {
                           wallInWay = true;
                       }
                   }
               }


        }


        private void HandlePlayerCollision(GameTime gameTime)
        {
            Rectangle bounds = BoundingRectangle;
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, level.Player.BoundingRectangle);

            if (direction == FaceDirection.Right)
            {
                enemySight = new Rectangle((int)Position.X, (int)Position.Y - 54, 400, 20);
                enemySense = new Rectangle((int)Position.X - 150, (int)Position.Y - 54, 150, 20);
            }
            else if (direction == FaceDirection.Left)
            {
                enemySight = new Rectangle((int)Position.X - 400, (int)Position.Y - 54, 400, 20);
                enemySense = new Rectangle((int)Position.X, (int)Position.Y - 54, 150, 20);
            }

            if (depth.X < -2.0f || depth.X > 2.0f)
            {
                velocity = Vector2.Zero;

                if (level.Player.StopPlayerCollision == false)
                {
                    level.Rumble.RumbleSetup(500.0f, 0.5f);
                    level.Player.Strikes -= 1;
                    level.Player.IsHit = true;
                    attackPlayer = true;
                    level.Player.StopPlayerCollision = true;
                }

                //StopEnemy = true;
            }
            else
            {
                StopEnemy = false;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            //spriteBatch.Draw(block, new Vector2((tileX + (int)direction) * Tile.Width, (tileY - 1) * Tile.Height), Color.White);

            // Draw facing the way the enemy is moving.
            SpriteEffects flip = direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, 0, flip, EnemyColor);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);
        }
    }
}