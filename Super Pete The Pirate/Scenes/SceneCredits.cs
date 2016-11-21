using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneCredits : SceneBase
    {
        //--------------------------------------------------
        // Background Texture

        private Texture2D _backgroundTexture;

        //--------------------------------------------------
        // Credits

        private Texture2D _creditsTexture;
        private Vector2 _creditsPosition;

        //--------------------------------------------------
        // Fade Out

        private Texture2D _fadeOutTexture;
        private bool _fadeOut;
        private float _fadeOutDelay;
        private float _fadeOutAlpha;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            _backgroundTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _backgroundTexture.SetData(new Color[] { new Color(18, 18, 20) });

            _creditsTexture = ImageManager.loadScene("credits", "Credits");
            _creditsPosition = new Vector2(0, 50);

            _fadeOutTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _fadeOutTexture.SetData(new Color[] { Color.Black });
        }
        public override void UnloadContent()
        {
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (_fadeOut)
            {
                _fadeOutDelay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_fadeOutDelay <= 0.0f)
                {
                    _fadeOutAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds / 1.5f;
                    if (_fadeOutAlpha >= 1.0f)
                    {
                        SceneManager.Instance.TypeOfSceneSaves = SceneManager.SceneSavesType.Save;
                        SceneManager.Instance.ChangeScene("SceneSaves");
                    }
                }
                return;
            }
            var newy = _creditsPosition.Y - (float)gameTime.ElapsedGameTime.TotalSeconds * 80;
            _creditsPosition = new Vector2(0, newy);
            if (newy <= (_creditsTexture.Height - SceneManager.Instance.VirtualSize.Y) * -1)
            {
                _fadeOutDelay = 1000.0f;
                _fadeOut = true;
            }
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            var screenSize = SceneManager.Instance.VirtualSize;
            var screenRect = new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y);
            spriteBatch.Begin(transformMatrix: SceneManager.Instance.ViewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_backgroundTexture, screenRect, Color.White);
            spriteBatch.Draw(_creditsTexture, _creditsPosition, Color.White);
            spriteBatch.Draw(_fadeOutTexture, screenRect, Color.White * _fadeOutAlpha);
            base.Draw(spriteBatch, viewportAdapter);
            spriteBatch.End();
        }
    }
}
