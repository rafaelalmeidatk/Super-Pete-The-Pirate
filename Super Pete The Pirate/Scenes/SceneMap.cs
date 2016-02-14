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

        //--------------------------------------------------
        // Random stuff

        private Random _rand;

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

            _rand = new Random();
            LoadMap(2);
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
            if (enemiesGroup == null) return;
            var tileSize = GameMap.Instance.TileSize;
            foreach (var enemieObj in enemiesGroup.Objects)
            {
                CreateEnemy(enemieObj, enemieObj.X, enemieObj.Y);
            }
        }

        public void CreateEnemy(TiledObject enemyObj, int x, int y)
        {
            var enemyName = enemyObj.Properties.FirstOrDefault(i => i.Key == "type").Value;
            if (enemyName == null) return;
            var texture = ImageManager.loadCharacter(enemyName);
            var newEnemy = (Enemy)Activator.CreateInstance(Type.GetType("Super_Pete_The_Pirate.Characters." + enemyName), texture);
            newEnemy.Position = new Vector2(x, y - newEnemy.CharacterSprite.GetColliderHeight());
            if (enemyObj.Properties.ContainsKey("FlipHorizontally"))
                newEnemy.CharacterSprite.Effect = enemyObj.Properties["FlipHorizontally"] == "true" ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            if (enemyName == "TurtleWheel")
                ((TurtleWheel)newEnemy).SetTarget(_player);
            _enemies.Add(newEnemy);
        }

        public void CreateProjectile(string name, Vector2 position, int dx, int dy, int damage, ProjectileSubject subject)
        {
            _projectiles.Add(new GameProjectile(_projectilesTextures[name], position, dx, dy, damage, subject));
        }

        public override void Update(GameTime gameTime)
        {
            _player.Update(gameTime);

            if (InputManager.Instace.KeyPressed(Keys.F)) _projectiles[0].Acceleration = new Vector2(_projectiles[0].Acceleration.X * -1, 0);

            for (var i = 0; i < _projectiles.Count; i++)
            {
                _projectiles[i].Update(gameTime);
                if (_projectiles[i].Subject == ProjectileSubject.FromEnemy && _projectiles[i].BoundingBox.Intersects(_player.BoundingRectangle))
                    _player.ReceiveAttack(_projectiles[i].Damage, _projectiles[i].LastPosition);

                if (_projectiles[i].RequestErase)
                    _projectiles.Remove(_projectiles[i]);
            }

            for (var i = 0; i < _enemies.Count; i++)
            {
                _enemies[i].Update(gameTime);

                if (_enemies[i].HasViewRange && _enemies[i].ViewRangeCooldown <= 0f && _enemies[i].ViewRange.Intersects(_player.BoundingRectangle))
                {
                        _enemies[i].PlayerOnSight(_player.Position);
                }

                if (!_enemies[i].Dying && _enemies[i].BoundingRectangle.Intersects(_player.BoundingRectangle))
                {
                    _player.ReceiveAttackWithPoint(1, _enemies[i].BoundingRectangle);
                }

                for (var j = 0; j < _projectiles.Count; j++)
                {
                    if (_projectiles[j].Subject == ProjectileSubject.FromPlayer)
                    {
                        if (!_enemies[i].Dying && !_enemies[i].IsImunity && _projectiles[j].BoundingBox.Intersects(_enemies[i].BoundingRectangle))
                        {
                            if (_enemies[i].EnemyType == EnemyType.TurtleWheel && _enemies[i].InWheelMode)
                            {
                                if (!_projectiles[j].IsTimerRunning())
                                {
                                    _projectiles[j].Acceleration = new Vector2(-Math.Abs(_projectiles[j].Acceleration.X) * 1.7f, _rand.Next(-4, 5));
                                    _projectiles[j].Subject = ProjectileSubject.FromEnemy;
                                    _projectiles[j].SetTimer(5000f);
                                }
                            }
                            else
                            {
                                _enemies[i].ReceiveAttack(_projectiles[j].Damage, _projectiles[j].LastPosition);
                                _projectiles[j].Destroy();
                            }
                        }
                    }
                    else if (_projectiles[j].BoundingBox.Intersects(_player.BoundingRectangle))
                    {
                        _player.ReceiveAttack(_projectiles[j].Damage, _projectiles[j].LastPosition);
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

            if (InputManager.Instace.KeyPressed(Keys.P))
            {
                CreateParticle();
            }
        }

        private void CreateParticle()
        {
            var texture = ImageManager.loadParticle("BulletPiece");
            for (var i = 0; i < 4; i++)
            {
                var position = new Vector2(100, 100);
                position.Y += _rand.NextFloat(-5f, 5f);
                var velocity = new Vector2(_rand.NextFloat(50f, 100f), _rand.NextFloat(-300f, -50f));
                var scale = Vector2.One;
                var hor = _rand.Next(0, 2) == 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                var ver = _rand.Next(0, 2) == 0 ? SpriteEffects.FlipVertically : SpriteEffects.None;

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    Type = ParticleType.GroundPieces,
                    Gravity = 3f
                };

                SceneManager.Instance.ParticleManager.CreateParticle(texture, position, Color.White, 200f, scale, state, hor|ver);
            }
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

            // Draw the particles

            SceneManager.Instance.ParticleManager.Draw(spriteBatch);

            spriteBatch.End();
        }
    }
}
