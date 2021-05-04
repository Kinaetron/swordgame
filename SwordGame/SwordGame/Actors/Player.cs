using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SwordGame
{
    class Player
    {
        // Physics state
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        int health = 100;


        private Rectangle localBounds;
        private Rectangle idleBounds;
        private Rectangle runBounds;
        private Rectangle jumpBounds;
        private Rectangle crouchBounds;

        private Rectangle middleAttackBounds;
        private Rectangle overAttackBounds;
        private Rectangle lowAttackMedBounds;
        private Rectangle lowAttackLowBounds;


        private Vector2 AttackPosition;
        private Rectangle AttackBounds;

        /// <summary>
        /// Gets a rectangle which bounds this player in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }
        bool isOnGround;

        Texture2D block;

        private Vector2 velocity;
        private AnimationPlayer sprite;

        private Level level;

        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation crouchAnimation;

        // Attack Animations
        private Animation middleAttackAnimation;
        private Animation overAttackAnimation;
        private Animation crouchAttackAnimationMed;
        private Animation crouchAttackAnimationLow;

        // Attack booleans 
        private bool middleAttack;
        private bool overAttack;
        private bool crouchAttackMed;
        private bool crouchAttackLow;

        private SpriteEffects flip = SpriteEffects.None;

        // movement variables
        private const float MoveAcceleration = 5000.0f;
        private const float MoveDeceleration = 7500.0f;
        private const float MaxMoveSpeed = 320.0f;
        private const float GroundDragFactor = 0.57f;

        // jumping variables
        private const float smallJump = 20000.0f;
        private const float normalJump = 80000.0f;

        // gravity variables 
        private const float GravityAcceleration = 3400.0f;
        private const float MaxFallSpeed = 900.0f;

        private float previousBottom;
       
        private Vector2 movement;
        private const float MoveStickScale = 1.0f;

        private bool isJumpButtonDown;
        private bool isJumpButtonUp;
        private bool jumping;

        // crouch position
        private bool crouch;

        // Old input states 
        KeyboardState oldKeyBoardState;
        GamePadState oldPadState;

        Texture2D healthBarImage;
        Rectangle healthRectangle;

        public Player(Level level, Vector2 position)
        {
            this.level = level;
            LoadContent();
            Reset(position);
        }

        public void LoadContent()
        {
            idleAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Stance"), 0.1f, 95, true);
            runAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Running"), 0.1f, 95, true);
            jumpAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Jumping"), 0.1f, 100, true);
            crouchAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Crouch"), 0.1f, 90, false);
            block = level.Content.Load<Texture2D>("square");
            healthBarImage = level.Content.Load<Texture2D>("Sprites/GUI/HealthImage");

            middleAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/MiddleAttack"), 0.1f, 160, true);
            overAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/OverAttack"), 0.1f, 140, true);
            crouchAttackAnimationLow = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/CrouchAttack"), 0.1f, 100, true);
            crouchAttackAnimationMed = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/CrouchAttack2"), 0.1f, 170, true);

            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameHeight * 0.75);
            int top = idleAnimation.FrameHeight - height;

            idleBounds = new Rectangle(left, top, width, height);

            width = (int)(runAnimation.FrameWidth * 0.4);
            left = (runAnimation.FrameWidth - width) / 2;
            height = (int)(runAnimation.FrameHeight * 0.75);
            top = runAnimation.FrameHeight - height;

            runBounds = new Rectangle(left, top, width, height);

            width = (int)(jumpAnimation.FrameWidth * 0.6);
            left = (jumpAnimation.FrameWidth - width) / 2;
            height = (int)(jumpAnimation.FrameHeight * 0.75);
            top = jumpAnimation.FrameHeight - height;

            jumpBounds = new Rectangle(left, top, width, height);

            width = (int)(crouchAnimation.FrameWidth * 0.4);
            left = (crouchAnimation.FrameWidth - width) / 2;
            height = (int)(crouchAnimation.FrameHeight * 0.5);
            top = crouchAnimation.FrameHeight - height;

            crouchBounds = new Rectangle(left, top, width, height);


            width = (int)(middleAttackAnimation.FrameWidth * 0.3);
            left = (middleAttackAnimation.FrameWidth - width) / 2;
            height = (int)(middleAttackAnimation.FrameHeight * 0.75);
            top = middleAttackAnimation.FrameHeight - height;

            middleAttackBounds = new Rectangle(left, top, width, height);


            width = (int)(overAttackAnimation.FrameWidth * 0.4);
            left = (overAttackAnimation.FrameWidth - width) / 2;
            height = (int)(overAttackAnimation.FrameHeight * 0.6);
            top = overAttackAnimation.FrameHeight - height;

            overAttackBounds = new Rectangle(left, top, width, height);


            width = (int)(crouchAttackAnimationLow.FrameWidth * 0.4);
            left = (crouchAttackAnimationLow.FrameWidth - width) / 2;
            height = (int)(crouchAttackAnimationLow.FrameHeight * 0.4);
            top = crouchAttackAnimationLow.FrameHeight - height;

            lowAttackLowBounds = new Rectangle(left, top, width, height);

            width = (int)(crouchAttackAnimationMed.FrameWidth * 0.3);
            left = (crouchAttackAnimationMed.FrameWidth - width) / 2;
            height = (int)(crouchAttackAnimationMed.FrameHeight * 0.75);
            top = crouchAttackAnimationMed.FrameHeight - height;

            lowAttackMedBounds = new Rectangle(left, top, width, height);

            localBounds = idleBounds;
        }

        public void Reset(Vector2 position)
        {
            Position = position;
            velocity = Vector2.Zero;
            sprite.PlayAnimation(idleAnimation);
        }

        public void Update(GameTime gameTime, TileMap tileMap)
        {
            ApplyPhysics(gameTime, tileMap);
            PlayerAnimation();

            healthRectangle = new Rectangle(0, 0, health, 20);
        }

        public void ApplyPhysics(GameTime gameTime, TileMap tileMap)
        {
            PlayerMovement(gameTime);
            HandleCollisions(tileMap, gameTime);
        }

        public void HandleInput(InputState input)
        {
            // Look up inputs for the active player profile.
            int playerIndex = 1;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[0];

            // Get analog horizontal movement.
            movement = gamePadState.ThumbSticks.Left * MoveStickScale;

            if (Math.Abs(movement.X) < 0.5f)
            {
                movement.X = 0.0f;
            }


            if (gamePadState.IsButtonDown(Buttons.DPadLeft) ||
                keyboardState.IsKeyDown(Keys.Left))
            {
                movement.X = -1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadRight) ||
                     keyboardState.IsKeyDown(Keys.Right))
            {
                movement.X = 1.0f;
            }

            if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
               movement.Y < -0.5f ||
               keyboardState.IsKeyDown(Keys.Down))
            {
                crouch = true;
            }

            if (IsOnGround == true)
            {

                if (keyboardState.IsKeyDown(Keys.Z) && crouch == false)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.Z))
                    {
                        middleAttack = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.X) && crouch == false)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.X))
                    {
                        overAttack = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.Z) && crouch == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.Z) && crouch == true)
                    {
                        crouchAttackLow = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.X) && crouch == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.X) && crouch == true)
                    {
                        crouchAttackMed = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.X) && crouch == false)
                {
                    if (!oldPadState.IsButtonDown(Buttons.X))
                    {
                        middleAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.Y) && crouch == false)
                {
                    if (!oldPadState.IsButtonDown(Buttons.Y))
                    {
                        overAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.X) && crouch == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.X) && crouch == true)
                    {
                        crouchAttackLow = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.Y) && crouch == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.Y) && crouch == true)
                    {
                        crouchAttackMed = true;
                    }
                }
            }

            if (gamePadState.IsConnected)
            {
                // Check if the player wants to jump.
                if (gamePadState.IsButtonDown(Buttons.A))
                {
                    isJumpButtonDown = true;
                }

                if (gamePadState.IsButtonUp(Buttons.A))
                {
                    isJumpButtonUp = true;
                }
            }
            else
            {
                // Check if the player wants to jump.
                if (keyboardState.IsKeyDown(Keys.Space))
                {
                    isJumpButtonDown = true;
                }

                if (keyboardState.IsKeyUp(Keys.Space))
                {
                    isJumpButtonUp = true;
                }
            }

            oldKeyBoardState = keyboardState;
            oldPadState = gamePadState;
        }

        private void PlayerMovement(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

             Vector2 previousPosition = Position;

            if (IsOnGround == true)
            {
                velocity.Y = 0;
            }

            if (crouch == true || middleAttack == true || overAttack == true)
            {
                movement = Vector2.Zero;
            }

            if (isJumpButtonDown && false == jumping)
            {
                if (isOnGround)
                {
                    jumping = true;
                    velocity.Y = -normalJump * elapsed;
                }

                localBounds = jumpBounds;
                sprite.PlayAnimation(jumpAnimation);
            }
            else if (isJumpButtonUp && true == jumping)
            {
                if (velocity.Y < -smallJump * elapsed)
                {
                    velocity.Y = -smallJump * elapsed;
                }

                jumping = false;
            }

            if (movement.X < 0.0f)
            {
                if (velocity.X > 0.0f)
                {
                    velocity.X -= MoveDeceleration * elapsed;
                }
                else if (velocity.X > -MaxMoveSpeed)
                {
                    velocity.X -= MoveAcceleration * elapsed;
                }
            }
            else if (movement.X > 0.0f)
            {
                if (velocity.X < 0.0f)
                {
                    velocity.X += MoveDeceleration * elapsed;
                }
                else if (velocity.X < MaxMoveSpeed)
                {
                    velocity.X += MoveAcceleration * elapsed;
                }
            }
            else
            {
                velocity.X *= GroundDragFactor;
            }

            velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);

            position += velocity * elapsed;

            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            isJumpButtonDown = false;
            isJumpButtonUp = false;
        }

        private void PlayerAnimation()
        {
            if (IsOnGround)
            {
                if (sprite.AnimationFinished == true)
                {
                    middleAttack = false;
                    overAttack = false;
                    crouchAttackLow = false;
                    crouchAttackMed = false;

                    AttackBounds = new Rectangle();
                }

                if (middleAttack == true)
                {
                    //localBounds = middleAttackBounds;
                    sprite.PlayAnimation(middleAttackAnimation);

                    if (flip == SpriteEffects.None)
                    {
                        localBounds = new Rectangle(middleAttackBounds.X - 20, middleAttackBounds.Y, 
                                                    middleAttackBounds.Width, middleAttackBounds.Height);


                        AttackPosition = new Vector2(Position.X + 10, Position.Y - 70);
                    }
                    else
                    {
                        localBounds = new Rectangle(middleAttackBounds.X + 30, middleAttackBounds.Y,
                                                   middleAttackBounds.Width, middleAttackBounds.Height);

                        AttackPosition = new Vector2(Position.X - 80, Position.Y - 70);
                    }

                    AttackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      80, 24);
                }
                else if (overAttack == true)
                {
                    localBounds = overAttackBounds;
                    sprite.PlayAnimation(overAttackAnimation);

                    if (flip == SpriteEffects.None)
                    {
                        AttackPosition = new Vector2(Position.X + 20, Position.Y - 130);
                    }
                    else
                    {
                        AttackPosition = new Vector2(Position.X - 70, Position.Y - 130);
                    }

                    AttackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      50, 100);
                }
                else if (crouchAttackLow == true)
                {
                    localBounds = lowAttackLowBounds;
                    sprite.PlayAnimation(crouchAttackAnimationLow);

                    if (flip == SpriteEffects.None)
                    {
                        AttackPosition = new Vector2(Position.X + 20, Position.Y - 30);
                    }
                    else
                    {
                        AttackPosition = new Vector2(Position.X - 50, Position.Y - 30);
                    }

                    AttackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      25, 25);
                }
                else if (crouchAttackMed == true)
                {
                    //localBounds = lowAttackMedBounds;
                    sprite.PlayAnimation(crouchAttackAnimationMed);

                    if (flip == SpriteEffects.None)
                    {
                        localBounds = new Rectangle(lowAttackMedBounds.X - 40, lowAttackMedBounds.Y,
                                                  lowAttackMedBounds.Width, lowAttackMedBounds.Height);


                        AttackPosition = new Vector2(Position.X + 20, Position.Y - 50);
                    }
                    else
                    {
                        localBounds = new Rectangle(lowAttackMedBounds.X + 40, lowAttackMedBounds.Y,
                                                  lowAttackMedBounds.Width, lowAttackMedBounds.Height);


                        AttackPosition = new Vector2(Position.X - 60, Position.Y - 50);
                    }

                    AttackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      64, 16);
                }
                else if (crouch == true)
                {
                    localBounds = crouchBounds;
                    sprite.PlayAnimation(crouchAnimation);
                }
                else if (Math.Abs(velocity.X) - 0.02f > 0)
                {
                    localBounds = runBounds;
                    sprite.PlayAnimation(runAnimation);
                }
                else
                {
                    localBounds = idleBounds;
                    sprite.PlayAnimation(idleAnimation);
                }
            }
            crouch = false;
        }

        private void HandleCollisions(TileMap tileMap, GameTime gameTime)
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            int leftTile = (int)Math.Floor((float)bounds.Left / Tile.Width);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / Tile.Width)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / Tile.Height);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / Tile.Height)) - 1;

            // Reset flag to search for ground collision.
            isOnGround = false;

              // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = level.GetCollision(x, y, tileMap);

                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                {
                                    isOnGround = true;
                                }

                                // Ignore platforms, unless we are on the ground.
                                if (collision == TileCollision.Impassable || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == TileCollision.Impassable) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }
            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera cam, TileMap tileMap)
        {

            cam.LockToTarget(Position, idleBounds, 1024, 576);
            cam.ClampToArea((int)tileMap.Layers[0].WidthInPixels - 1024, (int)tileMap.Layers[0].HeightInPixels - 576);

            if (velocity.X < 0)
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if (velocity.X > 0)
            {
                flip = SpriteEffects.None;
            }

            spriteBatch.Begin(SpriteSortMode.Immediate,
                              BlendState.AlphaBlend,
                              null, null, null, null,
                              cam.TransformMatrix);


            spriteBatch.Draw(healthBarImage, new Vector2(cam.Position.X + 20,
                             cam.Position.Y + 50), healthRectangle, Color.White);


            sprite.Draw(gameTime, spriteBatch, Position, 0.0f, flip);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);

            //spriteBatch.Draw(block, AttackBounds, Color.White);

            spriteBatch.End();
        }
    }
}
