using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Super_Pete_The_Pirate.Sprites;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate
{
    class CharacterBase
    {
        //--------------------------------------------------
        // Character sprite

        public CharacterSprite CharacterSprite;

        //--------------------------------------------------
        // Combat system

        protected bool _requestAttack;
        protected bool _isAttacking;
        protected int _attackType;
        protected float _attackCooldownTick;
        protected string[] _attackFrameList;
        protected bool _requestErase;
        public bool RequestErase { get { return _requestErase; } }
        public float AttackCooldown { get; set; }

        protected int _hp;
        protected bool _dying;
        public bool Dying { get { return _dying; } }

        public bool IsImunity { get { return CharacterSprite.ImmunityAnimationActive; } }

        protected bool _shot;

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //--------------------------------------------------
        // Physics variables

        protected float previousBottom;

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        protected Vector2 _position;

        public Vector2 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }
        protected Vector2 _velocity;

        protected float _knockbackAcceleration;
        private float _dyingAcceleration;

        //--------------------------------------------------
        // Constants for controling horizontal movement

        protected const float MoveAcceleration = 13000.0f;
        protected const float MaxMoveSpeed = 1750.0f;
        protected const float GroundDragFactor = 0.48f;
        protected const float AirDragFactor = 0.58f;

        //--------------------------------------------------
        // Constants for controlling vertical movement

        protected const float MaxJumpTime = 0.35f;
        protected const float JumpLaunchVelocity = -2500.0f;
        protected const float GravityAcceleration = 3400.0f;
        protected const float DyingGravityAcceleration = 2500.0f;
        protected const float MaxFallSpeed = 550.0f;
        protected const float JumpControlPower = 0.14f;
        protected const float PlayerSpeed = 0.3f;

        /// <summary>
        /// Gets whether or not the player's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return _isOnGround; }
        }
        protected bool _isOnGround;

        /// <summary>
        /// Current user movement input.
        /// </summary>
        public float _movement;

        // Jumping state
        protected bool _isJumping;
        protected bool _wasJumping;
        protected float _jumpTime;
        
        public Rectangle BoundingRectangle
        {
            get
            {
                var collider = CharacterSprite.GetBlockCollider();
                int left = (int)Math.Round(Position.X) + collider.OffsetX;
                int top = (int)Math.Round(Position.Y) + collider.OffsetY;
                return new Rectangle(left, top, collider.Width, collider.Height);
            }
        }

        //----------------------//------------------------//

        public CharacterBase(Texture2D texture)
        {
            CharacterSprite = new CharacterSprite(texture);

            // Physics variables init
            _knockbackAcceleration = 0f;
            _dyingAcceleration = 0f;

            // Battle system init
            _requestAttack = false;
            _isAttacking = false;
            _attackType = -1;
            _attackCooldownTick = 0f;
            AttackCooldown = 0f;
            _shot = false;
            _dying = false;

            _rand = new Random();

            _hp = 1;
        }

        public void RequestAttack(int type)
        {
            if (_attackCooldownTick <= 0f)
            {
                _requestAttack = true;
                _attackType = type;
            }
        }

        public virtual void ReceiveAttack(int damage, Vector2 subjectPosition)
        {
            if (_dying || IsImunity) return;

            CharacterSprite.RequestImmunityAnimation();

            _knockbackAcceleration = Math.Sign(BoundingRectangle.Center.X - subjectPosition.X) * 5000f;
            _velocity.Y = -300f;

            _hp = _hp - damage < 0 ? 0 : _hp - damage;
            if (_hp == 0)
            {
                CharacterSprite.RequestDyingAnimation();
                _velocity.Y -= 100f;
                _dyingAcceleration = Math.Sign(Position.X - subjectPosition.X) * 0.7f;
                _dying = true;
            }
        }

        public void ReceiveAttackWithPoint(int damage, Rectangle subjectRect)
        {
            var position = new Vector2(subjectRect.Center.X, subjectRect.Center.Y);
            ReceiveAttack(damage, position);
        }

        public virtual void ReceiveAttackWithCollider(int damage, Rectangle subjectRect, SpriteCollider colider)
        {
            ReceiveAttackWithPoint(damage, subjectRect);
        }

        public virtual void Update(GameTime gameTime)
        {
            var lastOnGround = _isOnGround;

            ApplyPhysics(gameTime);

            _movement = 0.0f;
            _isJumping = false;

            UpdateAttackCooldown(gameTime);
            UpdateAttack(gameTime);
            UpdateSprite(gameTime);
            if (CharacterSprite.DyingAnimationEnded) _requestErase = true;

            if (!lastOnGround && _isOnGround)
            {
                CreateGroundImpactParticles();
            }
                
        }

        private void UpdateAttackCooldown(GameTime gameTime)
        {
            if (_attackCooldownTick > 0f)
            {
                _attackCooldownTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }

        public virtual void UpdateAttack(GameTime gameTime)
        {
            if (_isAttacking)
            {
                if (CharacterSprite.Looped)
                {
                    _isAttacking = false;
                    _attackType = -1;
                    _shot = false;
                }
                else
                {
                    var sprite = CharacterSprite;
                    if (sprite.GetCurrentFramesList().FramesToAttack.Contains(sprite.CurrentFrame))
                    {
                        DoAttack();
                    }
                }
            }

            if (_requestAttack)
            {
                _isAttacking = true;
                _requestAttack = false;
                _attackCooldownTick = AttackCooldown;
            }
        }

        public virtual void DoAttack() { }

        public virtual void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else if (_isAttacking)
                CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            else if (!_isOnGround)
                CharacterSprite.SetFrameList("jumping");
            else
                CharacterSprite.SetFrameList("stand");
        }

        private void UpdateSprite(GameTime gameTime)
        {
            UpdateFrameList();
            CharacterSprite.SetPosition(Position);
            CharacterSprite.Update(gameTime);
        }

        #region Particles

        public virtual void CreateGroundImpactParticles()
        {
            var left = 0;
            for (var i = 0; i < 6; i++)
            {
                int side = 1;
                if (left < 3)
                {
                    side = -1;
                    left++;
                }

                var position = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Bottom);
                position.X += _rand.Next(5, 8) * side;
                var velocity = new Vector2(_rand.NextFloat(2f, 5f) * side, _rand.NextFloat(-1f, -3f)) * 3f;
                var size = new Vector2(_rand.NextFloat(5f, 7f), _rand.NextFloat(4f, 6f));

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    AlphaBase = _rand.NextFloat(0.2f, 0.6f),
                    Type = ParticleType.Smoke,
                    UseCustomVelocity = true,
                    VelocityMultiplier = 1.05f
                };

                SceneManager.Instance.ParticleManager.CreateParticle(ImageManager.loadParticle("Smoke"), position, Color.White, 1000f, size, state);
            }
        }

        public virtual void CreateJumpParticles()
        {
            var left = 0;
            for (var i = 0; i < 8; i++)
            {
                int side = 1;
                if (left < 3)
                {
                    side = -1;
                    left++;
                }

                var position = new Vector2(BoundingRectangle.Center.X, BoundingRectangle.Bottom);
                position.X += _rand.Next(5, 11) * side;
                var velocity = new Vector2(_rand.NextFloat(6f, 9f) * side, _rand.NextFloat(-4f, -9f)) * 3f;
                var size = new Vector2(_rand.NextFloat(4f, 7f), _rand.NextFloat(3f, 6f));

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    AlphaBase = _rand.NextFloat(0.2f, 0.6f),
                    Type = ParticleType.Smoke,
                    UseCustomVelocity = true
                };

                SceneManager.Instance.ParticleManager.CreateParticle(ImageManager.loadParticle("Smoke"), position, Color.White, 1000f, size, state);
            }
        }

        #endregion

        #region Collision

        /// <summary>
        /// Updates the player's velocity and position based on input, gravity, etc.
        /// </summary>
        public void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 previousPosition = Position;

            // Base velocity is a combination of horizontal movement control and
            // acceleration downward due to gravity.
            if (_dying) _velocity.X = _dyingAcceleration * MoveAcceleration * elapsed;
            else _velocity.X += (_movement * MoveAcceleration * elapsed);

            UpdateKnockback(elapsed);

            var gravity = _dying ? DyingGravityAcceleration : GravityAcceleration;
            _velocity.Y = MathHelper.Clamp(_velocity.Y + gravity * elapsed, -MaxFallSpeed, MaxFallSpeed);
            _velocity.Y = DoJump(_velocity.Y, gameTime);

            // Apply pseudo-drag horizontally.
            if (_dying)
                _velocity.X *= 0.8f;
            else if (IsOnGround)
                _velocity.X *= GroundDragFactor;
            else
                _velocity.X *= AirDragFactor;

            // Prevent the player from running faster than his top speed.            
            _velocity.X = MathHelper.Clamp(_velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            // Apply velocity.
            Position += _velocity * elapsed;
            Position = new Vector2((float)Math.Round(Position.X), (float)Math.Round(Position.Y));

            // If the player is now colliding with the level, separate them.
            if (!_dying)
                HandleCollisions();

            // If the collision stopped us from moving, reset the velocity to zero.
            if (Position.X == previousPosition.X)
                _velocity.X = 0;

            if (Position.Y == previousPosition.Y)
            {
                _velocity.Y = 0;
                _jumpTime = 0.0f;
            }
        }

        private void UpdateKnockback(float elapsed)
        {
            if (_knockbackAcceleration != 0.0f)
            {
                _velocity.X += (_knockbackAcceleration * elapsed);
                _knockbackAcceleration *= 0.9f;
                if (Math.Abs(_knockbackAcceleration) < 100f) _knockbackAcceleration = 0;
            }
        }

        /// <summary>
        /// Calculates the Y velocity accounting for jumping and
        /// animates accordingly.
        /// </summary>
        /// <remarks>
        /// During the accent of a jump, the Y velocity is completely
        /// overridden by a power curve. During the decent, gravity takes
        /// over. The jump velocity is controlled by the jumpTime field
        /// which measures time into the accent of the current jump.
        /// </remarks>
        /// <param name="velocityY">
        /// The player's current velocity along the Y axis.
        /// </param>
        /// <returns>
        /// A new Y velocity if beginning or continuing a jump.
        /// Otherwise, the existing Y velocity.
        /// </returns>
        private float DoJump(float velocityY, GameTime gameTime)
        {
            // If the player wants to jump
            if (_isJumping)
            {
                // Begin or continue a jump
                if ((!_wasJumping && IsOnGround) || _jumpTime > 0.0f)
                {
                    _jumpTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                // If we are in the ascent of the jump
                if (0.0f < _jumpTime && _jumpTime <= MaxJumpTime)
                {
                    // Fully override the vertical velocity with a power curve that gives players more control over the top of the jump
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(_jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }
            _wasJumping = _isJumping;

            return velocityY;
        }

        private void HandleCollisions()
        {
            // Get the player's bounding rectangle and find neighboring tiles.
            Rectangle bounds = BoundingRectangle;
            var tileSize = GameMap.Instance.TileSize.X;
            int leftTile = (int)Math.Floor((float)bounds.Left / tileSize);
            int rightTile = (int)Math.Ceiling(((float)bounds.Right / tileSize)) - 1;
            int topTile = (int)Math.Floor((float)bounds.Top / tileSize);
            int bottomTile = (int)Math.Ceiling(((float)bounds.Bottom / tileSize)) - 1;

            // Reset flag to search for ground collision.
            _isOnGround = false;

            // For each potentially colliding tile,
            for (int y = topTile; y <= bottomTile; ++y)
            {
                for (int x = leftTile; x <= rightTile; ++x)
                {
                    // If this tile is collidable,
                    GameMap.TileCollision collision = GameMap.Instance.GetCollision(x, y);
                    if (collision != GameMap.TileCollision.Passable)
                    {
                        // Determine collision depth (with direction) and magnitude.
                        Rectangle tileBounds = GameMap.Instance.GetTileBounds(x, y);
                        Vector2 depth = RectangleExtensions.GetIntersectionDepth(bounds, tileBounds);
                        if (depth != Vector2.Zero)
                        {
                            float absDepthX = Math.Abs(depth.X);
                            float absDepthY = Math.Abs(depth.Y);

                            // Resolve the collision along the shallow axis.
                            if (absDepthY < absDepthX || collision == GameMap.TileCollision.Platform || (_velocity.X == 0))
                            {
                                // If we crossed the top of a tile, we are on the ground.
                                if (previousBottom <= tileBounds.Top)
                                    _isOnGround = true;

                                // Ignore platforms, unless we are on the ground.
                                if (collision == GameMap.TileCollision.Block || IsOnGround)
                                {
                                    // Resolve the collision along the Y axis.
                                    Position = new Vector2(Position.X, Position.Y + depth.Y);

                                    // Perform further collisions with the new bounds.
                                    bounds = BoundingRectangle;
                                }
                            }
                            else if (collision == GameMap.TileCollision.Block) // Ignore platforms.
                            {
                                // Resolve the collision along the X axis.
                                Position = new Vector2(Position.X + depth.X, Position.Y);

                                // Perform further collisions with the new bounds.
                                bounds = BoundingRectangle;
                            }
                        }
                    }
                }
            }

            // Save the new bounds bottom.
            previousBottom = bounds.Bottom;
        }
        #endregion

        #region Draw

        public void DrawCharacter(SpriteBatch spriteBatch)
        {
            CharacterSprite.Draw(spriteBatch, new Vector2(BoundingRectangle.X, BoundingRectangle.Y));
        }

        public virtual void DrawColliderBox(SpriteBatch spriteBatch)
        {
            CharacterSprite.DrawColliders(spriteBatch);
        }

        #endregion
    }
}
