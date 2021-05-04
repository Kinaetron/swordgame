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

        private const int LadderAlignment = 12;

        float fallDepth;

        public bool IsOnLadder
        {
            get { return isOnLadder; }
            set { isOnLadder = value; }
        }
        private bool isOnLadder;

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
            set { health = value; if (health > 100) health = 100; }
        }
        int health = 100;


        public int Strikes
        {
            get { return strikes; }
            set { strikes = value; if (strikes > 2) strikes = 2; }
        }
        int strikes = 2;


        public bool IsAlive
        {
            get { return isAlive; }
        }
        bool isAlive;

        public bool IsHit
        {
            get { return isHit; }
            set { isHit = value; }
        }
        bool isHit;

        public bool IsKnockedBack
        {
            get { return isKnockedBack; }
            set { isKnockedBack = value; }
        }
        bool isKnockedBack;

        public bool StopPlayerCollision
        {
            get { return stopPlayerCollision; }
            set { stopPlayerCollision = value; }
        }
        bool stopPlayerCollision;

        private bool stopMovement = false;
        private float counter;

        private Camera camera;

        private Color color = Color.White;

        private Rectangle localBounds;
        private Rectangle idleBounds;
        private Rectangle runBounds;
        private Rectangle jumpBounds;
        private Rectangle crouchBounds;

        private Rectangle middleAttackBounds;
        private Rectangle overAttackBounds;
        private Rectangle lowAttackMedBounds;
        private Rectangle lowAttackLowBounds;
        private Rectangle airAttackBounds;
        private Rectangle overAirAttackBounds;


        private Vector2 AttackPosition;
      

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
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle ShieldBounds
        {
            get
            {
                //return new Circle(new Vector2(Position.X, Position.Y - 100), shield.Width / 2.0f);
                return shieldBounds;
            }
        }
        Circle shieldBounds;

        public Rectangle AttackBounds
        {
            get { return attackBounds; }
            set { attackBounds = value; }
        }
        Rectangle attackBounds;

        public int AttackPoints
        {
            get { return attackPoints; }
        }
        int attackPoints;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }
        bool isOnGround;

        public AnimationPlayer Sprite
        {
            get { return sprite; }
        }

        private AnimationPlayer sprite;

        Texture2D block;

        private Vector2 velocity;
       

        private Level level;
        private TileMap tileMap;

        private Animation idleAnimation;
        private Animation runAnimation;
        private Animation jumpAnimation;
        private Animation crouchAnimation;

        // Attack Animations
        private Animation middleAttackAnimation;
        private Animation overAttackAnimation;
        private Animation crouchAttackAnimationMed;
        private Animation crouchAttackAnimationLow;
        private Animation airAttackAnimation;
        private Animation overAirAttackAnimation;

        // Shield Texture
        private Texture2D shield;
        private Vector2   shieldOrigin;

        // Strikes display
        private Texture2D strikeImage;
        private Vector2   strikeOrigin;

        // Attack booleans 
        private bool middleAttack;
        private bool overAttack;
        private bool crouchAttackMed;
        private bool crouchAttackLow;
        private bool airAttack;
        private bool overAirAttack;


        //Shield values

        public bool InitShield
        {
            get { return initShield; }
            set { initShield = value; }
        }
        private bool initShield;

        public float CoolDownTime
        {
            get { return coolDownTime; }
            set { coolDownTime = value; }
        }
        private float coolDownTime;
      
        private float activationTime;
        private bool shieldActivation;
        private bool shieldDraw;
        private bool coolDown;
        private bool shieldIconDraw;

        // Charged attacks
        private float middleAttackTimer;
        private bool middleAttackCharged;
        private bool middleAttackChargedRelease;

        private float overAttackTimer;
        private bool overAttackCharged;
        private bool overAttackChargedRelease;

        private const float chargeTime = 700.0f;


        private SpriteEffects flip = SpriteEffects.None;

        // movement variables
        private const float MoveAcceleration = 1600.0f;
        private const float MoveDeceleration = 3400.0f;
        private const float MaxMoveSpeed = 520.0f;
        private const float GroundDragFactor = 0.86f;

        // Ladder acceleration
        private const float ladderAcceleration = 10000.0f;

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


        private bool knockbackDirection;
        float moveback = 0;

        // Old input states 
        KeyboardState oldKeyBoardState;
        GamePadState oldPadState;

        Texture2D healthBarImage;
        Rectangle healthRectangle;

        Texture2D shieldIcon;

        public Player(Level level, Vector2 position, Camera camera, TileMap tileMap)
        {
            this.level = level;
            this.camera = camera;
            this.tileMap = tileMap;
            LoadContent();
            Reset(position);
        }

        public void LoadContent()
        {
            strikeImage = level.Content.Load<Texture2D>("Sprites/Pickups/Heart");
            strikeOrigin = new Vector2(strikeImage.Width / 2.0f, strikeImage.Height / 2.0f);

            idleAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Stance"), 0.1f, 95, true);
            runAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Running"), 0.1f, 95, true);
            jumpAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Jumping"), 0.1f, 100, false);
            crouchAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Crouch"), 0.1f, 90, false);
            block = level.Content.Load<Texture2D>("square");
            healthBarImage = level.Content.Load<Texture2D>("Sprites/GUI/HealthImage");
            shieldIcon = level.Content.Load<Texture2D>("Sprites/GUI/ShieldIcon");

            middleAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/MiddleAttack"), 0.1f, 160, true);
            overAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/OverAttack"), 0.1f, 140, true);
            crouchAttackAnimationLow = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/CrouchAttack"), 0.1f, 100, true);
            crouchAttackAnimationMed = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/CrouchAttack2"), 0.1f, 170, true);
            airAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/AirAttack"), 0.1f, 145, true);
            overAirAttackAnimation = new Animation(level.Content.Load<Texture2D>("Sprites/PlayerAnimations/AirAttackOver"), 0.1f, 160, true);


            shield = level.Content.Load<Texture2D>("Sprites/PlayerAnimations/Shield");
            shieldOrigin = new Vector2(shield.Width / 2.0f, shield.Height / 2.0f);


            sprite.PlayAnimation(idleAnimation);

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

            width = (int)(airAttackAnimation.FrameWidth * 0.4);
            left = (airAttackAnimation.FrameWidth - width) / 2;
            height = (int)(airAttackAnimation.FrameHeight * 0.6);
            top = airAttackAnimation.FrameHeight - height;

            airAttackBounds = new Rectangle(left, top, width, height);



            width = (int)(overAirAttackAnimation.FrameWidth * 0.4);
            left = (overAirAttackAnimation.FrameWidth - width) / 2;
            height = (int)(overAirAttackAnimation.FrameHeight * 0.6);
            top = overAirAttackAnimation.FrameHeight - height;

            overAirAttackBounds = new Rectangle(left, top, width, height);

            localBounds = idleBounds;
        }

        public void Reset(Vector2 position)
        {
            isAlive = true;

            Position = position;
            velocity = Vector2.Zero;
            sprite.PlayAnimation(idleAnimation);

            camera.Position = Position;

            camera.CameraTrap = new Rectangle((int)BoundingRectangle.Left,
                BoundingRectangle.Bottom - 230, 350, 230);
        }

        public void OnKilled()
        {
            isAlive = false;
            level.Rumble.TurnOffRumble();
        }

        public void Update(GameTime gameTime, TileMap tileMap)
        {
            ApplyPhysics(gameTime, tileMap);

            camera.LockToTarget(velocity, BoundingRectangle, 1280, 720);
            camera.ClampToArea((int)tileMap.Layers[0].WidthInPixels - 1280, (int)tileMap.Layers[0].HeightInPixels - 720);

            PlayerAnimation(gameTime);

            if (strikes <= 0)
            {
                OnKilled();
            }

            //healthRectangle = new Rectangle(0, 0, health, 20);
        }

        public void ApplyPhysics(GameTime gameTime, TileMap tileMap)
        {
          

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsKnockedBack == true)
            {
                counter += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                stopMovement = true;
                StopPlayerCollision = true;
                color = Color.Red;

                if (knockbackDirection == false)
                {
                    if (velocity.X > 0)
                        moveback = -20000;
                    else
                        moveback = 20000;

                    knockbackDirection = true;
                }
               
                if (counter < 100)
                {
                    velocity = Vector2.Zero;
                    velocity.X = moveback * elapsed;
                    velocity.Y = -smallJump * elapsed;

                    velocity.Y = MathHelper.Clamp(
                     velocity.Y + GravityAcceleration * elapsed,
                     -MaxFallSpeed,
                     MaxFallSpeed);

                    Position += velocity * elapsed;
                }

                if (counter > 600)
                {
                    stopMovement = false;
                    IsHit = false;
                   
                    if (counter > 1500)
                    {
                        color = Color.White;
                        counter = 0;
                        IsKnockedBack = false;
                        StopPlayerCollision = false;

                        moveback = 0;
                        knockbackDirection = false;
                    }
                }
            }

            ChargeAttackColour();
            PlayerMovement(gameTime);
            HandleCollisions(tileMap, gameTime);
        }

        public void HandleInput(InputState input, GameTime gameTime)
        {
            // Look up inputs for the active player profile.
            int playerIndex = 1;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[0];

            // Get analog horizontal movement.
            movement = CircleToSquare(gamePadState.ThumbSticks.Left) * MoveStickScale;


            float deadZoneMagnitude = 0.25f;
            if (movement.Length() < deadZoneMagnitude)
            {
                movement = Vector2.Zero;
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

            // Ladder code
            if (gamePadState.IsButtonDown(Buttons.DPadUp) ||
                 keyboardState.IsKeyDown(Keys.Up))
            {
                movement.Y = 1.0f;
            }
            else if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
                    keyboardState.IsKeyDown(Keys.Down))
            {
                movement.Y = -1.0f;
            }

            if (isOnLadder == true)
            {
                movement.Y *= -1.0f;
            }

            if (isOnLadder == false)
            {
             if (gamePadState.IsButtonDown(Buttons.DPadDown) ||
             movement.Y < -0.5f ||
             keyboardState.IsKeyDown(Keys.Down))
                {
                    crouch = true;
                }
            }

            if (isOnLadder == false)
            {
                if (keyboardState.IsKeyDown(Keys.Z) && crouch == false && isOnGround == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.Z))
                    {
                        middleAttack = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.X) && crouch == false && isOnGround == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.X))
                    {
                        overAttack = true;
                    }
                }


                if (keyboardState.IsKeyDown(Keys.Z) && crouch == false && isOnGround == false)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.Z))
                    {
                        airAttack = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.X) && crouch == false && isOnGround == false)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.X))
                    {
                        overAirAttack = true;
                    }
                }


                if (keyboardState.IsKeyDown(Keys.Z) && crouch == true
                    && IsOnGround == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.Z) && crouch == true
                         && IsOnGround == true)
                    {
                        crouchAttackLow = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.X) && crouch == true
                     && IsOnGround == true)
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.X) && crouch == true
                          && IsOnGround == true)
                    {
                        crouchAttackMed = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.C))
                {
                    if (!oldKeyBoardState.IsKeyDown(Keys.C))
                    {
                        shieldActivation = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.X) && crouch == false && isOnGround == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.X))
                    {
                        middleAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.Y) && crouch == false && isOnGround == false)
                {
                    if (!oldPadState.IsButtonDown(Buttons.Y))
                    {
                        overAirAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.B))
                {
                    if (!oldPadState.IsButtonDown(Buttons.B))
                    {
                        shieldActivation = true;
                    }
                }

                if (gamePadState.Triggers.Right > 0.2f)
                {
                    if (oldPadState.Triggers.Right < 0.2f)
                    {
                        shieldActivation = true;
                    }
                }


                if (gamePadState.IsButtonDown(Buttons.X) && crouch == false && isOnGround == false)
                {
                    if (!oldPadState.IsButtonDown(Buttons.X))
                    {
                        airAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.Y) && crouch == false && isOnGround == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.Y))
                    {
                        overAttack = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.X) && crouch == true
                    && IsOnGround == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.X) && crouch == true
                        && IsOnGround == true)
                    {
                        crouchAttackLow = true;
                    }
                }

                if (gamePadState.IsButtonDown(Buttons.Y) && crouch == true
                    && IsOnGround == true)
                {
                    if (!oldPadState.IsButtonDown(Buttons.Y) && crouch == true
                        && IsOnGround == true)
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

            if (gamePadState.IsButtonDown(Buttons.X))
            {
                middleAttackTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (middleAttackTimer >= chargeTime)
                {
                    middleAttackCharged = true;
                }
            }
            else if (gamePadState.IsButtonUp(Buttons.X) && middleAttackCharged == true &&
                    (gamePadState.IsConnected))
            {
                middleAttackChargedRelease = true;
            }
            else if (keyboardState.IsKeyDown(Keys.Z))
            {
                middleAttackTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (middleAttackTimer >= chargeTime)
                {
                    middleAttackCharged = true;
                }
            }
            else if (keyboardState.IsKeyUp(Keys.Z) && middleAttackCharged == true)
            {
                middleAttackChargedRelease = true;
            }
            else if (keyboardState.IsKeyUp(Keys.Z))
            {
                middleAttackTimer = 0;
            }

            else if (gamePadState.IsButtonUp(Buttons.X))
            {
                middleAttackTimer = 0;
            }

            if (gamePadState.IsButtonDown(Buttons.Y))
            {
                overAttackTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (overAttackTimer >= chargeTime)
                {
                    overAttackCharged = true;
                }
            }
            else if (gamePadState.IsButtonUp(Buttons.Y) && overAttackCharged == true &&
             (gamePadState.IsConnected))
            {
                overAttackChargedRelease = true;
            }
            else if (keyboardState.IsKeyDown(Keys.X))
            {
                overAttackTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (overAttackTimer >= chargeTime)
                {
                    overAttackCharged = true;
                }
            }
            else if (keyboardState.IsKeyUp(Keys.X) && overAttackCharged == true)
            {
                overAttackChargedRelease = true;
            }
            else if (keyboardState.IsKeyUp(Keys.X))
            {
                overAttackTimer = 0;
            }
            else if (gamePadState.IsButtonUp(Buttons.Y))
            {
                overAttackTimer = 0;
            }

            oldKeyBoardState = keyboardState;
            oldPadState = gamePadState;
        }

        //LADDER
        private bool IsAlignedToLadder()
        {
            int playerOffset = ((int)position.X % Tile.Width) - Tile.Centre;
                
                if (Math.Abs(playerOffset) <= LadderAlignment &&
                level.GetTileCollisionBelowPlayer(new Vector2(
                position.X,
                position.Y + 1)) == TileCollision.Ladder ||
                level.GetTileCollisionBelowPlayer(new Vector2(
                position.X,
                position.Y - 1)) == TileCollision.Ladder)
            {
                // Align the player with the middle of the tile
                //position.X -= playerOffset;
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PlayerMovement(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

             Vector2 previousPosition = Position;

            if (IsOnGround == true || isOnLadder == true)
            {
                velocity.Y = 0;
            }

            if (crouch == true || stopMovement == true /*||
                overAttack == true || middleAttack == true*/
             || crouchAttackLow == true || crouchAttackMed == true)
            {
                movement = Vector2.Zero;
            }

            if (stopMovement == true)
            {
                isJumpButtonDown = false;
                isJumpButtonUp = false;
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

            if (isOnLadder == true)
            {
                velocity.Y = movement.Y * ladderAcceleration * elapsed;
            }
            else
            {
                velocity.Y = MathHelper.Clamp(
                       velocity.Y + GravityAcceleration * elapsed,
                       -MaxFallSpeed,
                       MaxFallSpeed);
            }

            position += velocity * elapsed;

            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            PlayerKnockBack(gameTime);
            //FallDamage();

            isJumpButtonDown = false;
            isJumpButtonUp = false;
        }


        private void PlayerKnockBack(GameTime gameTime)
        {
             float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

             if (isHit == true)
             {
                 isHit = false;
                 IsKnockedBack = true;
             }
        }

        static Vector2 CircleToSquare(Vector2 point)
        {
             return CircleToSquare(point, 2);
        }


        static Vector2 CircleToSquare(Vector2 point, double innerRoundness)
        {
            const float PiOverFour = MathHelper.Pi / 4;
            // Determine the theta angle
            var angle = Math.Atan2(point.Y, point.X) + MathHelper.Pi;
            Vector2 squared;
            // Scale according to which wall we're clamping to
            // X+ wall
            if (angle <= PiOverFour || angle > 7 * PiOverFour)
            squared = point * (float)(1 / Math.Cos(angle));
            // Y+ wall
            else if (angle > PiOverFour && angle <= 3 * PiOverFour)
            squared = point * (float)(1 / Math.Sin(angle));
            // X- wall
            else if (angle > 3 * PiOverFour && angle <= 5 * PiOverFour)
            squared = point * (float)(-1 / Math.Cos(angle));
            // Y- wall
            else if (angle > 5 * PiOverFour && angle <= 7 * PiOverFour)
            squared = point * (float)(-1 / Math.Sin(angle));
            else throw new InvalidOperationException("Invalid angle...?");
            // Early-out for a perfect square output

            if (innerRoundness == 0)
                return squared;
            // Find the inner-roundness scaling factor and LERP

            var length = point.Length();

            var factor = (float) Math.Pow(length, innerRoundness);

            return Vector2.Lerp(point, squared, factor);
        }

        private void ChargeAttackColour()
        {

            if (middleAttackCharged && overAttackCharged)
            {
                color = Color.Yellow;
            }
            else if (middleAttackCharged == true)
            {
                color = Color.Blue;
            }
            else if (overAttackCharged == true)
            {
                color = Color.Green;
            }
            else if(IsKnockedBack == false)
            {
                color = Color.White;
            }
        }


        private void FallDamage()
        {
            if (IsOnGround == false)
                fallDepth += Math.Sign(velocity.Y);


            if (fallDepth > 200 && isOnGround == true)
            {
                strikes -= 2;
                fallDepth = 0.0f;
            }
            else if (fallDepth > 50 && IsOnGround == true)
            {
                strikes -= 1;
                fallDepth = 0;
            } 
            else if(IsOnGround == true || isOnLadder == true)
                fallDepth = 0;
        }

        private void PlayerAnimation(GameTime gameTime)
        {
            shieldIconDraw = true;

            //if (IsOnGround)
            //{
            if (airAttack == true && sprite.AnimationFinished == true || 
                airAttack == true && IsOnGround == true)
            {

                if (airAttack == true)
                {
                    middleAttackCharged = false;
                }

                airAttack = false;
                sprite.AnimationFinished = true;
                attackBounds = new Rectangle();
            }


            if (overAirAttack == true && sprite.AnimationFinished == true ||
                overAirAttack == true && IsOnGround == true)
            {

                if (overAirAttack == true)
                {
                    overAttackCharged = false;
                }

                overAirAttack = false;
                sprite.AnimationFinished = true;
                attackBounds = new Rectangle();
            }

                if (sprite.AnimationFinished == true || IsOnGround == false)
                {
                  middleAttackCharged = false;
                  middleAttackChargedRelease = false;

                  overAttackCharged = false;
                  overAttackChargedRelease = false;

                    middleAttack = false;
                    overAttack = false;
                    crouchAttackLow = false;
                    crouchAttackMed = false;

                    attackBounds = new Rectangle();
                }

                if (activationTime >= 500.0f)
                {
                    shieldActivation = false;
                    shieldDraw = false;
                    activationTime = 0.0f;

                    shieldBounds = new Circle();

                    coolDown = true;
                }

                if (coolDown == true)
                {
                    coolDownTime += gameTime.ElapsedGameTime.Milliseconds;
                    shieldActivation = false;
                    shieldIconDraw = false;

                    if (coolDownTime >= 1500.0f) 
                    {
                        coolDown = false;
                        coolDownTime = 0.0f;
                    }
                }
                else  if (shieldActivation == true)
                {
                    if (shieldDraw == false)
                       initShield = true;

                    shieldIconDraw = false;
                    shieldDraw = true;
                    shieldBounds = new Circle(new Vector2(Position.X, Position.Y - 100), shield.Width / 2.0f);
                    activationTime += gameTime.ElapsedGameTime.Milliseconds;
                    attackPoints = 20;
                }


                if (middleAttackChargedRelease == true && isOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                  80, 34);
                    attackPoints = 60;
                }
                else if (middleAttack == true && isOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                  80, 34);
                   attackPoints = 20;
                }
                else if (overAttackChargedRelease == true && isOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      40, 100);
                    attackPoints = 60;
                }
                else if (overAttack == true && isOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      40, 100);

                    attackPoints = 20;
                }
                else if (airAttack == true && isOnGround == false)
                {
                    localBounds = airAttackBounds;
                    sprite.PlayAnimation(airAttackAnimation);

                    if (flip == SpriteEffects.None)
                    {
                        AttackPosition = new Vector2(Position.X + 20, Position.Y - 80);
                    }
                    else
                    {
                        AttackPosition = new Vector2(Position.X - 70, Position.Y - 80);
                    }

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      60, 24);

                    if (middleAttackCharged == true)
                    {
                        attackPoints = 60;
                    }
                    else
                    {
                        attackPoints = 20;
                    }
                }
                else if (overAirAttack == true && isOnGround == false)
                {
                    localBounds = overAirAttackBounds;
                    sprite.PlayAnimation(overAirAttackAnimation);

                    if (flip == SpriteEffects.None)
                    {
                        AttackPosition = new Vector2(Position.X + 10, Position.Y - 140);
                    }
                    else
                    {
                        AttackPosition = new Vector2(Position.X - 60, Position.Y - 140);
                    }

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      60, 140);

                    if (overAttackCharged == true)
                    {
                        attackPoints = 60;
                    }
                    else
                    {
                        attackPoints = 20;
                    }
                }
                else if (crouchAttackLow == true && IsOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      25, 25);

                    attackPoints = 10;
                }
                else if (crouchAttackMed == true && IsOnGround == true)
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

                    attackBounds = new Rectangle((int)AttackPosition.X, (int)AttackPosition.Y,
                                                      64, 16);

                    attackPoints = 15;
                }
                else if (IsHit == true)
                {
                    localBounds = idleBounds;
                    sprite.PlayAnimation(idleAnimation);
                }
                else if (crouch == true && IsOnGround == true)
                {
                    localBounds = crouchBounds;
                    sprite.PlayAnimation(crouchAnimation);
                }
                else if (Math.Abs(velocity.X) - 0.02f > 5 && IsOnGround == true)
                {
                    localBounds = runBounds;
                    sprite.PlayAnimation(runAnimation);
                }
                //else if (IsClimbing == true)
                //{

                //}
                else if(IsOnGround == true)
                {
                    localBounds = idleBounds;
                    sprite.PlayAnimation(idleAnimation);
                }
            //}
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

            if (IsAlignedToLadder() == false)
            {
                isOnLadder = false;
            }

              // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    TileCollision collision = level.GetCollision(x, y);

                    if (collision != TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = level.GetBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            if (collision == TileCollision.Ladder && movement.Y > 0.25f 
                                && IsAlignedToLadder() == true)
                            {
                                isOnLadder = true;
                            }

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX && collision != TileCollision.Patrol
                             || collision == TileCollision.Platform)
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top && collision != TileCollision.Ladder)
                                {
                                    isOnGround = true;
                                    camera.MoveTrapUp(tileBounds.Top);
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

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            float elasped = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

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
                              camera.TransformMatrix);


            //DebugDrawer.DrawCircle(spriteBatch, ShieldBounds, Color.Red, 2);
            if (shieldDraw == true)
            {
                spriteBatch.Draw(shield, new Vector2(Position.X, Position.Y - 100), null, Color.White, 0.0f,
                                              shieldOrigin, 1.0f, SpriteEffects.None, 1.0f);
            }

            //spriteBatch.Draw(healthBarImage, new Vector2(camera.Position.X + 20,
            //                 camera.Position.Y + 50), healthRectangle, Color.White);


            for (int i = 0; i < strikes; i++)
            {
                spriteBatch.Draw(strikeImage, new Vector2(camera.Position.X + 20 +(i * 40), camera.Position.Y + 60), 
                                                          null, Color.White, 0.0f, strikeOrigin, 1.0f, 
                                                          SpriteEffects.None, 1.0f);
            }

            if (shieldIconDraw == true)
            {
                spriteBatch.Draw(shieldIcon, new Vector2(camera.Position.X + 150,
                            camera.Position.Y + 50), null, Color.White);
            }
           

            sprite.Draw(gameTime, spriteBatch, Position, 0.0f, flip, color);

            //spriteBatch.Draw(block, BoundingRectangle, Color.White);
            //spriteBatch.Draw(block, AttackBounds, Color.White);
            //spriteBatch.Draw(block, camera.CameraTrap, Color.White);

            spriteBatch.End();
        }
    }
}