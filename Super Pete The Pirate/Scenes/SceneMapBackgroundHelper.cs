using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMapBackgroundHelper
    {
        //--------------------------------------------------
        // Scene Path

        private const string ScenePath = "map";

        //--------------------------------------------------
        // Images

        private Texture2D _static;
        private Texture2D _treeBack1;
        private Vector2 _treeBack1Position;

        //----------------------//------------------------//

        public SceneMapBackgroundHelper()
        {
            _static = ImageManager.loadScene(ScenePath, "Background");
            _treeBack1 = ImageManager.loadScene(ScenePath, "TreeBack1");

            _treeBack1Position = new Vector2(0, (int)SceneManager.Instance.VirtualSize.Y - _treeBack1.Height);
        }

        public void Draw(Camera2D camera, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: SceneManager.Instance.ViewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointClamp);
            spriteBatch.Draw(_static, _static.Bounds, Color.White);
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: SceneManager.Instance.ViewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointWrap);

            // TreeBack1
            spriteBatch.Draw(_treeBack1, _treeBack1Position, new Rectangle((int)(camera.Position.X * 0.5f), 0, _treeBack1.Width, _treeBack1.Height), Color.White);

            spriteBatch.End();
        }
    }
}
