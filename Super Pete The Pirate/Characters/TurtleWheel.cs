using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Scenes;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Characters
{
    class TurtleWheel : Enemy
    {
        //--------------------------------------------------
        // Physics constants

        private new const float MaxMoveSpeed = 1850.0f;
        private const float InitialWheelTime = 4000f;
        private const float InitialConfusionTime = 3000f;

        //--------------------------------------------------
        // Mechanics
        
        private float _movementSide;

        private bool _enterWheelMode;
        private float _wheelTick;

        private float _acceleration;
        private int _hitGroundFromImpact;

        private float _emitSparkTime;

        private float _confusionTick;

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

            // Confusion
            CharacterSprite.CreateFrameList("confusion", 150);
            CharacterSprite.AddCollider("confusion", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("confusion", new List<Rectangle>()
            {
                new Rectangle(0, 160, 32, 64),
                new Rectangle(32, 160, 32, 64),
                new Rectangle(64, 160, 32, 64),
                new Rectangle(96, 160, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Damage
            CharacterSprite.CreateFrameList("damage", 0);
            CharacterSprite.AddCollider("damage", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("damage", new List<Rectangle>()
            {
                new Rectangle(0, 224, 32, 64),
            }, new int[] { 0 }, new int[] { -32 });

            // Dying
            CharacterSprite.CreateFrameList("dying", 0);
            CharacterSprite.AddCollider("dying", new Rectangle(2, 0, 28, 32));
            CharacterSprite.AddFrames("dying", new List<Rectangle>()
            {
                new Rectangle(0, 224, 32, 64),
            }, new int[] { 0 }, new int[] { -32 });

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
            _confusionTick = 0f;

            CreateViewRange();
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (Position.X - playerPosition.X > 0)
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
            else
                CharacterSprite.Effect = SpriteEffects.None;

            if (!_wheelMode && !_enterWheelMode && _confusionTick < 1f) InitWheelMode(playerPosition.X);
        }

        public override void ReceiveAttack(int damage, Vector2 subjectPosition)
        {
            if (!_wheelMode)
                base.ReceiveAttack(damage, subjectPosition);
        }

        public override void ReceiveAttackWithCollider(int damage, Rectangle subjectRect, SpriteCollider colider)
        {
            if (_wheelMode)
            {
                var x = Math.Sign(subjectRect.Center.X - BoundingRectangle.Center.X) < 0 ? BoundingRectangle.Left : BoundingRectangle.Right;
                var y = colider.BoundingBox.Center.Y;
                ((SceneMap)SceneManager.Instance.GetCurrentScene()).CreateSparkParticle(new Vector2(x, y));
            }
            base.ReceiveAttackWithCollider(damage, subjectRect, colider);
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
            if (_confusionTick > 0f) _confusionTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        private void UpdateEnterWheel()
        {
            if (CharacterSprite.CurrentFrameList == "enter_wheel_mode" && CharacterSprite.Looped)
            {
                _enterWheelMode = false;
                _wheelMode = true;
                _wheelTick = InitialWheelTime;
                CharacterSprite.SetFrameList("wheel_mode");
                _emitSparkTime = 800f;
            }
        }

        private void UpdateWheelTick(GameTime gameTime)
        {
            _wheelTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_wheelTick < 1f)
            {
                _wheelMode = false;
                _wheelTick = 0f;
                _confusionTick = InitialConfusionTime;
                _emitSparkTime = 1000f;
            }
            else
            {
                _emitSparkTime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_emitSparkTime < 0f && _isOnGround)
                {
                    var point = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Bottom);
                    ((SceneMap)SceneManager.Instance.GetCurrentScene()).CreateSparkParticle(point, 30);
                    _emitSparkTime = _rand.Next(200, 1000);
                }
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
            if (!_dying && !CharacterSprite.ImmunityAnimationActive)
            {
                if (_confusionTick > 0f)
                    CharacterSprite.SetFrameList("confusion");
                else if (_enterWheelMode)
                    CharacterSprite.SetFrameList("enter_wheel_mode");
                else if (_wheelMode)
                    CharacterSprite.SetFrameList("wheel_mode");
            }
            else
            {
                base.UpdateFrameList();
            }
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
