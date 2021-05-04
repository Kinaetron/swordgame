using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace SwordGame
{
    class Level
    {
        private int globalX;
        private int globalY;
       
        private SpriteFont menuFont;
        private float elapsed;
        private int totalFrames;
        int fps;

        TileMap tileMap;

        public Player Player
        {
            get { return player; }
        }
        Player player;

        public Camera Camera
        {
            get { return camera; }
        }
        Camera camera;

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
        }
        ScreenManager screenManager;

        public Rumble Rumble
        {
            get { return rumble; }
        }
        Rumble rumble = new Rumble();

        public Tile[,] tiles;

        private Vector2 start;

        private List<Point> exits = new List<Point>();

        private List<CrawlEnemy> crawlies = new List<CrawlEnemy>();
        private List<FlyingEnemy> flyingEnemies = new List<FlyingEnemy>();
        private List<PatrolEnemy> patrolEnemies = new List<PatrolEnemy>();
        private List<Turret> turretEnemies = new List<Turret>();
        private List<ShootEnemy> shooterEnemies = new List<ShootEnemy>();

        GameplayScreen screen;

        public bool ReachedExit
        {
            get { return reachedExit; }
        }
        bool reachedExit;

        // Level content.        
        public ContentManager Content
        {
            get { return content; }
        }
        ContentManager content;

        private List<Health25> healths25 = new List<Health25>();

        private List<Health100> healths100 = new List<Health100>();

        public Level(ContentManager serviceProvider, TileMap tileMap, GameplayScreen gameScreen,
                     ScreenManager screenMan, Camera camera, int LevelNo)
        {
            this.camera = camera;
            this.tileMap = tileMap;
            screenManager = screenMan;

            screen = gameScreen;

            //DebugDrawer.LoadContent(screenManager.GraphicsDevice);

            content = serviceProvider;

            menuFont = content.Load<SpriteFont>("Debug/font");

            foreach (CollisionLayer layer in tileMap.colLayers)
                LoadTiles(layer);

            player = new Player(this, start, camera, tileMap);

            if (screen.PlayerStrikes > 0)
            {
                player.Strikes = screen.PlayerStrikes;
            }
        }

        private void LoadTiles(CollisionLayer colLayer)
        {
            globalX = colLayer.Width;
            globalY = colLayer.Height;

            tiles = new Tile[colLayer.Width, colLayer.Height];

            for (int x = 0; x < colLayer.Width; ++x)
                for (int y = 0; y < colLayer.Height; ++y)
                    tiles[x, y] = LoadTile(colLayer.GetCellIndex(x, y), x, y);

            //if (exits.Count == 0)
            //    throw new NotSupportedException("A level must have an exit.");
        }

        private Tile LoadTile(string tileType, int x, int y)
        {
            int tileNumberType;
            bool isNum = int.TryParse(tileType, out tileNumberType);
            string[] numbers = null;

            int addtionalNumber = 0;

            if (!isNum)
            {
                numbers = tileType.Split('#');
                tileNumberType = int.Parse(numbers[0]);
                addtionalNumber = int.Parse(numbers[1]);
            }

            switch (tileNumberType)
            {
                // Blank space
                case 0:
                    return new Tile(null, TileCollision.Passable);

                case 1:
                    return new Tile(null, TileCollision.Impassable);

                // Exit
                case 2:
                    return new Tile(null, TileCollision.NormalTile);

                case 3:
                    return new Tile(null, TileCollision.Platform);

                case 4:
                    return new Tile(null, TileCollision.Ladder);

                case 5:
                    return new Tile(null, TileCollision.Patrol);

                case 8:
                    return LoadStartTile(x, y);

                // Player  1 start point
                case 9:
                    return LoadExitTile(x, y);

                case 10:
                    return LoadHealthTile25(x, y);

                case 11:
                    return LoadCrawlEnemy(x, y, addtionalNumber);

                case 13:
                    return LoadFlyEnemy(x, y, addtionalNumber);

                case 14:
                    return LoadPatrolEnemy(x, y, addtionalNumber);

                case 15:
                    return LoadHealthTile100(x, y);

                case 16:
                    return LoadTurretEnemy(x, y, addtionalNumber);

                case 17:
                    return LoadShooterEnemy(x, y, int.Parse(numbers[1]));

                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private Tile LoadStartTile(int x, int y)
        {
            //if (Player != null)
            //    throw new NotSupportedException("A level may only have one starting point.");
            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            return new Tile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private Tile LoadExitTile(int x, int y)
        {
            exits.Add(GetBounds(x, y).Center);
            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadHealthTile25(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            healths25.Add(new Health25(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadHealthTile100(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            healths100.Add(new Health100(this, new Vector2(position.X, position.Y)));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadCrawlEnemy(int x, int y, int direction)
        {
            Point position = GetBounds(x, y).Center;
            crawlies.Add(new CrawlEnemy(this, new Vector2(position.X, position.Y), direction));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadFlyEnemy(int x, int y, int lootDrop)
        {
            Point position = GetBounds(x, y).Center;
            flyingEnemies.Add(new FlyingEnemy(this, new Vector2(position.X, position.Y), lootDrop));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadPatrolEnemy(int x, int y, int direction)
        {
            Point position = GetBounds(x, y).Center;
            patrolEnemies.Add(new PatrolEnemy(this, new Vector2(position.X, position.Y), direction));

            return new Tile(null, TileCollision.Passable);
        }


        private Tile LoadTurretEnemy(int x, int y, int lootDrop)
        {
            Point position = GetBounds(x, y).Center;
            turretEnemies.Add(new Turret(this, new Vector2(position.X, position.Y), lootDrop));

            return new Tile(null, TileCollision.Passable);
        }

        private Tile LoadShooterEnemy(int x, int y, int direction)
        {
            Point position = GetBounds(x, y).Center;
            shooterEnemies.Add(new ShootEnemy(this, new Vector2(position.X, position.Y), direction));

            return new Tile(null, TileCollision.Passable);
        }

        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= tileMap.GetWidth())
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= tileMap.GetHeight())
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        public TileCollision GetTileCollisionBehindPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y - 1) / Tile.Height;

            // Prevet escaping past the level ends.
            if (x == tileMap.GetWidth())
            {
                return TileCollision.Impassable;
            }

            // Allow jumping past the level top and falling through the bottom.
            if (y == tileMap.GetHeight())
            {
                return TileCollision.Passable;
            }

            return tiles[x, y].Collision;
        }

        public TileCollision GetTileCollisionBelowPlayer(Vector2 playerPosition)
        {
            int x = (int)playerPosition.X / Tile.Width;
            int y = (int)(playerPosition.Y) / Tile.Height;

            if (x == tileMap.GetWidth())
            {
                return TileCollision.Impassable;
            }

            // Allow jumping past the level top and falling through the bottom.
            if (y == tileMap.GetHeight())
            {
                return TileCollision.Passable;
            }

            return tiles[x, y].Collision;
        }
        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * Tile.Width, y * Tile.Height, Tile.Width, Tile.Height);
        }
          /// <summary>,
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime, TileMap tileMap, Camera camera)
        {
            elapsed += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (elapsed > 1000.0f)
            {
                fps = totalFrames;
                totalFrames = 0;
                elapsed = 0;
            }

            Player.Update(gameTime, tileMap);
            checkEnemyHits();
            UpdateHealths25(gameTime);
            UpdateHealths100(gameTime);
            UpdateCrawlEnemies(gameTime);
            UpdateFlyingEnemies(gameTime);
            UpdatePatrolEnemies(gameTime);
            UpdateTurretEnemies(gameTime);
            UpdateShooterEnemies(gameTime);

            rumble.Update(gameTime);

            foreach (Point exit in exits)
            {
                if (Player.IsAlive && player.BoundingRectangle.Contains(exit))
                    OnExitReached();
            }
        }

        private void UpdateHealths25(GameTime gameTime)
        {
            for (int i = 0; i < healths25.Count; ++i)
            {
                Health health = healths25[i];

                health.Update(gameTime);

                if (health.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    healths25.RemoveAt(i--);
                    player.Strikes += health.HealthAmount;
                }
            }
        }

        private void UpdateHealths100(GameTime gameTime)
        {
            for (int i = 0; i < healths100.Count; ++i)
            {
                Health health = healths100[i];

                health.Update(gameTime);

                if (health.BoundingCircle.Intersects(Player.BoundingRectangle))
                {
                    healths100.RemoveAt(i--);
                    player.Strikes += health.HealthAmount;
                }
            }
        }

        private void UpdateCrawlEnemies(GameTime gameTime)
        {
            for (int i = 0; i < crawlies.Count; ++i)
            {
                CrawlEnemy crawl = crawlies[i];
                crawl.StopEnemy = false;

                crawl.Update(gameTime);

                if (player.ShieldBounds.Intersects(crawl.BoundingRectangle) &&
                   crawl.ShieldHit == true)
                {
                    crawl.Health -= player.AttackPoints;
                    player.InitShield = false;
                }
                else if (crawl.BoundingRectangle.Intersects(player.AttackBounds) && 
                    crawl.PlayerHitEnemy == false)
                {
                      crawl.Health -= player.AttackPoints;
                      crawl.PlayerHitEnemy = true;
                }

                if (crawl.Health <= 0)
                {
                    if (Math.Abs((int)crawl.HealthDrop) == 2)
                    {
                        healths25.Add(new Health25(this,
                        new Vector2(crawl.Position.X, crawl.Position.Y)));
                    }
                    else if (Math.Abs(crawl.HealthDrop) == 3)
                    {
                        healths100.Add(new Health100(this, crawl.Position));
                    }

                    crawlies.RemoveAt(i--);
                }
            }
        }


        private void UpdateFlyingEnemies(GameTime gameTime)
        {
            for (int i = 0; i < flyingEnemies.Count; ++i)
            {
                FlyingEnemy fly = flyingEnemies[i];
                fly.StopEnemy = false;

                fly.Update(gameTime);

                if (player.ShieldBounds.Intersects(fly.BoundingRectangle) &&
                    fly.ShieldHit == true)
                {
                    fly.Health -= player.AttackPoints;
                    player.InitShield = false;
                }
                else if (fly.BoundingRectangle.Intersects(player.AttackBounds) &&
                    fly.PlayerHitEnemy == false )
                {
                    fly.Health -= player.AttackPoints;
                    fly.PlayerHitEnemy = true;
                }

                if (fly.Health <= 0)
                {

                    if (player.ShieldBounds.Intersects(fly.BoundingRectangle) &&
                        fly.BoundingRectangle.Intersects(player.AttackBounds))
                    {
                        player.CoolDownTime = 1505.0f;
                    }

                    if (Math.Abs((int)fly.HealthDrop) == 2)
                    {
                        healths25.Add(new Health25(this,
                        new Vector2(fly.Position.X, fly.Position.Y)));
                    }
                    else if (Math.Abs(fly.HealthDrop) == 3)
                    {
                        healths100.Add(new Health100(this, fly.Position));
                    }

                    flyingEnemies.RemoveAt(i--);
                }
            }
        }

        private void UpdatePatrolEnemies(GameTime gameTime)
        {

            for (int i = 0; i < patrolEnemies.Count; ++i)
            {
                PatrolEnemy patrol = patrolEnemies[i];
                patrol.Update(gameTime);
                patrol.BeingHit = false;


                if (player.ShieldBounds.Intersects(patrol.BoundingRectangle) &&
                    patrol.ShieldHit == true)
                {
                    patrol.Health -= player.AttackPoints;
                    player.InitShield = false;
                }
                else if (patrol.BoundingRectangle.Intersects(player.AttackBounds) &&
                    patrol.PlayerHitEnemy == false)
                {
                    patrol.Health -= player.AttackPoints;
                    patrol.PlayerHitEnemy = true;
                }

                if (patrol.Health <= 0)
                {
                   if(player.ShieldBounds.Intersects(patrol.BoundingRectangle) &&
                       patrol.BoundingRectangle.Intersects(player.AttackBounds))
                   {
                       player.CoolDownTime = 1505.0f;
                   }

                    if (Math.Abs((int)patrol.HealthDrop) == 2)
                    {
                        healths25.Add(new Health25(this,
                        new Vector2(patrol.Position.X, patrol.Position.Y - 100)));
                    }
                    else if (Math.Abs(patrol.HealthDrop) == 3)
                    {
                        healths100.Add(new Health100(this, patrol.Position));
                    }

                    patrolEnemies.RemoveAt(i--);
                }

                if (patrol.BoundingRectangle.Intersects(player.AttackBounds) ||
                    player.ShieldBounds.Intersects(patrol.BoundingRectangle))
                {
                    patrol.StopEnemy = true;
                    patrol.BeingHit = true;
                    patrol.EnemyColor = Color.Red;
                }
            }
        }

        private void UpdateTurretEnemies(GameTime gameTime)
        {
            for (int i = 0; i < turretEnemies.Count; ++i)
            {
                Turret turret = turretEnemies[i];
                turret.StopEnemy = false;

                turret.Update(gameTime);

                if (player.ShieldBounds.Intersects(turret.BoundingRectangle) &&
                    turret.ShieldHit == true)
                {
                    turret.Health -= player.AttackPoints;
                    player.InitShield = false;
                }
                else if (turret.BoundingRectangle.Intersects(player.AttackBounds) &&
                    turret.PlayerHitEnemy == false)
                {
                    turret.Health -= player.AttackPoints;
                    turret.PlayerHitEnemy = true;
                }


                if (turret.Health <= 0)
                {
                    if (player.ShieldBounds.Intersects(turret.BoundingRectangle) &&
                        turret.BoundingRectangle.Intersects(player.AttackBounds))
                    {
                        player.CoolDownTime = 1505.0f;
                    }

                    if (Math.Abs((int)turret.HealthDrop) == 2)
                    {
                        healths25.Add(new Health25(this,
                        new Vector2(turret.Position.X, turret.Position.Y)));
                    }
                    else if (Math.Abs(turret.HealthDrop) == 3)
                    {
                        healths100.Add(new Health100(this, turret.Position));
                    }

                    turretEnemies.RemoveAt(i--);
                }
            }
        }

        private void UpdateShooterEnemies(GameTime gameTime)
        {

            for (int i = 0; i < shooterEnemies.Count; ++i)
            {
                ShootEnemy shooter = shooterEnemies[i];
                shooter.Update(gameTime);
                shooter.BeingHit = false;


                if (player.ShieldBounds.Intersects(shooter.BoundingRectangle) &&
                    shooter.ShieldHit == true)
                {
                    shooter.Health -= player.AttackPoints;
                    player.InitShield = false;
                }
                else if (shooter.BoundingRectangle.Intersects(player.AttackBounds) &&
                    shooter.PlayerHitEnemy == false)
                {
                    shooter.Health -= player.AttackPoints;
                    shooter.PlayerHitEnemy = true;
                }

                if (shooter.Health <= 0)
                {
                    if (player.ShieldBounds.Intersects(shooter.BoundingRectangle) &&
                      shooter.BoundingRectangle.Intersects(player.AttackBounds))
                    {
                        player.CoolDownTime = 1505.0f;
                    }

                    if (Math.Abs((int)shooter.HealthDrop) == 2)
                    {
                        healths25.Add(new Health25(this,
                        new Vector2(shooter.Position.X, shooter.Position.Y - 100)));
                    }
                    else if (Math.Abs(shooter.HealthDrop) == 3)
                    {
                        healths100.Add(new Health100(this, shooter.Position));
                    }

                    shooterEnemies.RemoveAt(i--);
                }

                if (shooter.BoundingRectangle.Intersects(player.AttackBounds) ||
                   player.ShieldBounds.Intersects(shooter.BoundingRectangle))
                {
                    shooter.StopEnemy = true;
                    shooter.BeingHit = true;
                    shooter.EnemyColor = Color.Red;
                }
                else
                {
                    shooter.StopEnemy = false;
                }
            }
        }


        private void checkEnemyHits()
        {
            foreach (var patrol in patrolEnemies)
            {
                patrol.ShieldHit = false;

                if (player.InitShield == true)
                    patrol.ShieldHit = true;

                if (patrol.PlayerHitEnemy == true)
                {
                    if (player.Sprite.AnimationFinished == true)
                    {
                        patrol.PlayerHitEnemy = false;
                    }
                }
            }

            foreach (var fly in flyingEnemies)
            {
                fly.ShieldHit = false;

                if (player.InitShield == true)
                    fly.ShieldHit = true;


                if (fly.PlayerHitEnemy == true)
                {
                    if (player.Sprite.AnimationFinished == true)
                        fly.PlayerHitEnemy = false;
                }
            }

            foreach (var crawl in crawlies)
            {
                crawl.ShieldHit = false;

                if (player.InitShield == true)
                    crawl.ShieldHit = true;

                if (crawl.PlayerHitEnemy == true)
                {
                    if (player.Sprite.AnimationFinished == true)
                        crawl.PlayerHitEnemy = false;
                }
            }

            foreach (var turret in turretEnemies)
            {
                turret.ShieldHit = false;

                if (player.InitShield == true)
                    turret.ShieldHit = true;

                if (turret.PlayerHitEnemy == true)
                {
                    if (player.Sprite.AnimationFinished == true)
                        turret.PlayerHitEnemy = false;
                }
            }

            foreach (var shooter in shooterEnemies)
            {
                shooter.ShieldHit = false;

                if (player.InitShield == true)
                    shooter.ShieldHit = true;


                if (shooter.PlayerHitEnemy == true)
                {
                    if (player.Sprite.AnimationFinished == true)
                        shooter.PlayerHitEnemy = false;
                }
            }
        }

        /// <summary>
        /// Called when the player reaches the levels exit.
        /// </summary>
        private void OnExitReached()
        {
            screen.PlayerStrikes = player.Strikes;
            reachedExit = true;
        }

        public void HandleInput(InputState input, GameTime gameTime)
        {
            player.HandleInput(input, gameTime);
        }
         /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Camera cam)
        {
            totalFrames++;

            if (tileMap.Layers.Count > 1)
            {
                Point min = Engine.ConvertPositionToCell(camera.Position);
                Point max = Engine.ConvertPositionToCell(
                camera.Position + new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width  + Engine.TileWidth,
                ScreenManager.GraphicsDevice.Viewport.Height + Engine.TileHeight));

                tileMap.Layers[1].Draw(spriteBatch, camera, min, max);
            }

            Player.Draw(gameTime, spriteBatch);

            spriteBatch.Begin(SpriteSortMode.Immediate,
                           BlendState.AlphaBlend,
                           null, null, null, null,
                           cam.TransformMatrix);

            foreach (var crawlEnemy in crawlies)
            {
                crawlEnemy.Draw(gameTime, spriteBatch);
            }

            foreach (var flyingEnemy in flyingEnemies)
            {
                flyingEnemy.Draw(gameTime, spriteBatch);   
            }

            foreach (var patrolEnemy in patrolEnemies)
            {
                patrolEnemy.Draw(gameTime, spriteBatch);
            }

            foreach (var turret in turretEnemies)
            {
                turret.Draw(gameTime, spriteBatch);
            }

            foreach (var shooter in shooterEnemies)
            {
                shooter.Draw(gameTime, spriteBatch);
            }

            foreach (var health in healths25)
            {
                health.Draw(gameTime, spriteBatch);
            }

            foreach (var health in healths100)
            {
                health.Draw(gameTime, spriteBatch);
            }

            spriteBatch.DrawString(menuFont, "Fps : " + fps,
                                 new Vector2(cam.Position.X + 900, cam.Position.Y + 20), Color.Black);

            spriteBatch.End();
        }
    }
}
