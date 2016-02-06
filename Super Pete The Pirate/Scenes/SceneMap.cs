using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.Maps.Tiled;
using MonoGame.Extended.ViewportAdapters;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMap : SceneBase
    {
        //--------------------------------------------------
        // Player

        private Player _player;

        //--------------------------------------------------
        // Camera stuff

        private Camera2D _camera;
        private float _cameraSmooth = 0.1f;
        private int _playerCameraOffsetX = 40;

        //----------------------//------------------------//

        public Camera2D GetCamera()
        {
            return _camera; // aaa
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var viewportSize = SceneManager.Instance.VirtualSize;
            _camera = new Camera2D(SceneManager.Instance.ViewportAdapter);
            GameMap.Instance.LoadMap(Content, 1);
            _player = new Player(ImageManager.loadCharacter("Player"));
            _player.Position = new Vector2(35, 36);
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            DebugValues["playerPos"] = _player.Position.ToString();
            DebugValues["playerSpritePos"] = _player.CharacterSprite.Position.ToString();
            DebugValues["bouncing"] = _player.BoundingRectangle.ToString();
            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            var size = SceneManager.Instance.WindowSize;
            var viewport = SceneManager.Instance.ViewportAdapter;
            var newPosition = _player.Position - new Vector2(viewport.VirtualWidth / 2f, viewport.VirtualHeight / 2f);
            var playerOffsetX = _playerCameraOffsetX + _player.CharacterSprite.GetFrameWidth() / 2;
            var playerOffsetY = _player.CharacterSprite.GetFrameHeight() / 2;
            var x = MathHelper.Lerp(_camera.Position.X, newPosition.X + playerOffsetX, _cameraSmooth);
            var xOffset = (playerOffsetX - viewport.ViewportWidth * _cameraSmooth / 2);
            x = MathHelper.Clamp(x, 0.0f, GameMap.Instance.MapWidth - viewport.VirtualWidth + xOffset);
            var y = MathHelper.Lerp(_camera.Position.Y, newPosition.Y + playerOffsetY, _cameraSmooth);
            var yOffset = (viewport.VirtualHeight * _cameraSmooth / 2);
            y = MathHelper.Clamp(y, 0.0f, GameMap.Instance.MapHeight - viewport.VirtualHeight + yOffset);
            _camera.Position = new Vector2(x, y);
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            // Draw the camera (with the map)
            GameMap.Instance.Draw(_camera, spriteBatch);

            // Begin the player draw
            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            _player.DrawCharacter(spriteBatch);

            // Draw colliders
            if (SceneManager.Instance.DebugMode)
                _player.DrawColliderBox(spriteBatch);

            spriteBatch.End();
        }
    }
}
