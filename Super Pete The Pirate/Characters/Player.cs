using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;
using System.Diagnostics;
using Super_Pete_The_Pirate.Scenes;
using Super_Pete_The_Pirate.Objects;
using Super_Pete_The_Pirate.Managers;

namespace Super_Pete_The_Pirate
{
    class Player : CharacterBase
    {
        //--------------------------------------------------
        // Attacks constants

        private const int NormalAttack = 0;
        private const int AerialAttack = 1;
        private const int ShotAttack = 2;
        private const int ShotAttackJump = 3;

        //--------------------------------------------------
        // Coins
        
        public int Coins => PlayerManager.Instance.Coins;

        //--------------------------------------------------
        // Lives
        
        public int Lives => PlayerManager.Instance.Lives;

        //--------------------------------------------------
        // Ammo
        
        public int Ammo => PlayerManager.Instance.Ammo;

        //--------------------------------------------------
        // HP

        public new int HP => PlayerManager.Instance.Hearts;

        //--------------------------------------------------
        // Death tick

        private float _deathTick;
        private const float DeathMaxTick = 1500.0f;

        //--------------------------------------------------
        // Request respawn

        private bool _requestRespawn;
        public bool RequestRespawn => _requestRespawn;

        //--------------------------------------------------
        // Keys locked (no movement)

        private bool _keysLocked;

        //--------------------------------------------------
        // Keys locked (no movement)

        private bool _active;
        public bool Active => _active;

        //----------------------//------------------------//

        public Player(Texture2D texture) : base(texture)
        {
            // Stand
            CharacterSprite.CreateFrameList("stand", 140);
            CharacterSprite.AddCollider("stand", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32),
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });
            
            // Walking
            CharacterSprite.CreateFrameList("walking", 120);
            CharacterSprite.AddCollider("walking", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("walking", new List<Rectangle>()
            {
                new Rectangle(128, 0, 32, 32),
                new Rectangle(160, 0, 32, 32),
                new Rectangle(192, 0, 32, 32),
                new Rectangle(224, 0, 32, 32),
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Jumping
            CharacterSprite.CreateFrameList("jumping", 0);
            CharacterSprite.AddCollider("jumping", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Sword Attack
            CharacterSprite.CreateFrameList("attack_sword", 70);
            CharacterSprite.AddCollider("attack_sword", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFramesToAttack("attack_sword", 1, 2);
            CharacterSprite.AddFrames("attack_sword", new List<Rectangle>()
            {
                new Rectangle(32, 32, 32, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32),
                new Rectangle(192, 32, 64, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            CharacterSprite.AddAttackCollider("attack_sword", new List<List<Rectangle>>()
            {
                new List<Rectangle>() { },
                new List<Rectangle>() { new Rectangle(31, 14, 32, 6) },
                new List<Rectangle>() { new Rectangle(31, 14, 32, 6) }
            }, 64);

            // Aerial Attack
            CharacterSprite.CreateFrameList("attack_aerial", 60);
            CharacterSprite.AddCollider("attack_aerial", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("attack_aerial", new List<Rectangle>()
            {
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
                new Rectangle(128, 64, 64, 64),
                new Rectangle(192, 64, 64, 64)
            }, new int[] { 0, 0, -32, -32 }, new int[] { -32, 0, 0, -32 });
            CharacterSprite.AddAttackCollider("attack_aerial", new List<List<Rectangle>>()
            {
                new List<Rectangle>()
                {
                    new Rectangle(25, -21, 27, 21),
                    new Rectangle(31, 0, 25, 27)
                },

                new List<Rectangle>()
                {
                    new Rectangle(32, 25, 21, 27),
                    new Rectangle(5, 31, 27, 25)
                },

                new List<Rectangle>()
                {
                    new Rectangle(-20, 32, 27, 21),
                    new Rectangle(-24, 5, 25, 27)
                },

                new List<Rectangle>()
                {
                    new Rectangle(-21, -20, 21, 27),
                    new Rectangle(0, -24, 27, 25)
                }
            }, 64);
            CharacterSprite.AddFramesToAttack("attack_aerial", 0, 1, 2, 3);

            // Shot Attack
            CharacterSprite.CreateFrameList("attack_shot", 80);
            CharacterSprite.AddCollider("attack_shot", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFramesToAttack("attack_shot", 2);
            CharacterSprite.AddFrames("attack_shot", new List<Rectangle>()
            {
                new Rectangle(0, 128, 32, 32),
                new Rectangle(32, 128, 64, 32),
                new Rectangle(96, 128, 64, 32),
                new Rectangle(160, 128, 64, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            CharacterSprite.CreateFrameList("attack_shot_jumping", 50);
            CharacterSprite.AddCollider("attack_shot_jumping", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFramesToAttack("attack_shot_jumping", 3);
            CharacterSprite.AddFrames("attack_shot_jumping", new List<Rectangle>()
            {
                new Rectangle(0, 160, 32, 32),
                new Rectangle(32, 160, 64, 32),
                new Rectangle(96, 160, 64, 32),
                new Rectangle(160, 160, 64, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            Position = new Vector2(32, 160);

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_sword",
                "attack_aerial",
                "attack_shot",
                "attack_shot_jumping"
            };

            AttackCooldown = 300f;
            _deathTick = 0.0f;
            _requestRespawn = false;
            _active = true;
            _keysLocked = false;
        }

        public override void GainHP(int amount)
        {
            PlayerManager.Instance.AddHearts(amount);
        }

        public override int GetHp()
        {
            return HP;
        }

        public void Update(GameTime gameTime, bool keyLock)
        {
            if (!_active) return;

            _keysLocked = keyLock;
            if (!keyLock)
                CheckKeys(gameTime);

            if (_dying)
            {
                _deathTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (_deathTick >= DeathMaxTick)
                {
                    _requestRespawn = true;
                    _active = false;
                }
            }

            base.Update(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else if (_isAttacking)
            {
                if (_attackType == ShotAttack)
                {
                    if (!_isOnGround)
                        CharacterSprite.SetFrameList("attack_shot_jumping");
                    else
                        CharacterSprite.SetFrameList("attack_shot");
                }
                else
                    CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            } else if (!_isOnGround)
                CharacterSprite.SetFrameList("jumping");
            else if ((InputManager.Instace.KeyDown(Keys.Left) || InputManager.Instace.KeyDown(Keys.Right)) && !_keysLocked)
                CharacterSprite.SetFrameList("walking");
            else
                CharacterSprite.SetFrameList("stand");
        }

        private void CheckKeys(GameTime gameTime)
        {
            // Movement
            if (InputManager.Instace.KeyDown(Keys.Left) && Math.Abs(_knockbackAcceleration) < 1200f)
            {
                CharacterSprite.SetDirection(SpriteDirection.Left);
                _movement = -1.0f; 
            }
            else if (InputManager.Instace.KeyDown(Keys.Right) && Math.Abs(_knockbackAcceleration) < 1200f)
            {
                CharacterSprite.SetDirection(SpriteDirection.Right);
                _movement = 1.0f;
            }

            if (InputManager.Instace.KeyPressed(Keys.G))
                _knockbackAcceleration = 5000f;

            _isJumping = InputManager.Instace.KeyDown(Keys.C);

            if (InputManager.Instace.KeyPressed(Keys.C) && _isOnGround)
                CreateJumpParticles();

            // Attack
            if (!_isAttacking &&
                ((_isOnGround && InputManager.Instace.KeyPressed(Keys.S)) || (!_isOnGround && InputManager.Instace.KeyDown(Keys.S))))
                StartNormalAttack();

            if (InputManager.Instace.KeyPressed(Keys.A) && !_isAttacking)
                RequestAttack(ShotAttack);
        }

        private void StartNormalAttack()
        {
            if (_isOnGround)
                RequestAttack(NormalAttack);
            else
                RequestAttack(AerialAttack);
        }

        public override void UpdateAttack(GameTime gameTime)
        {
            base.UpdateAttack(gameTime);
            if (_isOnGround)
            {
                if (_attackType == AerialAttack)
                {
                    _isAttacking = false;
                    _attackType = -1;
                    _attackCooldownTick = 0;
                }

                if (_attackType == ShotAttack && CharacterSprite.CurrentFrameList == _attackFrameList[ShotAttackJump])
                    CharacterSprite.SetFrameListOnly(_attackFrameList[ShotAttack]);
            } else if (_attackType == ShotAttack && CharacterSprite.CurrentFrameList == _attackFrameList[ShotAttack])
            {
                CharacterSprite.SetFrameListOnly(_attackFrameList[ShotAttackJump]);
            }
        }

        public override void DoAttack()
        {
            var damage = _attackType == ShotAttack ? 2 : 1;

            if (_attackType == ShotAttack)
            {
                Shot(damage);
                return;
            }

            var sceneMap = (SceneMap)SceneManager.Instance.GetCurrentScene();
            var enemies = sceneMap.Enemies;
            foreach (var attackCollider in CharacterSprite.GetCurrentFramesList().Frames[CharacterSprite.CurrentFrame].AttackColliders)
            {
                for (var i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].CanReceiveAttacks && attackCollider.BoundingBox.Intersects(enemies[i].BoundingRectangle))
                    {
                        enemies[i].ReceiveAttackWithCollider(damage, this.BoundingRectangle, attackCollider);
                    }
                }
            }
        }

        private void Shot(int damage)
        {
            if (_shot) return;
            _shot = true;

            var position = Position;
            var dx = 5;

            // Initial position of the projectile
            if (CharacterSprite.Effect == SpriteEffects.FlipHorizontally)
            {
                position += new Vector2(13, 14);
                dx *= -1;
            }
            else
            {
                position += new Vector2(45, 16);
            }

            if (PlayerManager.Instance.Ammo <= 0)
            {
                if (CharacterSprite.Effect == SpriteEffects.FlipHorizontally)
                    position -= new Vector2(23, 0);
                CreateConfettiParticles(position, Math.Sign(dx));
                return;
            }

            ((SceneMap)SceneManager.Instance.GetCurrentScene()).CreateProjectile("common", position, dx, 0, damage, ProjectileSubject.FromPlayer);
            PlayerManager.Instance.AddAmmo(-1);
        }

        private void CreateConfettiParticles(Vector2 position, int signal)
        {
            var texture = ImageManager.loadParticle("WhitePoint");
            for (var i = 0; i < _rand.Next(2, 5); i++)
            {
                var velocity = new Vector2(_rand.NextFloat(10f, 100f) * signal, _rand.NextFloat(-220f, -100f));
                var color = ColorUtil.HSVToColor(MathHelper.ToRadians(_rand.NextFloat(0, 359)), 0.6f, 0.95f);
                var scale = _rand.Next(0, 2) == 0 ? new Vector2(2, 2) : new Vector2(3, 3);

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    Type = ParticleType.Confetti,
                    Gravity = 1.8f,
                    UseCustomVelocity = true,
                    VelocityMultiplier = 0.95f
                };

                SceneManager.Instance.ParticleManager.CreateParticle(texture, position, color, 800f, scale, state);
            }
        }
    }
}
