using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class Turret : Enemy
    {
        private Texture2D turretGun;
        private List<BulletInfo> bulletInfo = new List<BulletInfo>();
        private Vector2 turretGunPosition;
        private float turretGunAngle = MathHelper.PiOver2;



        // bullet variables
        private Texture2D bullet;
        private float bulletSpeed = 35.0f;

        TimeSpan fireTime = TimeSpan.FromSeconds(.25f);
        TimeSpan previousFireTime;

        public Circle EncounterRadius
        {
            get { return new Circle(new Vector2(Position.X + 20, Position.Y + 200), 300); }
        }

        private Vector2 turretGunOrigin;

        public Turret(Level level, Vector2 position, int lootDrop)
         : base(level, position)
            {
                Position = new Vector2(position.X + 50, position.Y + 35);
                turretGunPosition = new Vector2(Position.X, Position.Y - 10);
                this.healthDrop = lootDrop;
                this.Health = 40;
                maxHealth = Health;
                LoadContent();

                turretGunOrigin = new Vector2(0, turretGun.Height / 2);
            }


        protected override void LoadContent()
        {
            enemyAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/Enemies/Turret/TurretPart1"), 0.1f, 100, false);
            turretGun = level.Content.Load<Texture2D>("Sprites/Enemies/Turret/TurretPart2");
            bullet = level.Content.Load<Texture2D>("Sprites/Enemies/Turret/Bullet");

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
            HandleCollisions();

            if (EncounterRadius.Intersects(level.Player.BoundingRectangle) && 
                level.Player.IsOnLadder == false && 
                wallInWay == false)
            {
                turretGunAngle = TurnToFace(turretGunPosition, level.Player.Position, turretGunAngle,
                                60.0f);

                if (gameTime.TotalGameTime - previousFireTime > fireTime)
                {
                    bulletInfo.Add(new BulletInfo(turretGunPosition, turretGunAngle, bullet));
                    previousFireTime = gameTime.TotalGameTime;
                }
            }

            for (int i = 0; i < bulletInfo.Count; i++)
            {
                Vector2 direction = new Vector2((float)Math.Cos(bulletInfo[i].Angle),
                                                (float)Math.Sin(bulletInfo[i].Angle));
                direction.Normalize();

                bulletInfo[i].Position += direction * bulletSpeed;


                if (bulletInfo[i].Collision.Intersects(level.Player.BoundingRectangle) &&
                    level.Player.StopPlayerCollision == false)
                {
                    level.Rumble.RumbleSetup(500.0f, 0.5f);
                    level.Player.Strikes -= 1;
                    level.Player.IsHit = true;
                    level.Player.StopPlayerCollision = true;
                    bulletInfo.RemoveAt(i);
                }
            }


            foreach (var bulletIn in bulletInfo.ToList())
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
                    bulletInfo.Remove(bulletIn);
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
                            //if (bulletInfo.Count > 0)
                            //{
                                bulletInfo.Remove(bulletIn);
                            //}
                        }
                    }
                }
            }   
        }


        public void HandleCollisions()
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

        /// <summary>
        /// Calculates the angle that an object should face, given its position, its
        /// target's position, its current angle, and its maximum turning speed.
        /// </summary>
        private static float TurnToFace(Vector2 position, Vector2 faceThis,
            float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

       
            float desiredAngle = (float)Math.Atan2(y, x);

      
            float difference = WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return WrapAngle(currentAngle + difference);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //DebugDrawer.DrawCircle(spriteBatch, EncounterRadius, Color.Red, 2);

            sprite.PlayAnimation(enemyAnimation);

            spriteBatch.Draw(turretGun, turretGunPosition, null, Color.White,
            turretGunAngle, turretGunOrigin, 1.0f, SpriteEffects.None, 0.0f);

            sprite.Draw(gameTime, spriteBatch, Position, 0, SpriteEffects.None);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);

            foreach (var info in bulletInfo)
            {
                spriteBatch.Draw(bullet, info.Position, null, Color.White, 0,
                new Vector2(bullet.Width / 2, bullet.Height / 2), 1f, SpriteEffects.None, 0f);
            }
        }
    }

    class BulletInfo
    {
        private Texture2D tex;

        public Vector2 Position
        {
            get { return position; }
            set { position = value;  }
        }

        Vector2 position;

        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
      
        float angle;


        public Circle Collision
        {
            get { return new Circle(Position, tex.Width); }
        }

        public BulletInfo(Vector2 position, float angle, Texture2D texture)
        {
            this.position = position;
            this.angle = angle;
            tex = texture;
        }
    };
}