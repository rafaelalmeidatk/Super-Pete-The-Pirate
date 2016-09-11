using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Sprites;
using System;

namespace Super_Pete_The_Pirate
{
    public class CharacterBase : PhysicalObject
    {
        //--------------------------------------------------
        // Character sprite

        public CharacterSprite CharacterSprite;

        //--------------------------------------------------
        // Combat system

        protected bool _requestAttack;
        protected bool _isAttacking;
        public bool IsAttacking { get { return _isAttacking; } }
        protected int _attackType;
        protected float _attackCooldownTick;
        protected string[] _attackFrameList;
        protected bool _requestErase;
        public bool RequestErase { get { return _requestErase; } }
        public float AttackCooldown { get; set; }
        public bool IsImunity { get { return CharacterSprite.ImmunityAnimationActive; } }
        protected bool _shot;
        protected int _hp;
        public int HP { get { return _hp; } }

        //--------------------------------------------------
        // Damage stuff

        protected bool _canReceiveAttacks;
        public virtual bool CanReceiveAttacks { get { return _canReceiveAttacks; } }

        protected bool _contactDamageEnabled;
        public virtual bool ContactDamageEnabled { get { return _contactDamageEnabled; } }

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //--------------------------------------------------
        // Prevent first OnGroundLand

        private bool _firstGroudLand;

        //--------------------------------------------------
        // Hit Se

        protected SoundEffect _hitSe;

        //--------------------------------------------------
        // Bounding Rectangle

        public override Rectangle BoundingRectangle
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
            IgnoreGravity = false;

            // Battle system init
            _hp = 1;
            _requestAttack = false;
            _isAttacking = false;
            _attackType = -1;
            _attackCooldownTick = 0f;
            AttackCooldown = 0f;
            _shot = false;
            _dying = false;
            _canReceiveAttacks = true;
            _contactDamageEnabled = true;

            // Rand init
            _rand = new Random();

            _firstGroudLand = false;

            // SE init

            _hitSe = SoundManager.LoadSe("Hit");
        }

        public T Clone<T>() where T : CharacterBase
        {
            var characterSprite = CharacterSprite.Clone();
            var clone = (T)MemberwiseClone();
            clone.CharacterSprite = characterSprite;
            return clone;
        }

        public virtual void RequestAttack(int type)
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

            GainHP(-damage);
            if (GetHp() <= 0)
            {
                _dyingAcceleration = Math.Sign(Position.X - subjectPosition.X) * 0.7f;
                OnDie();
            }

            _hitSe.PlaySafe();
        }

        public virtual void GainHP(int amount)
        {
            _hp += amount;
        }

        public virtual int GetHp()
        {
            return _hp;
        }

        public void ReceiveAttackWithRect(int damage, Rectangle subjectRect)
        {
            var position = new Vector2(subjectRect.Center.X, subjectRect.Center.Y);
            ReceiveAttack(damage, position);
        }

        public virtual void ReceiveAttackWithCollider(int damage, Rectangle subjectRect, SpriteCollider colider)
        {
            ReceiveAttackWithRect(damage, subjectRect);
        }

        public virtual void OnDie()
        {
            CharacterSprite.RequestDyingAnimation();
            _velocity.Y -= 100f;
            _dying = true;
        }

        public override void Update(GameTime gameTime)
        {
            var lastOnGround = _isOnGround;

            base.Update(gameTime);

            UpdateAttackCooldown(gameTime);
            UpdateAttack(gameTime);
            UpdateSprite(gameTime);
            if (CharacterSprite.DyingAnimationEnded) _requestErase = true;

            if (!lastOnGround && _isOnGround)
            {
                if (!_firstGroudLand)
                {
                    _firstGroudLand = true;
                    return;
                }
                OnGroundLand();
            }
        }

        protected virtual void OnGroundLand()
        {
            CreateGroundImpactParticles();
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
            else if (CharacterSprite.ImmunityAnimationActive)
                CharacterSprite.SetIfFrameListExists("damage");
            else if (_isAttacking)
                CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            else if (!_isOnGround)
                CharacterSprite.SetIfFrameListExists("jumping");
            else
                CharacterSprite.SetFrameList("stand");
        }

        protected virtual void UpdateSprite(GameTime gameTime)
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
                var velocity = new Vector2(_rand.NextFloat(3f, 6f) * side, _rand.NextFloat(-1f, -3f)) * 3f;
                var size = new Vector2(_rand.NextFloat(5f, 7f), _rand.NextFloat(4f, 6f));

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    AlphaBase = _rand.NextFloat(0.2f, 0.4f),
                    Type = ParticleType.Smoke
                };

                SceneManager.Instance.ParticleManager.CreateParticle(ImageManager.loadParticle("Smoke"), position, Color.White, 800f, size, state);
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
                    AlphaBase = _rand.NextFloat(0.2f, 0.4f),
                    Type = ParticleType.Smoke,
                    UseCustomVelocity = true
                };

                SceneManager.Instance.ParticleManager.CreateParticle(ImageManager.loadParticle("Smoke"), position, Color.White, 800f, size, state);
            }
        }

        #endregion

        #region Draw

        public virtual void DrawCharacter(SpriteBatch spriteBatch)
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
