using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Objects
{
    //--------------------------------------------------
    // Projectile Subject

    public enum ProjectileSubject
    {
        FromPlayer,
        FromEnemy
    }

    public class GameProjectile
    {
        //--------------------------------------------------
        // Sprite

        private Sprite _sprite;
        public Sprite Sprite { get { return _sprite; } }

        //--------------------------------------------------
        // Position

        private Vector2 _position;
        public Vector2 Position { get { return _position; } }
        public Vector2 LastPosition { get; private set; }

        //--------------------------------------------------
        // Acceleration

        private Vector2 _acceleration;
        public Vector2 Acceleration
        {
            get { return _acceleration; }
            set
            {
                _acceleration = value;
            }
        }

        //--------------------------------------------------
        // Subject

        private ProjectileSubject _subject;
        public ProjectileSubject Subject
        {
            get { return _subject; }
            set
            {
                _subject = value;
            }
        }

        //--------------------------------------------------
        // Damage

        private int _damage;
        public int Damage { get { return _damage; } }

        //--------------------------------------------------
        // Request erase

        public bool RequestErase { get; set; }

        //--------------------------------------------------
        // Timer

        private float _timer;

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //--------------------------------------------------
        // Bouding box

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)_position.X, (int)_position.Y, _sprite.TextureRegion.Width, _sprite.TextureRegion.Height);
            }
        }

        //----------------------//------------------------//

        public GameProjectile(Texture2D texture, Vector2 initialPosition, float dx, float dy, int damage, ProjectileSubject subject)
        {
            _sprite = new Sprite(texture);
            _sprite.Position = initialPosition;
            _position = initialPosition;
            LastPosition = _position;
            _acceleration = new Vector2(dx, dy);
            _damage = damage;
            _subject = subject;
            _rand = new Random();
            _timer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            if (_timer > 0f) _timer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            LastPosition = _position;
            _position += _acceleration;
            _sprite.Position = _position;
            var tileX = (int)(_position.X / GameMap.Instance.TileSize.X);
            var tileY = (int)(_position.Y / GameMap.Instance.TileSize.Y);
            if (_position.X >= GameMap.Instance.MapWidth || _position.Y >= GameMap.Instance.MapHeight ||
                Position.X + Sprite.TextureRegion.Width <= 0 || Position.Y + Sprite.TextureRegion.Height <= 0)
            {
                Destroy();
                CreateProjectileCrashparticles();
            }

            if (GameMap.Instance.IsTileBlocked(tileX, tileY))
            {
                Destroy();
                CreateTileCrashParticles();
            }
        }

        private void CreateProjectileCrashparticles()
        {
            var texture = ImageManager.loadParticle("BulletPiece");
            for (var i = 0; i < 2; i++)
            {
                var position = new Vector2(_position.X, _position.Y);
                position.Y += _rand.NextFloat(-5f, 5f);
                var velocity = new Vector2(_rand.NextFloat(50f, 100f) * -Math.Sign(_acceleration.X), _rand.NextFloat(-300f, -50f));
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

        private void CreateTileCrashParticles()
        {
            var texture = ImageManager.loadParticle("GroundPiece");
            for (var i = 0; i < 10; i++)
            {
                var position = new Vector2(_position.X, _position.Y);
                position.Y += _rand.NextFloat(0f, 3f);
                var velocity = new Vector2(_rand.NextFloat(130f, 150f) * -Math.Sign(_acceleration.X), _rand.NextFloat(-300f, 50f));
                var scale = new Vector2(_rand.NextFloat(1f, 3f), _rand.NextFloat(1f, 3f));

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    Type = ParticleType.GroundPieces,
                    Gravity = 3f
                };

                SceneManager.Instance.ParticleManager.CreateParticle(texture, position, Color.White, 500f, scale, state);
            }
        }

        public void SetTimer(float time)
        {
            _timer = time;
        }

        public bool IsTimerRunning()
        {
            return _timer > 0f;
        }

        public void Destroy()
        {
            CreateProjectileCrashparticles();
            Sprite.Alpha = 0.0f;
            RequestErase = true;
        }
    }
}
