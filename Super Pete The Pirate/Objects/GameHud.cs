using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.BitmapFonts;
using Super_Pete_The_Pirate.Scenes;
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

        //--------------------------------------------------
        // Ammo text

        private Color _ammoTextColor;

        //----------------------//------------------------//

        public GameHud(Texture2D texture) : base(texture)
        {
            _position = Vector2.Zero;

            _hearthSprite = new Rectangle(0, 0, 11, 10);
            _lifeSprite = new Rectangle(0, 10, 13, 14);
            _gunSprite = new Rectangle(0, 24, 20, 9);

            _hearthSpritePosition = new Vector2(0, 0);
            _lifeSpritePosition = new Vector2(0, 11);
            _gunSpritePosition = new Vector2(0, 26);

            _ammoTextColor = new Color(23, 34, 68);
        }

        public void SetPosition(Vector2 position)
        {
            _position = position;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var sceneMap = (SceneMap)SceneManager.Instance.GetCurrentScene();
            var player = sceneMap.Player;

            // Draw the hearths
            for (var i = 0; i < player.HP; i++)
            {
                spriteBatch.Draw(this.TextureRegion.Texture,
                    _position + _hearthSpritePosition + (Vector2.UnitX * i * (_hearthSprite.Width + 2)), _hearthSprite, Color.White);
            }

            // Draw the lives
            for (var i = 0; i < player.Lives; i++)
            {
                spriteBatch.Draw(this.TextureRegion.Texture,
                    _position + _lifeSpritePosition + (Vector2.UnitX * i * (_lifeSprite.Width + 2)), _lifeSprite, Color.White);
            }

            // Draw the ammo
            spriteBatch.Draw(this.TextureRegion.Texture, _position + _gunSpritePosition, _gunSprite, Color.White);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, player.Ammo.ToString(),
                _position + _gunSpritePosition + (Vector2.UnitX * (_gunSprite.Width + 2)), _ammoTextColor);
        }
    }
}
