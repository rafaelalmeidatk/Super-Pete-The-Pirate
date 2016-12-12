using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneGameover : SceneBase
    {
        //--------------------------------------------------
        // Background

        private Texture2D _backgroundTexture;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _backgroundTexture = ImageManager.loadScene("gameOver", "GameOver");

            // Start BGM
            SoundManager.StartBgm("Ossuary6Air");
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!SceneManager.Instance.IsTransitioning && InputManager.Instace.AnyKeyPressed())
            {
                SoundManager.PlayConfirmSe();
                SceneManager.Instance.ChangeScene("SceneTitle");
            }
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            spriteBatch.Begin(transformMatrix: viewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_backgroundTexture, Vector2.Zero, Color.White);
            spriteBatch.End();
        }
    }
}
