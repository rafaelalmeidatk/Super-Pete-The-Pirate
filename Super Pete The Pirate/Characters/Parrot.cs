using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Super_Pete_The_Pirate.Extensions.Utils;

namespace Super_Pete_The_Pirate.Characters
{
    class Parrot : Enemy
    {
        //--------------------------------------------------
        // Mechanics
        
        private int _flyWidth = 224;
        private Rectangle _flyRange;
        private Direction _flyDirection;

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

            // Damage
            CharacterSprite.CreateFrameList("damage", 0);
            CharacterSprite.AddCollider("damage", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("damage", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Dying
            CharacterSprite.CreateFrameList("dying", 0);
            CharacterSprite.AddCollider("dying", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("dying", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Combat system init
            _hp = 2;
            _damage = 2;
            _flyDirection = Direction.Right;

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
            _velocity.X = _flyDirection == Direction.Right ? 200f : -200f;

            base.Update(gameTime);

            if (_flyDirection == Direction.Right && (Position.X >= _flyRange.Right || Velocity.X == 0))
            {
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
                _flyDirection = Direction.Left;
            }
            else if (_flyDirection == Direction.Left && (Position.X <= _flyRange.Left || Velocity.X == 0))
            {
                CharacterSprite.Effect = SpriteEffects.None;
                _flyDirection = Direction.Right;
            }
        }

        public override void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else if (CharacterSprite.ImmunityAnimationActive)
                CharacterSprite.SetIfFrameListExists("damage");
            else
                CharacterSprite.SetFrameList("stand");
        }
    }
}
