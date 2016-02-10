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
using Microsoft.Xna.Framework.Input;
using Super_Pete_The_Pirate.Characters;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMap : SceneBase
    {
        //--------------------------------------------------
        // Player

        private Player _player;

        //--------------------------------------------------
        // Enemies

        private List<Enemy> _enemies;
        private List<Enemy> _enemiesToErase;
        public List<Enemy> Enemies { get { return _enemies; } }

        //--------------------------------------------------
        // Camera stuff

        private Camera2D _camera;
        private float _cameraSmooth = 0.1f;
        private int _playerCameraOffsetX = 40;

        //----------------------//------------------------//

        public Camera2D GetCamera()
        {
            return _camera;
        }

        public override void LoadContent()
        {
            base.LoadContent();

            var viewportSize = SceneManager.Instance.VirtualSize;
            _camera = new Camera2D(SceneManager.Instance.ViewportAdapter);
            GameMap.Instance.LoadMap(Content, 1);

            // Player init
            _player = new Player(ImageManager.loadCharacter("Player"));
            _player.Position = new Vector2(32, GameMap.Instance.MapHeight - _player.CharacterSprite.GetFrameHeight() * 2);

            // Enemies init
            _enemies = new List<Enemy>();
            _enemiesToErase = new List<Enemy>();

            CreateEnemy(128, 96);
            CreateEnemy(224, 96);

            CreateEnemy(128, 192);
            CreateEnemy(224, 192);
        }

        public void CreateEnemy(int x, int y)
        {
            var newEnemy = new Enemy(ImageManager.loadCharacter("Player"));
            newEnemy.Position = new Vector2(x, y);
            _enemies.Add(newEnemy);
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            foreach (var enemy in _enemies)
            {
                enemy.Update(gameTime);
                if (enemy.RequestErase)
                    _enemiesToErase.Add(enemy);
            }

            foreach (var enemy in _enemiesToErase)
                _enemies.Remove(enemy);

            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            var size = SceneManager.Instance.WindowSize;
            var viewport = SceneManager.Instance.ViewportAdapter;
            var newPosition = _player.Position - new Vector2(viewport.VirtualWidth / 2f, viewport.VirtualHeight / 2f);
            var playerOffsetX = _playerCameraOffsetX + _player.CharacterSprite.GetColliderWidth() / 2;
            var playerOffsetY = _player.CharacterSprite.GetFrameHeight() / 2;
            var x = MathHelper.Lerp(_camera.Position.X, newPosition.X + playerOffsetX, _cameraSmooth);
            x = MathHelper.Clamp(x, 0.0f, GameMap.Instance.MapWidth - viewport.VirtualWidth);
            var y = MathHelper.Lerp(_camera.Position.Y, newPosition.Y + playerOffsetY, _cameraSmooth);
            y = MathHelper.Clamp(y, 0.0f, GameMap.Instance.MapHeight - viewport.VirtualHeight);
            _camera.Position = new Vector2(x, y);
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            // Draw the camera (with the map)
            GameMap.Instance.Draw(_camera, spriteBatch);

            spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);

            // Begin the player draw
            _player.DrawCharacter(spriteBatch);
            if (SceneManager.Instance.DebugMode)
                _player.DrawColliderBox(spriteBatch);

            // Draw the enemies
            foreach (var enemy in _enemies)
            {
                enemy.DrawCharacter(spriteBatch);
                if (SceneManager.Instance.DebugMode)
                    enemy.DrawColliderBox(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
