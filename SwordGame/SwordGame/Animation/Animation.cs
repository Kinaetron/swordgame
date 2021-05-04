using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace SwordGame
{
    class Animation
    {
        /// <summary>
        /// All frames in the animation arranged horizontally.
        /// <summary>
        public Texture2D Texture
        {
            get { return texture; }
        }
        Texture2D texture;

        /// <summary>
        /// Duration of time to show each frame.
        /// </summary>
        public float FrameTime
        {
            get { return frameTime; }
        }
        float frameTime;

        /// <summary>
        /// When the end of the animation is reached, should it 
        /// continue playing from the beginning ? 
        public bool IsLooping
        {
            get { return isLooping; }
        }
        bool isLooping;

        /// <summary>
        /// Gets the number of frames in the animation
        /// </summary>
        public int FrameCount
        {
            get { return Texture.Width / FrameWidth; }
        }

        /// <summary>
        /// Gets the width of a frame in the animation.
        /// </summary>
        public int FrameWidth
        {
            get { return frameWidth; }
        }
        int frameWidth;

        /// <summary>
        /// Gets the height of a frame in the animation.
        /// </summary>
        public int FrameHeight
        {
            get { return Texture.Height; }
        }
        /// <summary>
        /// Constructors a new animation
        /// <\summary>
        public Animation(Texture2D texture, float frameTime, int frameWidth, bool isLooping)
        {
            this.texture = texture;
            this.frameTime = frameTime;
            this.frameWidth = frameWidth;
            this.isLooping = isLooping;
        }
    }
}
