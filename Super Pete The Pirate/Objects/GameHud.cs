using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Objects
{
    class GameHud : Sprite
    {
        //--------------------------------------------------
        // Positions

        private Vector2 _position;
        private Vector2 _hearthSpritePosition;
        private Vector2 _lifeSpritePosition;
        private Vector2 _gunSpritePosition;

        //--------------------------------------------------
        // Sprites

        private Rectangle _hearthSprite;
        private Rectangle _lifeSprite;
        private Rectangle _gunSprite;

        //----------------------//------------------------//

        public GameHud(Texture2D texture) : base(texture)
        {
            _position = Vector2.Zero;

            _hearthSprite = new Rectangle(0, 0, 11, 10);
            _lifeSprite = new Rectangle(0, 10, 13, 14);
            _gunSprite = new Rectangle(0, 24, 20, 9);

            _hearthSpritePosition = new Vector2(0, 0);
            _lifeSpritePosition = new Vector2(0, 10);
            _gunSpritePosition = new Vector2(0, 24);
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.TextureRegion.Texture, _position + _hearthSpritePosition, _hearthSprite, Color.White);
            spriteBatch.Draw(this.TextureRegion.Texture, _position + _lifeSpritePosition, _lifeSprite, Color.White);
            spriteBatch.Draw(this.TextureRegion.Texture, _position + _gunSpritePosition, _gunSprite, Color.White);
        }
    }
}
