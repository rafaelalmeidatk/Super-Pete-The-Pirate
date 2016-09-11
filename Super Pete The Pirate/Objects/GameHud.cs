using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using Super_Pete_The_Pirate.Scenes;

namespace Super_Pete_The_Pirate.Objects
{
    class GameHud : Sprite
    {
        //--------------------------------------------------
        // Positions

        private Vector2 _position;
        private Vector2 _coinsPosition;
        private Vector2 _hearthSpritePosition;
        private Vector2 _lifeSpritePosition;
        private Vector2 _gunSpritePosition;

        //--------------------------------------------------
        // Sprites

        private Rectangle _heartSprite;
        private Rectangle _lifeSprite;
        private Rectangle _gunSprite;
        private Rectangle _coinSprite;

        //--------------------------------------------------
        // Ammo text

        private Color _ammoTextColor;
        private Color _coinsTextColor;

        //----------------------//------------------------//

        public GameHud(Texture2D texture) : base(texture)
        {
            _position = Vector2.Zero;
            _coinsPosition = Vector2.Zero;

            _heartSprite = new Rectangle(0, 0, 11, 10);
            _lifeSprite = new Rectangle(0, 10, 13, 14);
            _gunSprite = new Rectangle(0, 24, 20, 9);
            _coinSprite = new Rectangle(0, 33, 14, 14);

            _hearthSpritePosition = new Vector2(0, 0);
            _lifeSpritePosition = new Vector2(0, 11);
            _gunSpritePosition = new Vector2(0, 26);

            _ammoTextColor = new Color(23, 34, 68);
            _coinsTextColor = new Color(200, 154, 95);
        }

        public void SetPosition(Vector2 position, Vector2 coinsPosition)
        {
            _position = position;
            _coinsPosition = coinsPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var player = ((SceneMap)SceneManager.Instance.GetCurrentScene()).Player;

            // Draw the hearts
            for (var i = 0; i < player.HP; i++)
            {
                spriteBatch.Draw(TextureRegion.Texture,
                    _position + _hearthSpritePosition + (Vector2.UnitX * i * (_heartSprite.Width + 2)), _heartSprite, Color.White);
            }

            // Draw the lives
            for (var i = 0; i < player.Lives; i++)
            {
                spriteBatch.Draw(TextureRegion.Texture,
                    _position + _lifeSpritePosition + (Vector2.UnitX * i * (_lifeSprite.Width + 2)), _lifeSprite, Color.White);
            }

            // Draw the ammo
            spriteBatch.Draw(TextureRegion.Texture, _position + _gunSpritePosition, _gunSprite, Color.White);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, player.Ammo.ToString(),
                _position + _gunSpritePosition + (Vector2.UnitX * (_gunSprite.Width + 2)), _ammoTextColor);

            // Draw the coins
            spriteBatch.Draw(TextureRegion.Texture, _coinsPosition, _coinSprite, Color.White);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, player.Coins.ToString(), 
                _coinsPosition + new Vector2(_coinSprite.Width + 3, 1), _coinsTextColor);
        }
    }
}
