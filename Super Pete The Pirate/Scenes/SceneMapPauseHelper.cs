using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMapPauseHelper
    {
        //--------------------------------------------------
        // Paused

        public bool Paused => _paused;
        private bool _paused;

        //--------------------------------------------------
        // Title

        private const string Title = "Paused";
        private Vector2 _pausePosition;

        //--------------------------------------------------
        // Background

        private Texture2D _backgroundTexture;
        private Rectangle _backgroundDrawArea;

        //--------------------------------------------------
        // Menu

        private string[] _menu;
        private float[] _menuItemsX;
        private float _menuY;
        private int _index;
        private Texture2D _cursor;

        //----------------------//------------------------//

        public SceneMapPauseHelper()
        {
            _backgroundTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _backgroundTexture.SetData<Color>(new Color[] { Color.Black });
            _backgroundDrawArea = new Rectangle(0, 0, (int)SceneManager.Instance.VirtualSize.X, (int)SceneManager.Instance.VirtualSize.Y);

            var virtualSize = SceneManager.Instance.VirtualSize;

            _menu = new string[]
            {
                "Continue",
                "Exit to Menu"
            };

            _menuItemsX = new float[_menu.Length];
            for (var i = 0; i < _menu.Length; i++)
                _menuItemsX[i] = (virtualSize.X - SceneManager.Instance.GameFont.MeasureString(_menu[i]).X) / 2;
            _menuY = virtualSize.Y - (_menu.Length * SceneManager.Instance.GameFont.LineHeight) - 20;

            _cursor = ImageManager.loadSystem("CursorIcon");

            _pausePosition = new Vector2((virtualSize.X - SceneManager.Instance.GameFontBig.MeasureString(Title).X) / 2, 30);
        }

        public void SetPaused(bool paused)
        {
            _paused = paused;
        }

        public void Update(GameTime gameTime)
        {
            if (!_paused) return;

            if (InputManager.Instace.KeyPressed(Keys.Up, Keys.W))
                _index = _index - 1 < 0 ? _menu.Length - 1 : _index - 1;

            if (InputManager.Instace.KeyPressed(Keys.Down, Keys.S))
                _index = (_index + 1) % _menu.Length;

            if (InputManager.Instace.KeyPressed(Keys.Z, Keys.Enter))
            {
                switch (_index)
                {
                    case 0:
                        _paused = false;
                        break;

                    case 1:
                        returnToMenu();
                        break;
                }
            }
        }

        private void returnToMenu()
        {
            SceneManager.Instance.ChangeScene("SceneTitle");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!_paused) return;

            spriteBatch.Draw(_backgroundTexture, _backgroundDrawArea, Color.White * 0.5f);

            spriteBatch.DrawString(SceneManager.Instance.GameFontBig, Title, _pausePosition, Color.White);

            for (var i = 0; i < _menu.Length; i++)
            {
                var position = new Vector2(_menuItemsX[i], _menuY + (i * SceneManager.Instance.GameFont.LineHeight));
                spriteBatch.DrawString(SceneManager.Instance.GameFont, _menu[i], position, Color.White);
            }

            spriteBatch.Draw(_cursor, new Vector2(_menuItemsX[_index] - 16, _menuY + (_index * SceneManager.Instance.GameFont.LineHeight) - 1), Color.White);
        }
    }
}
