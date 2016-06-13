using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Managers;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneTitle : SceneBase
    {
        //--------------------------------------------------
        // Font

        private BitmapFont _bitmapFont;

        //--------------------------------------------------
        // Images

        private Sprite _backgroundImage;

        //--------------------------------------------------
        // Press Any Button

        private Vector2 _pressAnyButtonPosition;

        //--------------------------------------------------
        // Menu

        private Color _menuDefaultColor;
        private string[] _menuOptions;
        private int _menuY;

        private const int NewGame = 0;
        private const int LoadGame = 1;
        private const int Options = 2;
        private const int Exit = 3;

        //--------------------------------------------------
        // Menu icon

        private Sprite _menuIcon;
        private int _menuIconBaseY;

        //--------------------------------------------------
        // Scene mechanic

        private int _index;
        private int _phase;

        //--------------------------------------------------
        // Scene phases

        private const int PressAnyButtonPhase = 0;
        private const int MenuPhase = 1;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();
            _bitmapFont = Content.Load<BitmapFont>("fonts/Alagard");

            // Background
            var texture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            texture.SetData<Color>(new Color[] { Color.LightGreen });
            _backgroundImage = new Sprite(texture);
            _backgroundImage.Origin = Vector2.Zero;
            _backgroundImage.Scale = new Vector2(SceneManager.Instance.VirtualSize.X, SceneManager.Instance.VirtualSize.Y);

            // Press any button
            var pabX = (SceneManager.Instance.ViewportAdapter.VirtualWidth - _bitmapFont.GetSize("Press Any Button").Width) / 2;
            var pabY = SceneManager.Instance.ViewportAdapter.VirtualHeight - _bitmapFont.GetSize("Press Any Button").Height - 20;
            _pressAnyButtonPosition = new Vector2(pabX, pabY);

            // Menu init
            _menuDefaultColor = new Color(23, 34, 68);
            _menuOptions = new string[]
            {
                "New Game",
                "Load Game",
                "Options",
                "Exit"
            };
            _menuY = SceneManager.Instance.ViewportAdapter.VirtualHeight - (_menuOptions.Length * _bitmapFont.LineHeight) - 7;

            // Menu icon
            _menuIconBaseY = _menuY + _bitmapFont.LineHeight / 2;
            _menuIcon = new Sprite(ImageManager.loadScene("title", "indexIcon"));
            _menuIcon.Position = new Vector2(13, _menuIconBaseY);

            _index = 0;
            _phase = 0;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            HandleInput();

            _menuIcon.Position = new Vector2(_menuIcon.Position.X, _menuIconBaseY + (_bitmapFont.LineHeight * _index));

            if (_phase == PressAnyButtonPhase && InputManager.Instace.CurrentKeyState.GetPressedKeys().Length > 0)
                _phase = MenuPhase;
        }

        private void HandleInput()
        {
            if (InputManager.Instace.KeyPressed(Keys.Up) || InputManager.Instace.KeyPressed(Keys.Left))
                _index = _index - 1 < 0 ? _menuOptions.Length - 1 : _index - 1;

            if (InputManager.Instace.KeyPressed(Keys.Down) || InputManager.Instace.KeyPressed(Keys.Right))
                _index = _index + 1 > _menuOptions.Length - 1 ? 0 : _index + 1;

            if (_phase == MenuPhase && InputManager.Instace.KeyPressed(Keys.Z, Keys.Enter))
            {
                switch (_index)
                {
                    case NewGame:
                        CommandNewGame();
                        break;

                    case LoadGame:
                        CommandLoadGame();
                        break;

                    case Exit:
                        SceneManager.Instance.RequestExit();
                        break;
                }
            }
        }

        private void CommandNewGame()
        {
            PlayerManager.Instance.CreateNewGame();
            SceneManager.Instance.ChangeScene("SceneStageSelect");
        }

        private void CommandLoadGame()
        {
            SceneManager.Instance.TypeOfSceneSaves = SceneManager.SceneSavesType.Load;
            SceneManager.Instance.ChangeScene("SceneSaves");
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);
            spriteBatch.Begin(transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);

            // Background
            spriteBatch.Draw(_backgroundImage);

            if (_phase == PressAnyButtonPhase)
            {
                spriteBatch.DrawString(_bitmapFont, "Press Any Button", _pressAnyButtonPosition, Color.White);
            }
            else if (_phase == MenuPhase)
            {
                // Menu
                for (var i = 0; i < _menuOptions.Length; i++)
                    spriteBatch.DrawString(_bitmapFont, _menuOptions[i], new Vector2(25, _menuY + (i * _bitmapFont.LineHeight)), _menuDefaultColor);
                spriteBatch.Draw(this._menuIcon);
            }

            spriteBatch.End();
        }
    }
}
