using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;

namespace Super_Pete_The_Pirate
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class GameMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //--------------------------------------------------
        // Viewport adapter

        public static BoxingViewportAdapter ViewportAdapter;
        public static GameWindow GameWindow;

        public GameMain()
        {
            var windowSize = SceneManager.Instance.WindowSize;
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = (int)windowSize.X;
            graphics.PreferredBackBufferHeight = (int)windowSize.Y;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";

            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            this.Window.Position = new Point((displayMode.Width - (int)windowSize.X) / 2, (displayMode.Height - (int)windowSize.Y) / 2);
            this.Window.AllowUserResizing = true;

            GameWindow = this.Window;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            var virtualSize = SceneManager.Instance.VirtualSize;
            ViewportAdapter = new BoxingViewportAdapter(this.Window, GraphicsDevice, (int)virtualSize.X, (int)virtualSize.Y);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SceneManager.Instance.GraphicsDevice = GraphicsDevice;
            SceneManager.Instance.SpriteBatch = spriteBatch;
            SceneManager.Instance.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            SceneManager.Instance.UnloadContent();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)
                || SceneManager.Instance.RequestingExit)
                Exit();

            SceneManager.Instance.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            SceneManager.Instance.Draw(spriteBatch);
            base.Draw(gameTime);
        }
    }
}
