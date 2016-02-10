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

        //--------------------------------------------------
        // Acceleration

        private Vector2 _acceleration;

        //--------------------------------------------------
        // Subject

        private ProjectileSubject _subject;
        public ProjectileSubject Subject { get { return _subject; } }

        //--------------------------------------------------
        // Damage

        private int _damage;
        public int Damage { get { return _damage; } }

        //--------------------------------------------------
        // Request erase

        public bool RequestErase { get; set; }

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
            _acceleration = new Vector2(dx, dy);
            _damage = damage;
            _subject = subject;
        }

        public void Update(GameTime gameTime)
        {
            _position += _acceleration;
            _sprite.Position = _position;
            if (_position.X >= GameMap.Instance.MapWidth || _position.Y >= GameMap.Instance.MapHeight ||
                Position.X + Sprite.TextureRegion.Width <= 0 || Position.Y + Sprite.TextureRegion.Height <= 0)
                Destroy();
        }

        public void Destroy()
        {
            Sprite.Alpha = 0.0f;
            RequestErase = true;
        }
    }
}
