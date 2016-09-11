using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneBase
    {
        //--------------------------------------------------
        // Some stuff

        private SpriteFont _debugFont;
        public ContentManager Content;
        public Dictionary<string, string> DebugValues;

        //--------------------------------------------------
        // FPS counter

        private FramesPerSecondCounter _fpsCounter;

        //----------------------//------------------------//

        public virtual void LoadContent()
        {
            Content = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");
            _fpsCounter = new FramesPerSecondCounter();
            _debugFont = Content.Load<SpriteFont>("fonts/DebugFont");
            DebugValues = new Dictionary<string, string>();
        }

        public virtual void UnloadContent()
        {
            Content.Unload();
        }

        public virtual void Update(GameTime gameTime)
        {
            InputManager.Instace.Update();
        }

        public void UpdateFpsCounter(GameTime gameTime)
        {
            _fpsCounter.Update(gameTime);
        }

        public void DrawDebugValue(SpriteBatch spriteBatch)
        {
            if (!SceneManager.Instance.DebugMode) return;
            spriteBatch.Begin();
            spriteBatch.DrawString(_debugFont, string.Format("FPS: {0}", _fpsCounter.AverageFramesPerSecond), new Vector2(5, 5), Color.Gray);
            var i = 0;
            foreach (KeyValuePair<string, string> value in DebugValues)
                spriteBatch.DrawString(_debugFont, value.Key + ": " + value.Value, new Vector2(5, 25 + 20 * i++), Color.White);
            spriteBatch.End();
        }

        public virtual void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter) { }
    }
}
