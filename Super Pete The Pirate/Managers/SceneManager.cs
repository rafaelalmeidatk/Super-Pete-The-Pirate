using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Scenes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate
{
    class SceneManager
    {
        //--------------------------------------------------
        // Public variables

        public Vector2 WindowSize = new Vector2(720, 480);
        public Vector2 VirtualSize = new Vector2(360, 240);
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;
        public ViewportAdapter ViewportAdapter { get { return GameMain.ViewportAdapter; } }
        public GameWindow GameMap { get { return GameMain.GameWindow; } }
        public ContentManager Content { private set; get; }

        public bool RequestingExit = false;

        //--------------------------------------------------
        // SceneManager Singleton variables

        private static SceneManager _instance = null;
        private static readonly object _padlock = new object();

        //--------------------------------------------------
        // Transition

        private SceneBase _currentScene, _newScene;
        private Sprite _transitionImage;
        private bool _isTransitioning = false;
        public bool IsTransitioning { get { return _isTransitioning; } }
        private bool _beginTransitionFade = false;

        //--------------------------------------------------
        // Particle Manager

        public ParticleManager<ParticleState> ParticleManager { get; private set; }

        //--------------------------------------------------
        // Debug mode

        public bool DebugMode = true;

        //--------------------------------------------------
        // Game fonts

        private BitmapFont _gameFont;
        public BitmapFont GameFont { get { return _gameFont; } }

        private BitmapFont _gameFontSmall;
        public BitmapFont GameFontSmall { get { return _gameFontSmall; } }

        private BitmapFont _gameFontBig;
        public BitmapFont GameFontBig { get { return _gameFontBig; } }

        //--------------------------------------------------
        // Saves scene type

        public enum SceneSavesType
        {
            Save,
            Load,
            NewGame
        }

        public SceneSavesType TypeOfSceneSaves = SceneSavesType.NewGame;

        //--------------------------------------------------
        // Map To Load

        public int MapToLoad = 0;

        //----------------------//------------------------//

        public static SceneManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new SceneManager();
                    return _instance;
                }
            }
        }

        private SceneManager()
        {
            TypeOfSceneSaves = SceneSavesType.Load;
            _currentScene = new SceneStageSelect();
        }

        public void RequestExit()
        {
            RequestingExit = true;
        }

        public SceneBase GetCurrentScene()
        {
            return _currentScene;
        }

        public void LoadContent(ContentManager Content)
        {
            this.Content = new ContentManager(Content.ServiceProvider, "Content");
            var transitionTexture = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            transitionTexture.SetData<Color>(new Color[] { Color.Black });
            _transitionImage = new Sprite(transitionTexture);
            _transitionImage.Scale = new Vector2(VirtualSize.X, VirtualSize.Y);
            _transitionImage.Alpha = 0.0f;
            _transitionImage.IsVisible = false;
            _gameFont = Content.Load<BitmapFont>("fonts/Alagard");
            _gameFontSmall = Content.Load<BitmapFont>("fonts/AlagardSmall");
            _gameFontBig = Content.Load<BitmapFont>("fonts/AlagardBig");
            ParticleManager = new ParticleManager<ParticleState>(1024 * 20, ParticleState.UpdateParticle);
            _currentScene.LoadContent();
        }

        public void UnloadContent()
        {
            _currentScene.UnloadContent();
        }

        public void Update(GameTime gameTime)
        {
            if (_isTransitioning)
                UpdateTransition(gameTime);
            else if (InputManager.Instace.KeyPressed(Keys.F5))
                    DebugMode = !DebugMode;

            ParticleManager.Update(gameTime);

            _currentScene.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _currentScene.Draw(spriteBatch, ViewportAdapter);
            spriteBatch.Begin();
            spriteBatch.Draw(_transitionImage.TextureRegion.Texture, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White * _transitionImage.Alpha);
            spriteBatch.End();
            _currentScene.DrawDebugValue(spriteBatch);
        }

        public void ChangeScene(string newScene)
        {
            if (_isTransitioning) return;
            _newScene = (SceneBase)Activator.CreateInstance(Type.GetType("Super_Pete_The_Pirate.Scenes." + newScene));
            _transitionImage.Alpha = 0;
            _transitionImage.IsVisible = true;
            _isTransitioning = true;
            _beginTransitionFade = true;
        }

        private void UpdateTransition(GameTime gameTime)
        {
            var dt = gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_beginTransitionFade)
            {
                if (_transitionImage.Alpha < 1.0f)
                    _transitionImage.Alpha += 0.1f;
                else
                    _beginTransitionFade = false;
            }
            else
            {
                if (_newScene != null)
                {
                    _currentScene.UnloadContent();
                    _currentScene = _newScene;
                    _currentScene.LoadContent();
                    _newScene = null;
                }

                if (_transitionImage.Alpha > 0.0f)
                    _transitionImage.Alpha -= 0.1f;
                else
                {
                    _transitionImage.IsVisible = false;
                    _isTransitioning = false;
                }
            }
        }
    }
}
