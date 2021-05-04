using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    public enum ShooterState
    {
        Patrol,
        Wait,
        Shoot,
        Run,
        Hit
    }

    class ShootEnemy : Enemy
    {
        private Animation enemyAnimationWalk;
        private Animation enemyAnimationShoot;
        private ShooterState state;

        private float waitTime;
        private const float MaxWaitTime = 1.7f;

        private Rectangle enemySight;
        private Rectangle enemySense;

        // bullet variables
        private Texture2D bullet;
        private float bulletSpeed = 13.5f;
        private List<BulletInfo> bullets = new List<BulletInfo>();

        TimeSpan fireTime = TimeSpan.FromSeconds(0.15f);
        TimeSpan previousFireTime;

        private float bulletWaitTime;
        private const float bulletMaxWaitTime = 1.2f;

        int bulletCount;

        public bool BeingHit
        {
            get { return beingHit; }
            set { beingHit = value; }
        }
        bool beingHit;

        public ShootEnemy(Level level, Vector2 position, int direction)
            :base(level, position)
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
            enemyAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/ShootEnemy/ShootIdle"), 0.1f, 60, true);
            enemyAnimationWalk = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/ShootEnemy/ShootEnemyRun"), 0.1f, 60, true);
            enemyAnimationShoot = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/ShootEnemy/ShootEnemy"), 0.1f, 60, false);
            bullet = level.Content.Load<Texture2D>("Sprites/Enemies/ShootEnemy/EnemyBullet");
            block = level.Content.Load<Texture2D>("square");

            int width = (int)(enemyAnimation.FrameWidth * 0.4);
            int left = (enemyAnimation.FrameWidth - width) / 2;
            int height = (int)(enemyAnimation.FrameHeight * 0.75);
            int top = enemyAnimation.FrameHeight - height;

            localBounds = new Rectangle(left, top, width, height);

            sprite.PlayAnimation(enemyAnimation);
            state = ShooterState.Patrol;
        }

        public override void Update(GameTime gameTime)
        {
            EnemyColor = Color.White;
            //StopEnemy = false;
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if (level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable
             || level.GetCollision(tileX + (int)direction, tileY + 1) == TileCollision.Passable)
            {
                if (state != ShooterState.Wait)
                {
                    waitTime = MaxWaitTime;
                    state = ShooterState.Wait;
                }
            }

            if (level.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Patrol
                &&  state != ShooterState.Wait)
            {
                waitTime = MaxWaitTime;
                state = ShooterState.Wait;
            }

            if (BeingHit == true)
            {
                state = ShooterState.Hit;
            }


            if (CanEnenmyAttack() == true && state != ShooterState.Shoot && beingHit == false)
            {
                state = ShooterState.Shoot;
            }


            //Vector2 targetDirection = new Vector2(level.Player.Position.X, level.Player.Position.Y - 40) - Position;
            //if (targetDirection.Length() < 150.0f)
            //{
            //    state = ShooterState.Run;
            //}


            EnenmyActions(gameTime);
            HandleCollisions(gameTime);
            HandleBullets();
        }

        private bool CanEnenmyAttack()
        {

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

            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / Tile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / Tile.Height);

            if(enemySense.Intersects(level.Player.BoundingRectangle) &&
            level.GetCollision(tileX + (int)direction, tileY + 1) != TileCollision.Passable && wallInWay == false)
            {
                direction = (FaceDirection)(-(int)direction);
                return true;
            }
            else if (enemySight.Intersects(level.Player.BoundingRectangle) &&
            level.GetCollision(tileX + (int)direction, tileY + 1) != TileCollision.Passable && wallInWay == false)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }

        public void EnenmyActions(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (state)
            {
                case ShooterState.Patrol:
                    sprite.PlayAnimation(enemyAnimationWalk);
                    velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);

                    if (StopEnemy == true)
                        velocity = Vector2.Zero;

                    Position = Position + velocity;
                    Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
                break;

                case ShooterState.Wait:
                    sprite.PlayAnimation(enemyAnimation);
                    waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                    if (waitTime <= 0.0f)
                    {
                        direction = (FaceDirection)(-(int)direction);
                        state = ShooterState.Patrol;
                    }
                break;

                case ShooterState.Shoot:
                    sprite.PlayAnimation(enemyAnimationShoot);


                    if (bulletWaitTime > 0)
                        bulletWaitTime = Math.Max(0.0f, bulletWaitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);

                    if (bulletCount > 2)
                    {
                        bulletWaitTime = bulletMaxWaitTime;
                        bulletCount = 0;
                        state = ShooterState.Patrol;
                    }

                    if (gameTime.TotalGameTime - previousFireTime > fireTime && bulletWaitTime <= 0)
                    {
                        bulletCount++;
                        bullets.Add(new BulletInfo(new Vector2(Position.X, Position.Y - 32),(int)direction, bullet));
                        previousFireTime = gameTime.TotalGameTime;
                    }

                break;

                case ShooterState.Run:
                    sprite.PlayAnimation(enemyAnimationWalk); 
                    Vector2 targetDirection = Position - new Vector2(level.Player.Position.X, level.Player.Position.Y - 40);

                    if (targetDirection.Length() > 150)
                    {
                        if (CanEnenmyAttack() == true)
                        {
                            state = ShooterState.Shoot;
                        }
                        else
                        {
                            state = ShooterState.Patrol;
                        }              
                        break;
                    }

                    if (targetDirection.X < 0)
                    {
                        direction = FaceDirection.Left;
                    }
                    else
                    {
                        direction = FaceDirection.Right;
                    }

                    targetDirection.Normalize();
                    velocity = new Vector2(targetDirection.X, 0.0f) * MoveSpeed * elapsed;

                    if (StopEnemy == true)
                        velocity = Vector2.Zero;

                    Position = Position + velocity;
                    Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));
                break;

                case ShooterState.Hit:
                    EnemyColor = Color.Red;
                break;
                    
            }
        }

        public override void HandleCollisions(GameTime gameTime)
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
                    if (collision != TileCollision.Passable &&
                        collision != TileCollision.Patrol)
                    {
                        Rectangle tileBounds = level.GetBounds(x, y);

                        if (enemyToChar.Intersect(tileBounds) == true)
                        {
                            wallInWay = true;
                        }
                    }
                }



            Rectangle bounds = BoundingRectangle;
            Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, level.Player.BoundingRectangle);

            if (depth.X < -2.0f || depth.X > 2.0f)
            {
                velocity = Vector2.Zero;

                if (level.Player.StopPlayerCollision == false)
                {
                    level.Rumble.RumbleSetup(500.0f, 0.5f);
                    level.Player.Strikes -= 1;
                    level.Player.IsHit = true;
                }
            }
        }

        private void HandleBullets()
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Position += new Vector2(bullets[i].Angle, 0) * bulletSpeed;

                if (bullets[i].Collision.Intersects(level.Player.BoundingRectangle) &&
                 level.Player.StopPlayerCollision == false)
                {
                    level.Rumble.RumbleSetup(500.0f, 0.5f);
                    level.Player.Strikes -= 1;
                    level.Player.IsHit = true;
                    level.Player.StopPlayerCollision = true;
                    bullets.RemoveAt(i);
                }
            }

            foreach (var bulletIn in bullets.ToList())
            {
                Rectangle bounds = new Rectangle((int)bulletIn.Position.X,
                                                 (int)bulletIn.Position.Y,
                                                 bullet.Width,
                                                 bullet.Height);


                int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
                int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
                int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
                int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;


                if (level.Player.AttackBounds.Intersects(bounds) || 
                    level.Player.ShieldBounds.Intersects(bounds))
                {
                    bullets.Remove(bulletIn);
                    break;
                }

                for (int y = topTile; y <= bottomTile; ++y)
                {
                    for (int x = leftTile; x <= rightTile; ++x)
                    {
                        TileCollision collision = level.GetCollision(x, y);

                        if (collision != TileCollision.Passable &&
                            collision != TileCollision.Patrol)
                        {
                            bullets.Remove(bulletIn);
                        }
                    }
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            foreach (var bull in bullets)
            {
                spriteBatch.Draw(bullet, bull.Position, null, Color.White, 0,
                new Vector2(bullet.Width / 2, bullet.Height / 2), 1f, SpriteEffects.None, 0f);
            }

            //spriteBatch.Draw(block, enemySight, Color.White);
            //spriteBatch.Draw(block, enemySense, Color.White);

            SpriteEffects flip = direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            sprite.Draw(gameTime, spriteBatch, Position, 0, flip, EnemyColor);
        }
    }
}
