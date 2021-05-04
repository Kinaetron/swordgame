using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SwordGame
{
    class Rumble
    {
        private float rumbleTime;
        private float rumbleStrength;
        private float timer;
        private bool startRumble;

        public Rumble() { }

        public void RumbleSetup(float rumbleTime, float rumbleStrength)
        {
            this.rumbleTime = rumbleTime;
            this.rumbleStrength = rumbleStrength;

            startRumble = true;
        }

        public void TurnOffRumble()
        {
            timer = rumbleTime;
        }

        public void Update(GameTime gameTime)
        {
            if (startRumble == true)
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                GamePad.SetVibration(PlayerIndex.One, rumbleStrength, rumbleStrength);

                if (timer >= rumbleTime)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
                    startRumble = false;
                    timer = 0;
                }
            }
        }
    }
}
