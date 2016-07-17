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
using Super_Pete_The_Pirate.Extensions;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneTitle : SceneBase
    {
        //--------------------------------------------------
        // Scene Name

        private const string ScenePathName = "title";

        //--------------------------------------------------
        // Sprites

        private Sprite _backgroundSprite;
        private Sprite _logoSprite;

        //--------------------------------------------------
        // Press Any Button

        private float _pressAnyButtonInitialY;
        private Vector2 _pressAnyButtonPosition;

        //--------------------------------------------------
        // Menu

        private Color _menuItemColor;
        private Color _menuShadowColor;
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
            var viewportWidth = SceneManager.Instance.ViewportAdapter.VirtualWidth;
            var viewportHeight = SceneManager.Instance.ViewportAdapter.VirtualHeight;

            // Background
            _backgroundSprite = new Sprite(ImageManager.loadScene(ScenePathName, "Background"));
            _backgroundSprite.Origin = Vector2.Zero;

            // Logo
            _logoSprite = new Sprite(ImageManager.loadSystem("Logo"));
            _logoSprite.Position = new Vector2(viewportWidth / 2, 80);

            // Press any button
            var pabX = (viewportWidth - SceneManager.Instance.GameFont.GetSize("Press Any Button").Width) / 2;
            var pabY = viewportHeight - SceneManager.Instance.GameFont.GetSize("Press Any Button").Height - 15;
            _pressAnyButtonPosition = new Vector2(pabX, pabY);
            _pressAnyButtonInitialY = pabY;

            // Menu init
            _menuItemColor = new Color(68, 44, 45);
            _menuShadowColor = new Color(243, 171, 71);
            _menuOptions = new string[]
            {
                "New Game",
                "Load Game",
                "Options",
                "Exit"
            };
            _menuY = viewportHeight - (_menuOptions.Length * SceneManager.Instance.GameFont.LineHeight) - 7;

            // Menu icon
            _menuIconBaseY = _menuY + SceneManager.Instance.GameFont.LineHeight / 2;
            _menuIcon = new Sprite(ImageManager.loadScene(ScenePathName, "indexIcon"));
            _menuIcon.Position = new Vector2(13, _menuIconBaseY);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            HandleInput();

            _menuIcon.Position = new Vector2(_menuIcon.Position.X, _menuIconBaseY + (SceneManager.Instance.GameFont.LineHeight * _index));

            if (_phase == PressAnyButtonPhase)
            {
                if (InputManager.Instace.CurrentKeyState.GetPressedKeys().Length > 0)
                {
                    _phase = MenuPhase;
                    SoundManager.PlayConfirmSe();
                }

                var delta = (float)gameTime.TotalGameTime.TotalMilliseconds / 10;
                _pressAnyButtonPosition.Y = (float)MathUtils.SinInterpolation(_pressAnyButtonInitialY, _pressAnyButtonInitialY + 5, delta);
            }
        }

        private void HandleInput()
        {
            if (InputManager.Instace.KeyPressed(Keys.Up) || InputManager.Instace.KeyPressed(Keys.Left))
            {
                _index = _index - 1 < 0 ? _menuOptions.Length - 1 : _index - 1;
                SoundManager.PlaySelectSe();
            }

            if (InputManager.Instace.KeyPressed(Keys.Down) || InputManager.Instace.KeyPressed(Keys.Right))
            {
                _index = _index + 1 > _menuOptions.Length - 1 ? 0 : _index + 1;
                SoundManager.PlaySelectSe();
            }

            if (_phase == MenuPhase && InputManager.Instace.KeyPressed(Keys.Z, Keys.Enter))
            {
                switch (_index)
                {
                    case NewGame:
                        CommandNewGame();
                        SoundManager.PlayConfirmSe();
                        break;

                    case LoadGame:
                        CommandLoadGame();
                        SoundManager.PlayConfirmSe();
                        break;

                    case Exit:
                        SceneManager.Instance.RequestExit();
                        SoundManager.PlayConfirmSe();
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

            // Background and Logo
            spriteBatch.Draw(_backgroundSprite);
            spriteBatch.Draw(_logoSprite);

            if (_phase == PressAnyButtonPhase)
            {
                spriteBatch.DrawString(SceneManager.Instance.GameFont, "Press Any Button", _pressAnyButtonPosition, _menuItemColor);
            }
            else if (_phase == MenuPhase)
            {
                // Menu
                for (var i = 0; i < _menuOptions.Length; i++)
                    spriteBatch.DrawString(SceneManager.Instance.GameFont, _menuOptions[i],
                        new Vector2(25, _menuY + (i * SceneManager.Instance.GameFont.LineHeight)), _menuItemColor);
                spriteBatch.Draw(_menuIcon);
            }

            spriteBatch.End();
        }
    }
}
