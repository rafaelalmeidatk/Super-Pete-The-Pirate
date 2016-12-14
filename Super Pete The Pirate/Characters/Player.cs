using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Super_Pete_The_Pirate.Characters;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Objects;
using Super_Pete_The_Pirate.Scenes;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;

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
        // Touched spikes
        
        public bool TouchedSpikes { get; set; }

        //--------------------------------------------------
        // Active

        private bool _active;
        public bool Active => _active;

        //--------------------------------------------------
        // Hat Cut Scene

        private bool _onHatCutScene;
        private bool _hatDropping;
        private AnimatedSprite _hatSprite;
        private float _hatDroppingSpeed;
        private const float HatDroppingMaxTime = 4800f;
        private bool _withHat;

        private bool _flashScreen;
        private Texture2D _flashTexture;
        private float _flashScreenAlpha;

        private float _flashAfterDelay;
        private const float FlashAfterDelayMax = 500f;

        private bool _hatReceived;

        private bool _thumbsUp;
        private float _thumbsUpTick;
        private const float ThumbsUpDelayMax = 1000.0f;

        private bool _walkingWithHat;

        //--------------------------------------------------
        // SEs

        private SoundEffect _aerialAttackSe;
        private SoundEffect _normalAttackSe;
        private SoundEffect _shotEmptyAttackSe;
        private SoundEffect _footstepSe;
        private SoundEffect _hatSe;

        private float _footstepCooldown;
        private float _footstepTick;

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

            // Stand with hat
            CharacterSprite.CreateFrameList("stand_with_hat", 140);
            CharacterSprite.AddCollider("stand_with_hat", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("stand_with_hat", new List<Rectangle>()
            {
                new Rectangle(128, 192, 32, 64),
                new Rectangle(160, 192, 32, 64),
                new Rectangle(192, 192, 32, 64),
                new Rectangle(224, 192, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

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

            // Walking with hat
            CharacterSprite.CreateFrameList("walking_with_hat", 220);
            CharacterSprite.AddCollider("walking_with_hat", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("walking_with_hat", new List<Rectangle>()
            {
                new Rectangle(32, 256, 32, 64),
                new Rectangle(64, 256, 32, 64),
                new Rectangle(96, 256, 32, 64),
                new Rectangle(128, 256, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

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

            // Shot Attack Jumping
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

            // Hat Received
            CharacterSprite.CreateFrameList("hat_received", 1000, false);
            CharacterSprite.AddCollider("hat_received", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("hat_received", new List<Rectangle>()
            {
                new Rectangle(0, 192, 32, 64),
                new Rectangle(32, 192, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Thumbs up
            CharacterSprite.CreateFrameList("thumbs_up", 140, false);
            CharacterSprite.AddCollider("thumbs_up", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("thumbs_up", new List<Rectangle>()
            {
                new Rectangle(64, 192, 32, 64),
                new Rectangle(96, 192, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Looking up
            CharacterSprite.CreateFrameList("looking_up", 140);
            CharacterSprite.AddCollider("looking_up", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("looking_up", new List<Rectangle>()
            {
                new Rectangle(0, 256, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Looking up with hat
            CharacterSprite.CreateFrameList("looking_up_with_hat", 140);
            CharacterSprite.AddCollider("looking_up_with_hat", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("looking_up_with_hat", new List<Rectangle>()
            {
                new Rectangle(0, 192, 32, 64),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

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

            // Hat drop
            var hatTexture = ImageManager.loadMisc("PeteHat");
            _hatSprite = new AnimatedSprite(hatTexture, new Rectangle[] {
                new Rectangle(0, 0, 96, 64),
                new Rectangle(96, 0, 96, 64),
                new Rectangle(192, 0, 96, 64),
                new Rectangle(288, 0, 96, 64),
                new Rectangle(384, 0, 96, 64),
                new Rectangle(0, 64, 96, 64),
                new Rectangle(96, 64, 96, 64),
                new Rectangle(192, 64, 96, 64),
                new Rectangle(288, 64, 96, 64),
                new Rectangle(384, 64, 96, 64),
            }, 100, Vector2.Zero);
            _hatSprite.Origin = new Vector2(50, 37);

            // Flash texture
            _flashTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _flashTexture.SetData(new Color[] { Color.White });

            // SEs init
            _aerialAttackSe = SoundManager.LoadSe("Aerial");
            _normalAttackSe = SoundManager.LoadSe("Sword");
            _shotEmptyAttackSe = SoundManager.LoadSe("ShotPeteEmpty");
            _footstepSe = SoundManager.LoadSe("Footstep");
            _hatSe = SoundManager.LoadSe("Hat");
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
            if (_onHatCutScene)
            {
                if (_hatDropping)
                {
                    UpdateHatDrop(gameTime);
                }
                UpdateHatCutSceneEnd(gameTime);
                UpdateSprite(gameTime);
                base.Update(gameTime);
                return;
            }

            if (!_active) return;

            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            _keysLocked = keyLock;
            if (!keyLock)
                CheckKeys(gameTime);

            if (_dying)
            {
                _deathTick += deltaTime;
                if (_deathTick >= DeathMaxTick)
                {
                    _requestRespawn = true;
                    _active = false;
                }
            }

            if (_movement != 0 && _isOnGround)
            {
                _footstepTick += deltaTime;
                if (_footstepTick >= _footstepCooldown)
                {
                    _footstepSe.PlaySafe();
                    _footstepCooldown = _rand.NextFloat(250.0f, 320.0f);
                    _footstepTick = 0.0f;
                }
            }

            base.Update(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_walkingWithHat)
            {
                CharacterSprite.SetFrameList("walking_with_hat");
            }
            else if (_thumbsUp)
            {
                CharacterSprite.SetFrameList("thumbs_up");
            }
            else if (_hatReceived)
            {
                CharacterSprite.SetFrameList("hat_received");
            }
            else if (_withHat)
            {
                CharacterSprite.SetFrameList("looking_up_with_hat");
            }
            else if (_onHatCutScene)
            {
                CharacterSprite.SetFrameList("looking_up");
            }
            else if (_dying)
            {
                CharacterSprite.SetIfFrameListExists("dying");
            }
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
            }
            else if (!_isOnGround)
            {
                CharacterSprite.SetFrameList("jumping");
            }
            else if ((InputManager.Instace.KeyDown(Keys.Left) || InputManager.Instace.KeyDown(Keys.Right)) && !_keysLocked && !_onHatCutScene)
            {
                CharacterSprite.SetFrameList("walking");
            }
            else
            {
                CharacterSprite.SetFrameList("stand");
            }
        }

        private void CheckKeys(GameTime gameTime)
        {
            if (!_dying)
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

                _isJumping = InputManager.Instace.KeyDown(Keys.C);

                if (InputManager.Instace.KeyPressed(Keys.C) && _isOnGround)
                    CreateJumpParticles();

                // Attack
                if (!_isAttacking)
                {
                    if (!_isAttacking &&
                        ((_isOnGround && InputManager.Instace.KeyPressed(Keys.X)) || (!_isOnGround && InputManager.Instace.KeyDown(Keys.X))))
                        StartNormalAttack();

                    if (InputManager.Instace.KeyPressed(Keys.Z) && !_isAttacking && !_dying)
                        RequestAttack(ShotAttack);
                }

                // Run
                _running = InputManager.Instace.KeyDown(Keys.A);
            }
        }

        private void StartNormalAttack()
        {
            if (_isOnGround)
            {
                RequestAttack(NormalAttack);
                _normalAttackSe.PlaySafe();
            }
            else
            {
                var oldAttackType = _attackType;
                RequestAttack(AerialAttack);
                if (_attackType == AerialAttack && oldAttackType != AerialAttack)
                    _aerialAttackSe.PlaySafe();
            }
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
                position += new Vector2(13, 10);
                dx *= -1;
            }
            else
            {
                position += new Vector2(45, 12);
            }

            if (PlayerManager.Instance.Ammo <= 0)
            {
                if (CharacterSprite.Effect == SpriteEffects.FlipHorizontally)
                    position -= new Vector2(23, 0);
                CreateConfettiParticles(position, Math.Sign(dx));
                SoundManager.PlaySafe(_shotEmptyAttackSe);
                return;
            }

            ((SceneMap)SceneManager.Instance.GetCurrentScene()).CreateProjectile("common", position, dx, 0, damage, ProjectileSubject.FromPlayer);
            PlayerManager.Instance.AddAmmo(-1);
        }

        protected override void OnGroundLand()
        {
            base.OnGroundLand();
            _footstepSe.PlaySafe();
            _footstepTick = 0.0f;
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

        public void PerformHatDrop()
        {
            _onHatCutScene = true;
            _hatDropping = true;
            CharacterSprite.Effect = SpriteEffects.None;
            _hatSprite.Position = new Vector2(Position.X + CharacterSprite.GetFrameWidth() / 2, 0);
            _hatDroppingSpeed = BoundingRectangle.Top / HatDroppingMaxTime;
        }

        private void UpdateHatDrop(GameTime gameTime)
        {
            var lastFrame = _hatSprite.CurrentFrame;
            _hatSprite.Update(gameTime);

            if (_hatSprite.IsVisible && _hatSprite.CurrentFrame == 0 && lastFrame != 0)
            {
                _hatSe.PlaySafe();
            }

            if (_hatSprite.Position.Y >= BoundingRectangle.Top)
            {
                _flashScreen = true;
                _hatSprite.Pause();
                _hatSe.Dispose();
            }
            else
            {
                var newPositionY = _hatSprite.Position.Y + (float)gameTime.ElapsedGameTime.TotalMilliseconds * _hatDroppingSpeed;
                _hatSprite.Position = new Vector2(_hatSprite.Position.X, newPositionY);
            }
        }

        private void UpdateHatCutSceneEnd(GameTime gameTime)
        {
            if (_flashScreen)
            {
                if (_withHat)
                {
                    _flashScreenAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
                }
                else
                {
                    _flashScreenAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds * 5;
                    if (_flashScreenAlpha >= 1.3f)
                    {
                        _hatDropping = false;
                        _withHat = true;
                        _normalAttackSe.PlaySafe();
                    }
                }
            }

            if (_withHat && !_hatDropping && !_walkingWithHat)
            {
                if (_thumbsUp && _hatReceived && CharacterSprite.Looped)
                {
                    _thumbsUpTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_thumbsUpTick >= ThumbsUpDelayMax)
                    {
                        _walkingWithHat = true;
                    }
                }
                else if (!_thumbsUp && _hatReceived && CharacterSprite.Looped)
                {
                    _thumbsUp = true;
                }
                else if (!_hatReceived)
                {
                    _flashAfterDelay += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_flashAfterDelay >= FlashAfterDelayMax)
                    {
                        _hatReceived = true;
                    }
                }
            }

            if (_walkingWithHat)
            {
                _movement = 1.0f;
                if (Position.X >= GameMap.Instance.MapWidth - 48)
                {
                    PlayerManager.Instance.CompleteAllStages();
                    SceneManager.Instance.ChangeScene("SceneCredits");
                }
            }
        }

        public void DrawHat(SpriteBatch spriteBatch)
        {
            if (!_hatDropping) return;
            _hatSprite.Draw(spriteBatch);
        }

        public void DrawScreenFlash(SpriteBatch spriteBatch)
        {
            if (!_flashScreen) return;
            var screenSize = SceneManager.Instance.VirtualSize;
            spriteBatch.Draw(_flashTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White * _flashScreenAlpha);
        }

        protected override float GetMoveAcceleration()
        {
            return _onHatCutScene ? 4000.0f : base.GetMoveAcceleration();
        }
    }
}
