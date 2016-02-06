using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate
{
    class Player : CharacterBase
    {
        public Player(Texture2D texture) : base(texture)
        {
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32),
            });
            CharacterSprite.AddFrames("walking", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32),
                new Rectangle(32, 32, 32, 32),
                new Rectangle(64, 32, 32, 32),
                new Rectangle(96, 32, 32, 32),
            });
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 64, 32, 32)
            });

            Position = new Vector2(50, 160);
        }

        public override void Update(GameTime gameTime)
        {
            CheckKeys(gameTime);
            base.Update(gameTime);
        }

        private void CheckKeys(GameTime gameTime)
        {
            var elapsed = gameTime.ElapsedGameTime.TotalMilliseconds;

            if (InputManager.Instace.KeyDown(Keys.Left))
            {
                CharacterSprite.SetDirection(SpriteDirection.Left);
                _movement = -1.0f;
            }
            else if (InputManager.Instace.KeyDown(Keys.Right))
            {
                CharacterSprite.SetDirection(SpriteDirection.Right);
                _movement = 1.0f;
            }

            _isJumping = InputManager.Instace.KeyDown(Keys.Up);
        }
    }
}
