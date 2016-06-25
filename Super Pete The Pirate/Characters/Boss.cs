using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Super_Pete_The_Pirate.Extensions.Utils;

namespace Super_Pete_The_Pirate.Characters
{
    class Boss : Enemy
    {
        //--------------------------------------------------
        // Attacks constants

        private const int Melee = 0;
        private const int Dash = 1;
        private const int Cannonballs = 2;

        //--------------------------------------------------
        // Direction

        private Direction _direction;

        //--------------------------------------------------
        // Dash

        private Direction _dashDirection;
        private float _dashDelayTick;
        private const float DashDelayMaxTick = 1000.0f;
        private int _dashCount;
        private bool _preparingDash;
        private bool _isDashing;

        //--------------------------------------------------
        // Requesting Shot

        private bool _requestingShot;
        public bool RequestingShot => _requestingShot;

        //----------------------//------------------------//

        public Boss(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.Boss;

            // Stand
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 96, 96),
                new Rectangle(96, 0, 96, 96),
                new Rectangle(192, 0, 96, 96),
                new Rectangle(288, 0, 96, 96)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Melee Attack
            CharacterSprite.CreateFrameList("melee_attack", 120, false);
            CharacterSprite.AddCollider("melee_attack", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("melee_attack", new List<Rectangle>()
            {
                new Rectangle(0, 320, 96, 94),
                new Rectangle(96, 320, 128, 94),
                new Rectangle(224, 320, 128, 94),
                new Rectangle(352, 320, 128, 94),
                new Rectangle(480, 320, 128, 94)
            }, new int[] { 0, -32, -32, -32, -32 }, new int[] { -29, -29, -29, -29, -29 });

            // Dash preparation
            CharacterSprite.CreateFrameList("dash_preparation", 100, false);
            CharacterSprite.AddCollider("dash_preparation", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("dash_preparation", new List<Rectangle>()
            {
                new Rectangle(0, 96, 96, 96),
                new Rectangle(96, 96, 96, 96),
            }, new int[] { 0, 0 }, new int[] { -32, -32 });

            // Dash attack
            CharacterSprite.CreateFrameList("dash_attack", 40);
            CharacterSprite.AddCollider("dash_attack", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("dash_attack", new List<Rectangle>()
            {
                new Rectangle(192, 96, 128, 96),
                new Rectangle(320, 96, 128, 96),
                new Rectangle(448, 96, 128, 96),
                new Rectangle(572, 96, 128, 96)
            }, new int[] { -16, -16, -16, -16 }, new int[] { -32, -32, -32, -32 });

            // Preparation of Cannons
            CharacterSprite.CreateFrameList("cannonballs", 100, false);
            CharacterSprite.AddCollider("cannonballs", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("cannonballs", new List<Rectangle>()
            {
                new Rectangle(0, 192, 96, 128),
                new Rectangle(96, 192, 96, 128),
                new Rectangle(192, 192, 96, 128),
                new Rectangle(288, 192, 96, 128)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -64, -64, -64, -64 });

            // Damage
            CharacterSprite.CreateFrameList("damage", 130);
            CharacterSprite.AddCollider("damage", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("damage", new List<Rectangle>()
            {
                new Rectangle(512, 0, 128, 96),
                new Rectangle(384, 0, 128, 96),
            }, new int[] { 0, 0 }, new int[] { -32, -32 });

            // Attacks setup
            _attackFrameList = new string[]
            {
                "melee_attack",
                "dash_attack",
                "cannonballs"
            };

            // Combat system init
            _hp = 50;
            _viewRangeSize = new Vector2(10, 74);
            _viewRangeOffset = new Vector2(0, -5);
            _damage = 2;
            _dashDelayTick = DashDelayMaxTick;

            // Direction init
            _direction = CharacterSprite.Effect == SpriteEffects.None ? Direction.Left : Direction.Right;

            CreateViewRange();
        }

        public override void RequestAttack(int type)
        {
            if (type == Dash)
            {
                _preparingDash = true;
                CharacterSprite.SetFrameList("dash_preparation");
            }
            else
            {
                base.RequestAttack(type);
            }
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (IsFreeToAttack())
                RequestAttack(Melee);
        }

        public override void Update(GameTime gameTime)
        {
            if (_dashDelayTick > 0f)
                _dashDelayTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_isDashing)
                _velocity.X = _dashDirection == Direction.Right ? 1000f : -1000f;

            base.Update(gameTime);

            if (_isDashing && _dashDirection == Direction.Left && Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                _isDashing = false;
                _velocity.X = 0;
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
                _direction = Direction.Right;
                _dashDelayTick = DashDelayMaxTick;
            }
            else if (_isDashing && _dashDirection == Direction.Right && Position.X > 288)
            {
                Position = new Vector2(288, Position.Y);
                _isDashing = false;
                _velocity.X = 0;
                CharacterSprite.Effect = SpriteEffects.None;
                _direction = Direction.Left;
                _dashDelayTick = DashDelayMaxTick;
            }

            UpdateSpriteEffect();
        }

        public override void UpdateAttack(GameTime gameTime)
        {
            if (IsFreeToAttack() && _dashDelayTick <= 0 && _dashCount < 2)
            {
                RequestAttack(Dash);
                _dashCount++;
            }
            else if (IsFreeToAttack() && _dashDelayTick <= 0)
            {
                RequestAttack(Cannonballs);
                _dashDelayTick = DashDelayMaxTick;
                _dashCount = 0;
            }

            if (_preparingDash && CharacterSprite.Looped)
            {
                _preparingDash = false;
                _isDashing = true;
                if (Position.X >= SceneManager.Instance.VirtualSize.X / 2)
                    _dashDirection = Direction.Left;
                else
                    _dashDirection = Direction.Right;
            }

            if (_isAttacking && _attackType == Cannonballs && CharacterSprite.Looped)
            {
                _requestingShot = true;
                _dashDelayTick = DashDelayMaxTick * 2.0f;   
            }
                

            if (!_preparingDash && !_isDashing)
                base.UpdateAttack(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else if (CharacterSprite.ImmunityAnimationActive)
                CharacterSprite.SetIfFrameListExists("damage");
            else if (_preparingDash)
                CharacterSprite.SetFrameList("dash_preparation");
            else if (_isDashing)
                CharacterSprite.SetFrameList("dash_attack");
            else if (_isAttacking)
                CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            else if (!_isOnGround)
                CharacterSprite.SetIfFrameListExists("jumping");
            else
                CharacterSprite.SetFrameList("stand");
        }

        private void UpdateSpriteEffect()
        {
            if (_isDashing)
            {
                if (_velocity.X < 0 && CharacterSprite.Effect == SpriteEffects.FlipHorizontally)
                    CharacterSprite.Effect = SpriteEffects.None;
                else if (_velocity.X > 0 && CharacterSprite.Effect == SpriteEffects.None)
                    CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
            }
            else
            {
                CharacterSprite.Effect = _direction == Direction.Left ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            }
        }

        public bool IsFreeToAttack()
        {
            return !_isDashing && !_preparingDash && !_isAttacking && !_requestAttack && !_requestErase;
        }
    }
}
