using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;

namespace Super_Pete_The_Pirate.Managers
{
    class IconsManager
    {
        //--------------------------------------------------
        // Singleton variables

        private static IconsManager _instance = null;
        private static readonly object _padlock = new object();

        public static IconsManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new IconsManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // Texture

        private Texture2D _buttonsTexture;

        //--------------------------------------------------
        // Positions

        private Rectangle[] _zButton;
        private Rectangle[] _xButton;
        private Rectangle[] _aButton;
        private Rectangle[] _bButton;
        private Rectangle[] _rightArrow;

        //----------------------//------------------------//

        private IconsManager()
        {
            _buttonsTexture = ImageManager.loadSystem("ButtonsSpritesheet");

            _zButton = new Rectangle[] {
                new Rectangle(0, 0, 15, 15),
                new Rectangle(45, 0, 15, 15)
            };

            _xButton = new Rectangle[]
            {
                new Rectangle(15, 0, 15, 15),
                new Rectangle(60, 0, 15, 15)
            };

            _aButton = new Rectangle[]
            {
                new Rectangle(0, 30, 15, 15),
                new Rectangle(60, 30, 15, 15)
            };

            _bButton = new Rectangle[]
            {
                new Rectangle(15, 30, 15, 15),
                new Rectangle(75, 30, 15, 15)
            };

            _rightArrow = new Rectangle[]
            {
                new Rectangle(45, 45, 15, 15),
                new Rectangle(60, 45, 15, 15)
            };
        }

        public void DrawAButton(SpriteBatch spriteBatch, Vector2 position, bool pressed, string label, float alpha, bool labelWithShadow = false)
        {
            if (InputManager.Instace.IsPadConnected())
                DrawButton(spriteBatch, position, pressed ? _aButton[1] : _aButton[0], label, alpha, labelWithShadow);
            else
                DrawButton(spriteBatch, position, pressed ? _zButton[1] : _zButton[0], label, alpha, labelWithShadow);
        }

        public void DrawBButton(SpriteBatch spriteBatch, Vector2 position, bool pressed, string label, float alpha, bool labelWithShadow = false)
        {
            if (InputManager.Instace.IsPadConnected())
                DrawButton(spriteBatch, position, pressed ? _bButton[1] : _bButton[0], label, alpha, labelWithShadow);
            else
                DrawButton(spriteBatch, position, pressed ? _xButton[1] : _xButton[0], label, alpha, labelWithShadow);
        }

        public void DrawRightArrow(SpriteBatch spriteBatch, Vector2 position, bool pressed)
        {
            DrawButton(spriteBatch, position, pressed ? _rightArrow[1] : _rightArrow[0], "", 1.0f, false);
        }

        private void DrawButton(SpriteBatch spriteBatch, Vector2 position, Rectangle sprite, string label, float alpha, bool labelWithShadow)
        {
            spriteBatch.Draw(_buttonsTexture, position, sprite, Color.White * alpha);
            if (label != "")
            {
                var h = SceneManager.Instance.GameFontSmall.MeasureString(label).Y;
                var labelPosition = position + new Vector2(18, (15 - h) / 2);
                if (labelWithShadow)
                    spriteBatch.DrawTextWithShadow(SceneManager.Instance.GameFontSmall, label, labelPosition, Color.White * alpha);
                else
                    spriteBatch.DrawString(SceneManager.Instance.GameFontSmall, label, labelPosition, Color.White * alpha);
            }
        }
    }
}
