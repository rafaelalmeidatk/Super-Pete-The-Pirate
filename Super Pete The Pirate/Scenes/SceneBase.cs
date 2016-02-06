using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.ViewportAdapters;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneBase
    {
        //--------------------------------------------------
        // Some stuff

        private SpriteFont _debugFont;
        public ContentManager Content;
        public Dictionary<string, string> DebugValues;

        //----------------------//------------------------//

        public virtual void LoadContent()
        {
            Content = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");
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

        public void DrawDebugValue(SpriteBatch spriteBatch)
        {
            if (!SceneManager.Instance.DebugMode) return;
            spriteBatch.Begin();
            var i = 0;
            foreach (KeyValuePair<string, string> value in DebugValues)
                spriteBatch.DrawString(_debugFont, value.Value, new Vector2(0, 20 * i++), Color.White);
            spriteBatch.End();
        }

        public virtual void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter) { }
    }
}
