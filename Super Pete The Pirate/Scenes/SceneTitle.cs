using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Extensions;
using Super_Pete_The_Pirate.Managers;

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
        // Press Start!

        private float _pressStartInitialY;
        private Vector2 _pressStartPosition;

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

        private const int PressStartPhase = 0;
        private const int MenuPhase = 1;
        private const int OptionsPhase = 2;

        //--------------------------------------------------
        // Option Helper

        private SceneTitleOptionsHelper _optionsHelper;

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

            // Press Start!
            var pabX = (viewportWidth - SceneManager.Instance.GameFont.GetSize("Press Start!").Width) / 2;
            var pabY = viewportHeight - SceneManager.Instance.GameFont.GetSize("Press Start!").Height - 15;
            _pressStartPosition = new Vector2(pabX, pabY);
            _pressStartInitialY = pabY;

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
            _menuIcon = new Sprite(ImageManager.loadScene(ScenePathName, "IndexIcon"));
            _menuIcon.Position = new Vector2(13, _menuIconBaseY);

            // Helper init
            _optionsHelper = new SceneTitleOptionsHelper();

            // Start BGM
            SoundManager.StartBgm("AchaidhCheide");

            if (SceneManager.Instance.TitleStarted)
                _phase = 1;
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_phase == OptionsPhase)
            {
                _optionsHelper.Update(gameTime);
                _index = _optionsHelper.Index;
                if (_optionsHelper.RequestingExit)
                {
                    _phase = MenuPhase;
                    _index = Options;
                    SettingsManager.Instance.SaveSettings();
                }
            }
            else
            {
                HandleInput(gameTime);
            }

            _menuIcon.Position = new Vector2(_menuIcon.Position.X, _menuIconBaseY + (SceneManager.Instance.GameFont.LineHeight * _index));
        }

        private void HandleInput(GameTime gameTime)
        {
            if (_phase == PressStartPhase)
            {
                if (InputManager.Instace.Pressed(InputCommand.Confirm))
                {
                    SceneManager.Instance.TitleStarted = true;
                    _phase = MenuPhase;
                    SoundManager.PlayConfirmSe();
                    return;
                }

                var delta = (float)gameTime.TotalGameTime.TotalMilliseconds / 10;
                _pressStartPosition.Y = (float)MathUtils.SinInterpolation(_pressStartInitialY, _pressStartInitialY + 5, delta);
            }

            if (_phase == MenuPhase)
            {
                if (InputManager.Instace.Pressed(InputCommand.Up) || InputManager.Instace.Pressed(InputCommand.Left))
                {
                    _index = _index - 1 < 0 ? _menuOptions.Length - 1 : _index - 1;
                    SoundManager.PlaySelectSe();
                }

                if (InputManager.Instace.Pressed(InputCommand.Down) || InputManager.Instace.Pressed(InputCommand.Right))
                {
                    _index = _index + 1 > _menuOptions.Length - 1 ? 0 : _index + 1;
                    SoundManager.PlaySelectSe();
                }

                if (InputManager.Instace.Pressed(InputCommand.Confirm))
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

                        case Options:
                            _optionsHelper.Activate();
                            _phase = OptionsPhase;
                            break;

                        case Exit:
                            SceneManager.Instance.RequestExit();
                            SoundManager.PlayConfirmSe();
                            break;
                    }
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

            if (_phase == PressStartPhase)
            {
                spriteBatch.DrawString(SceneManager.Instance.GameFont, "Press Start!", _pressStartPosition, _menuItemColor);
            }
            else if (_phase == MenuPhase)
            {
                // Menu
                for (var i = 0; i < _menuOptions.Length; i++)
                    spriteBatch.DrawString(SceneManager.Instance.GameFont, _menuOptions[i],
                        new Vector2(25, _menuY + (i * SceneManager.Instance.GameFont.LineHeight)), _menuItemColor);
                spriteBatch.Draw(_menuIcon);
            }
            else if (_phase == OptionsPhase)
            {
                _optionsHelper.Draw(spriteBatch);
                spriteBatch.Draw(_menuIcon);
            }

            spriteBatch.End();
        }
    }
}
