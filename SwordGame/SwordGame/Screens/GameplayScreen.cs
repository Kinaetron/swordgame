using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace SwordGame
{
    class GameplayScreen : GameScreen
    {
        ContentManager content;
        SpriteFont gameFont;

        private int levelIndex = -1;
        private Level level;

        private float revive;

        public TileMap tileMap = new TileMap();
        public Camera camera = new Camera();
        private SpriteBatch spriteBatch;

        KeyboardState previousKeyBoardState;

        float pauseAlpha;

        public int PlayerHealth
        {
            get { return playerHealth; }
            set { playerHealth = value; if (playerHealth > 100) playerHealth = 100; }
        }
        private int playerHealth;

        public int PlayerStrikes
        {
            get { return playerStrikes; }
            set { playerStrikes = value; if (playerStrikes > 2) playerStrikes = 2; }
        }
        private int playerStrikes;

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            spriteBatch = new SpriteBatch(ScreenManager.GraphicsDevice);

            gameFont = content.Load<SpriteFont>("Menu/gamefont");

            ScreenManager.Game.ResetElapsedTime();

            LoadNextLevel();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);

            // Gradually fade in or out depending on whether we are covered by the pause screen.
            if (coveredByOtherScreen)
                pauseAlpha = Math.Min(pauseAlpha + 1f / 32, 1);
            else
                pauseAlpha = Math.Max(pauseAlpha - 1f / 32, 0);

            if (IsActive)
            {
                level.Update(gameTime, tileMap, camera);

                if (level.Player.IsAlive == false)
                {
                    revive += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (revive > 200)
                    {
                        ReloadCurrentLevel();
                        revive = 0.0f;
                    }
                }
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input, GameTime gameTime)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            // Look up inputs for the active player profile.
            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            level.HandleInput(input, gameTime);

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
            }

            if (level.ReachedExit)
            {
                LoadNextLevel();
            }

            if (keyboardState.IsKeyDown(Keys.K))
                if (previousKeyBoardState.IsKeyUp(Keys.K))
                    LoadNextLevel();

            if (keyboardState.IsKeyDown(Keys.J))
                if (previousKeyBoardState.IsKeyUp(Keys.J))
                    LoadPreviousLevel();

            previousKeyBoardState = keyboardState;
        }

        private void LoadNextLevel()
        {
            // Find the path of the next level.
            string levelPath;
            bool fileFound = false;

            // Loop here so we can try again when we can't find a level.
            while (true)
            {
                // Try to find the next level. They are sequentially numbered txt files.
                levelPath = String.Format("Maps/{0}.xnb", ++levelIndex);
                levelPath = "Content/" + levelPath;

                try
                {
                    StreamReader stream = new StreamReader(TitleContainer.OpenStream(levelPath));
                    fileFound = true;
                }
                catch
                {
                    fileFound = false;
                }

                if (fileFound)
                    break;

                // If there isn't even a level 0, something has gone wrong.
                if (levelIndex == 0)
                    throw new Exception("No levels found.");
                else
                {
                    levelIndex = 0;
                    break;
                }
            }

            tileMap = content.Load<TileMap>(String.Format("Maps/{0}", levelIndex));
            // Load the level.
            level = new Level(content, tileMap, this, ScreenManager, camera, levelIndex);
        }

        private void ReloadCurrentLevel()
        {
            --levelIndex;
            LoadNextLevel();
        }

        private void LoadPreviousLevel()
        {
            levelIndex = levelIndex - 2;
            LoadNextLevel();
        }
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            //tileMap.Layers[0].Draw(spriteBatch, camera);


            Point min = Engine.ConvertPositionToCell(camera.Position);
            Point max = Engine.ConvertPositionToCell(
            camera.Position + new Vector2(
            ScreenManager.GraphicsDevice.Viewport.Width  + Engine.TileWidth,
            ScreenManager.GraphicsDevice.Viewport.Height + Engine.TileHeight));


            tileMap.Layers[0].Draw(spriteBatch, camera, min, max);

            level.Draw(gameTime, spriteBatch, camera);

            if (TransitionPosition > 0 || pauseAlpha > 0)
            {
                float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, pauseAlpha / 2);

                ScreenManager.FadeBackBufferToBlack(alpha);
            }
        }
        private void DrawShadowedString(SpriteFont font, string value, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, value, position + new Vector2(1.0f, 1.0f), Color.Black);
            spriteBatch.DrawString(font, value, position, color);
        }
    }
}
