using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    enum FaceDirection
    {
        Left = -1,
        Right = 1,
    }

    class Enemy
    {
        protected float timer;
        protected bool startTimer;

        protected bool startChase;

        protected Texture2D block;

        protected Level level;

        protected AnimationPlayer sprite;
        protected Animation enemyAnimation;

        protected Rectangle localBounds;

        protected float MoveSpeed = 250.0f;

        protected Vector2 velocity;

        protected LiangBarsky enemyToChar;

        protected bool wallInWay;

        public bool StopEnemy
        {
            get { return stopEnemy; }
            set { stopEnemy = value; }
        }
        bool stopEnemy;

        public Color EnemyColor
        {
            get { return enemyColor; }
            set { enemyColor = value; }
        }
        Color enemyColor;

        public FaceDirection Direction
        {
            get { return direction; }
        }
       protected FaceDirection direction = FaceDirection.Right;


       public int HealthDrop
       {
           get { return healthDrop; }
       }
       protected int healthDrop;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        protected int maxHealth = 100;

        public int Health
        {
            get { return health; }
            set { health = value; if (health > maxHealth) health = maxHealth; }
        }
        int health = 85;


        public bool PlayerHitEnemy
        {
            get { return playerHitEnemy; }
            set { playerHitEnemy = value; }
        }
        private bool playerHitEnemy;

        public bool ShieldHit
        {
            get { return shieldHit; }
            set { shieldHit = value; }
        }
        private bool shieldHit;


        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public Enemy(Level level, Vector2 position)
        {
            this.level = level;
            Position = position; 
        }

        protected virtual void LoadContent()
        {
        }

        public virtual void Update(GameTime gameTime)
        {
        }

        public virtual void HandleCollisions(GameTime gameTime)
        {
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
        }
    }
}
