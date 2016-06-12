using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        }

        public void DrawActionButton(SpriteBatch spriteBatch, Vector2 position, bool pressed, string label, float alpha, bool labelWithShadow = false)
        {
            DrawButton(spriteBatch, position, pressed ? _zButton[0] : _zButton[1], label, alpha, labelWithShadow);
        }

        public void DrawCancelButton(SpriteBatch spriteBatch, Vector2 position, bool pressed, string label, float alpha, bool labelWithShadow = false)
        {
            DrawButton(spriteBatch, position, pressed ? _xButton[0] : _xButton[1], label, alpha, labelWithShadow);
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
