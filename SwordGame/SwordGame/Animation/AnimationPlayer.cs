using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    struct AnimationPlayer
    {
        ///<summary>
        /// Gets the animation which is currently playing.
        ///</summary>
        public Animation Animation
        {
            get { return animation; }
        }
        Animation animation;

        /// <summary>
        /// Gets the index  of the current frame in the animation
        /// </summary>
        public int FrameIndex
        {
            get { return frameIndex; }
        }
        int frameIndex;

        ///<summary>
        /// The amount of time in seconds that the current frame has been shown for.
        ///<\summary>
        private float time;

        ///<summary>
        /// Gets a texture origin at the bottom center of each time.
        /// <\summary>
        public Vector2 Origin
        {
            get { return new Vector2(Animation.FrameWidth / 2.0f, Animation.FrameHeight); }
        }

        /// <summary>
        /// Gets the boolean which tells states if the animation has fully played
        /// </summary>
        public bool AnimationFinished
        {
            get { return animationFinished; }
            set { animationFinished = value; }
        }
        bool animationFinished;


        public bool AnimationStarted
        {
            get { return animationStarted; }
            set { animationStarted = value; }
        }
        bool animationStarted;


        /// <summary>
        /// Begins or continues playback of an animation.
        /// </summary>
        public void PlayAnimation(Animation animation)
        {

            // If animation is already running, do not restart it.
            if (Animation == animation)
            {
                return;
            }

            this.animation = animation;
            this.frameIndex = 0;
            this.time = 0;
        }

        /// <summary>
        /// Advances the time position and draws the current frame for the animation.
        /// <\summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch, Vector2 position, float rotation,
                         SpriteEffects spriteEffects, Color color = default(Color))
        {
            if (Animation == null)
            {
                throw new NotSupportedException("No animation is currently playing.");
            }

            if (object.Equals(color, default(Color)))
                color = Color.White;

            AnimationFinished = false;
            AnimationStarted = false;

            // Process passing time.
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (time > Animation.FrameTime)
            {
                time -= Animation.FrameTime;

                // Advance the frame index; looping or clamping as appropriate
                if (Animation.IsLooping)
                {
                    frameIndex = (frameIndex + 1) % Animation.FrameCount;

                    if (frameIndex == 0)
                    {
                        AnimationFinished = true;
                    }


                    if (frameIndex == 1)
                    {
                        AnimationStarted = true;
                    }
                }
                else
                {
                    frameIndex = Math.Min(frameIndex + 1, Animation.FrameCount - 1);

                    if (frameIndex == 0)
                    {
                        AnimationFinished = true;
                    }

                    if (frameIndex == 1)
                    {
                        AnimationStarted = true;
                    }
                }
            }

            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(FrameIndex * Animation.FrameWidth, 0,
                                             Animation.FrameWidth, Animation.FrameHeight);

            // Draw the current frame. 
            spriteBatch.Draw(Animation.Texture, position, source, color, rotation,
                             Origin, 1.0f, spriteEffects, 0.0f);
        }
    }
}
