using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Characters
{
    class TurtleWheel : Enemy
    {
        //--------------------------------------------------
        // Physics constants

        private new const float MaxMoveSpeed = 1850.0f;
        private const float InitialWheelTime = 3000f;
        private const float InitialConfusionTime = 3000f;

        //--------------------------------------------------
        // Mechanics

        private CharacterBase _target;
        private float _movementSide;

        private bool _enterWheelMode;
        private float _wheelTick;

        private float _acceleration;
        private int _hitGroundFromImpact;

        private float _counfusionTick;

        //----------------------//------------------------//

        public TurtleWheel(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.TurtleWheel;

            // Stand
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 64),
                new Rectangle(32, 0, 32, 64),
                new Rectangle(64, 0, 32, 64),
                new Rectangle(96, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Jumping
            CharacterSprite.CreateFrameList("jumping", 150);
            CharacterSprite.AddCollider("jumping", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 64),
                new Rectangle(32, 0, 32, 64),
                new Rectangle(64, 0, 32, 64),
                new Rectangle(96, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Enter Wheel Mode
            CharacterSprite.CreateFrameList("enter_wheel_mode", 80, false);
            CharacterSprite.AddCollider("enter_wheel_mode", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("enter_wheel_mode", new List<Rectangle>()
            {
                new Rectangle(0, 96, 32, 64),
                new Rectangle(32, 96, 32, 64),
                new Rectangle(64, 96, 32, 64),
                new Rectangle(96, 96, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Wheel Mode
            CharacterSprite.CreateFrameList("wheel_mode", 150);
            CharacterSprite.AddCollider("wheel_mode", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("wheel_mode", new List<Rectangle>()
            {
                new Rectangle(0, 64, 32, 32),
                new Rectangle(32, 64, 32, 32),
                new Rectangle(64, 64, 32, 32),
                new Rectangle(96, 64, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_roll"
            };

            // Combat system init
            _hp = 3;
            _damage = 3;
            _viewRangeSize = new Vector2(150, 150);
            _viewRangeOffset = new Vector2(0, -100);
            _movementSide = 1f;
            _acceleration = 0f;

            // Mechanics init
            _enterWheelMode = false;
            _wheelTick = 0f;
            _wheelMode = false;
            _hitGroundFromImpact = 0;
            _counfusionTick = 0f;

            CreateViewRange();
        }

        public void SetTarget(CharacterBase target)
        {
            _target = target;
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (Position.X - playerPosition.X > 0)
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
            else
                CharacterSprite.Effect = SpriteEffects.None;

            if (!_wheelMode && !_enterWheelMode && _counfusionTick < 1f) InitWheelMode(playerPosition.X);
        }

        private void InitWheelMode(float targetX)
        {
            _movementSide = Math.Sign(targetX - Position.X);
            _movement = 0f;
            _acceleration = 0f;
            _enterWheelMode = true;
            _velocity.Y = -1000f;
            CharacterSprite.SetFrameList("enter_wheel_mode");
        }

        public override void Update(GameTime gameTime)
        {
            var previousPosition = Position;
            if (_enterWheelMode) UpdateEnterWheel();
            if (_wheelMode) UpdateWheel((float)gameTime.ElapsedGameTime.TotalMilliseconds);
            base.Update(gameTime);
            CheckCollided(previousPosition);
            if (_wheelTick > 0f) UpdateWheelTick(gameTime);
            if (_counfusionTick > 0f) _counfusionTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void UpdateEnterWheel()
        {
            if (CharacterSprite.CurrentFrameList == "enter_wheel_mode" && CharacterSprite.Looped)
            {
                _enterWheelMode = false;
                _wheelMode = true;
                _wheelTick = InitialWheelTime;
                CharacterSprite.SetFrameList("wheel_mode");
            }
        }

        private void UpdateWheelTick(GameTime gameTime)
        {
            _wheelTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_wheelTick < 1f)
            {
                _wheelMode = false;
                _wheelTick = 0f;
                _counfusionTick = InitialConfusionTime;
            }
        }

        private void CheckCollided(Vector2 previousPosition)
        {
            if (!_wheelMode) return;

            if (Position.Y == previousPosition.Y && _isOnGround)
            {
                if (_hitGroundFromImpact == 1)
                {
                    _velocity.Y -= 180f;
                    _knockbackAcceleration += 1500f * _movementSide;
                    _hitGroundFromImpact = 2;
                } else if (_hitGroundFromImpact == 2)
                {
                    _velocity.Y -= 120f;
                    _knockbackAcceleration += 0800f * _movementSide;
                    _hitGroundFromImpact = 0;
                }
            } 

            if (Math.Abs(_acceleration) >= 1f && Position.X == previousPosition.X)
            {
                _velocity.Y -= 300f;
                _knockbackAcceleration = _movementSide * -1 * 15000f;
                _movement = 0f;
                _movementSide *= -1;
                _hitGroundFromImpact = 1;
            }
        }

        private void UpdateWheel(float elapsed)
        {
            if (Math.Abs(_movement) < MaxMoveSpeed)
            {
                _movement = MathHelper.Lerp(_acceleration, _movementSide, 0.1f) + (0.01f * _movementSide);
                _acceleration = _movement;
            }
        }

        public override void UpdateFrameList()
        {
            if (_enterWheelMode)
                CharacterSprite.SetFrameList("enter_wheel_mode");
            else if (_wheelMode)
                CharacterSprite.SetFrameList("wheel_mode");
            else
                base.UpdateFrameList();
        }

        public override void CreateGroundImpactParticles()
        {
            if (!_wheelMode)
            {
                base.CreateGroundImpactParticles();
                return;
            }

            for (var i = 0; i < 6; i++)
            {
                var position = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Bottom);
                position.X += _rand.Next(-5, 5);
                var velocity = new Vector2(_rand.NextFloat(-5f, 5f), _rand.NextFloat(-1f, -3f)) * 3f;
                var size = new Vector2(_rand.NextFloat(5f, 7f), _rand.NextFloat(4f, 6f));

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    AlphaBase = _rand.NextFloat(0.2f, 0.6f),
                    Type = ParticleType.Smoke
                };

                SceneManager.Instance.ParticleManager.CreateParticle(ImageManager.loadParticle("Smoke"), position, Color.White, 1000f, size, state);
            }
        }
    }
}
