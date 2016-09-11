using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;

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
        private float _treeBack1Offset;
        private Texture2D _treeBack2;
        private float _treeBack2Offset;

        //--------------------------------------------------
        // Positions

        public float _verticalOffset;

        //----------------------//------------------------//

        public SceneMapBackgroundHelper()
        {
            _static = ImageManager.loadScene(ScenePath, "Background");
            _treeBack1 = ImageManager.loadScene(ScenePath, "TreeBack1");
            _treeBack1Offset = SceneManager.Instance.VirtualSize.Y - _treeBack1.Height;
            _treeBack2 = ImageManager.loadScene(ScenePath, "TreeBack2");
            _treeBack2Offset = SceneManager.Instance.VirtualSize.Y - _treeBack2.Height;
        }

        public void Update(Camera2D camera)
        {
            _verticalOffset = Math.Max(GameMap.Instance.MapHeight - SceneManager.Instance.VirtualSize.Y - camera.Position.Y, 0);
        }

        public void Draw(Camera2D camera, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(transformMatrix: SceneManager.Instance.ViewportAdapter.GetScaleMatrix(), samplerState: SamplerState.PointWrap);

            spriteBatch.Draw(_static, Vector2.Zero, Color.White);
            spriteBatch.Draw(_treeBack2, new Vector2(0, _treeBack2Offset + _verticalOffset * 0.2f), new Rectangle((int)(camera.Position.X * 0.3f), 0, _treeBack1.Width, _treeBack1.Height), Color.White);
            spriteBatch.Draw(_treeBack1, new Vector2(0, _treeBack1Offset + _verticalOffset * 0.4f), new Rectangle((int)(camera.Position.X * 0.5f), 0, _treeBack1.Width, _treeBack1.Height), Color.White);

            spriteBatch.End();
        }
    }
}
