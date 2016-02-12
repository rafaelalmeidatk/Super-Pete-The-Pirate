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
using Super_Pete_The_Pirate.Objects;

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
        public List<Enemy> Enemies { get { return _enemies; } }

        //--------------------------------------------------
        // Projectiles

        private Dictionary<string, Texture2D> _projectilesTextures;

        private List<GameProjectile> _projectiles;

        //--------------------------------------------------
        // Camera stuff

        private Camera2D _camera;
        private float _cameraSmooth = 0.1f;
        private int _playerCameraOffsetX = 40;

        private string mapInfo = "";

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

            // Player init
            _player = new Player(ImageManager.loadCharacter("Player"));

            // Enemies init
            _enemies = new List<Enemy>();

            // Projectiles init
            _projectilesTextures = new Dictionary<string, Texture2D>()
            {
                {"common", ImageManager.loadProjectile("common")}
            };
            _projectiles = new List<GameProjectile>();

            LoadMap(4);
            mapInfo = GameMap.Instance._tiledMap.Layers.ToString();
        }

        private void LoadMap(int mapId)
        {
            GameMap.Instance.LoadMap(Content, mapId);
            SpawnEnemies();
            SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            var spawnPoint = new Vector2(GameMap.Instance.GetPlayerSpawn().X, GameMap.Instance.GetPlayerSpawn().Y);
            _player.Position = new Vector2(spawnPoint.X, spawnPoint.Y - _player.CharacterSprite.GetColliderHeight());
        }

        private void SpawnEnemies()
        {
            var enemiesGroup = GameMap.Instance.GetObjectGroup("Enemies");
            var tileSize = GameMap.Instance.TileSize;
            foreach (var enemieObj in enemiesGroup.Objects)
            {
                CreateEnemy(enemieObj, enemieObj.X, enemieObj.Y);
            }
        }

        public void CreateEnemy(TiledObject enemyObj, int x, int y)
        {
            var enemyName = enemyObj.Properties.FirstOrDefault(i => i.Key == "type").Value;
            var texture = ImageManager.loadCharacter(enemyName);
            var newEnemy = (Enemy)Activator.CreateInstance(Type.GetType("Super_Pete_The_Pirate.Characters." + enemyName), texture);
            newEnemy.Position = new Vector2(x, y - newEnemy.CharacterSprite.GetColliderHeight());
            newEnemy.CharacterSprite.Effect = enemyObj.Properties["FlipHorizontally"] == "true" ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            _enemies.Add(newEnemy);
        }

        public void CreateProjectile(string name, Vector2 position, int dx, int dy, int damage, ProjectileSubject subject)
        {
            _projectiles.Add(new GameProjectile(_projectilesTextures[name], position, dx, dy, damage, subject));
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);
            
            DebugValues["Player HP"] = String.Format("{0}/10", _player._hp);

            for (var i = 0; i < _projectiles.Count; i++)
            {
                _projectiles[i].Update(gameTime);
                if (_projectiles[i].Subject == ProjectileSubject.FromEnemy && _projectiles[i].BoundingBox.Intersects(_player.BoundingRectangle))
                    _player.ReceiveAttack(_projectiles[i].Damage, _projectiles[i].Position);

                if (_projectiles[i].RequestErase)
                    _projectiles.Remove(_projectiles[i]);
            }

            for (var i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Update(gameTime);

                if (_enemies[i].EnemyType == EnemyType.SniperPig)
                {
                    if (_enemies[i].ViewRangeCooldown <= 0f && _enemies[i].ViewRange.Intersects(_player.BoundingRectangle))
                        _enemies[i].PlayerOnSight(_player.Position);
                }

                if (_enemies[i].BoundingRectangle.Intersects(_player.BoundingRectangle))
                {
                    _player.ReceiveAttack(1, _enemies[i].Position);
                }

                for (var j = 0; j < _projectiles.Count; j++)
                {
                    if (_projectiles[j].Subject == ProjectileSubject.FromPlayer)
                    {
                        if (!_enemies[i].Dying && !_enemies[i].IsImunity && _projectiles[j].BoundingBox.Intersects(_enemies[i].BoundingRectangle))
                        {
                            _enemies[i].ReceiveAttack(_projectiles[j].Damage, _projectiles[j].Position);
                            _projectiles[j].Destroy();
                        }
                    }
                    else if (_projectiles[j].BoundingBox.Intersects(_player.BoundingRectangle))
                    {
                        _player.ReceiveAttack(_projectiles[j].Damage, _projectiles[j].Position);
                        _projectiles[j].Destroy();
                    }

                    if (_projectiles[j].RequestErase)
                        _projectiles.Remove(_projectiles[j]);
                }

                if (_enemies[i].RequestErase)
                    _enemies.Remove(_enemies[i]);
            }

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

            // Draw the projectiles
            foreach (var projectile in _projectiles)
                spriteBatch.Draw(projectile.Sprite);

            spriteBatch.End();
        }
    }
}
