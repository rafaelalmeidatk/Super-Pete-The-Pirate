using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Characters
{
    class Parrot : Enemy
    {
        //--------------------------------------------------
        // Fly width

        private int _flyWidth = 224;

        //--------------------------------------------------
        // Mechanics

        private enum FlyDirection
        {
            Left,
            Right
        }

        private Rectangle _flyRange;
        private FlyDirection _flyDirection;

        //----------------------//------------------------//

        public Parrot(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.Parrot;

            // Stand
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Combat system init
            _hp = 4;
            _damage = 2;
            _flyDirection = FlyDirection.Right;

            IgnoreGravity = true;
        }

        public void SetFlyWidth(int width)
        {
            _flyWidth = width;
        }

        public void SetFlyRange(int x, int y)
        {
            _flyRange = new Rectangle(x, y, _flyWidth, 32);
        }

        public override void ReceiveAttack(int damage, Vector2 subjectPosition)
        {
            if (_dying || IsImunity) return;

            CharacterSprite.RequestImmunityAnimation();

            _hp = _hp - damage < 0 ? 0 : _hp - damage;
            if (_hp == 0)
            {
                IgnoreGravity = false;
                _dyingAcceleration = Math.Sign(Position.X - subjectPosition.X) * 0.7f;
                OnDie();
            }
        }

        public override void Update(GameTime gameTime)
        {
            _velocity.X = _flyDirection == FlyDirection.Right ? 200f : -200f;
            base.Update(gameTime);
            if (_flyDirection == FlyDirection.Right && Position.X >= _flyRange.Right)
            {
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
                _flyDirection = FlyDirection.Left;
            }
            else if (_flyDirection == FlyDirection.Left && Position.X <= _flyRange.Left)
            {
                CharacterSprite.Effect = SpriteEffects.None;
                _flyDirection = FlyDirection.Right;
            }
        }

        public override void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else
                CharacterSprite.SetFrameList("stand");
        }
    }
}
