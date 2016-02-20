using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Objects
{
    class GameCoin : PhysicalObject
    {
        //--------------------------------------------------
        // Coin Sprite

        public AnimatedSprite CoinSprite { get; set; }

        //--------------------------------------------------
        // Bounding Rectangle

        public override Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle((int)Position.X + 8, (int)Position.Y + 8, CoinSprite.BoundingBox.Width, CoinSprite.BoundingBox.Height);
            }
        }

        //--------------------------------------------------
        // Physics

        private bool _applyPhysics;
        private float _xAcceleration;

        //----------------------//------------------------//

        public GameCoin(Texture2D texture, Rectangle[] frames, int delay, int x, int y, Vector2 velocity, bool applyPhysics = false)
        {
            CoinSprite = new AnimatedSprite(texture, frames, delay, x, y, true);
            Position = new Vector2(x, y);
            Velocity = velocity;
            _applyPhysics = applyPhysics;
            _xAcceleration = 0f;
        }

        public void SetXAcceleration(float acceleration)
        {
            _xAcceleration = acceleration;
        }

        public override void Update(GameTime gameTime)
        {
            if (_applyPhysics)
            {
                if (_xAcceleration != 0f)
                {
                    _velocity.X += _xAcceleration;
                    _xAcceleration *= 0.96f;
                }
                base.Update(gameTime);
            }
            CoinSprite.Position = Position;
            CoinSprite.Update(gameTime);
        }
    }
}
