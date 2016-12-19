using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Objects;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;
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
        // Max HP

        private const int MaxHP = 50;

        //--------------------------------------------------
        // HP HUD

        private Rectangle _hpBackRegion;
        private Rectangle _hpFullRegion;
        private Rectangle _hpHalfRegion;
        private Texture2D _hpSpritesheetTexture;
        private Vector2 _hpBackPosition;
        private Vector2 _hpSpritesPosition;
        private float _hudAlpha;

        //--------------------------------------------------
        // Direction

        private Direction _direction;

        //--------------------------------------------------
        // Dash

        private const float DashSpeed = 1000.0f;
        private Direction _dashDirection;
        private float _dashDelayTick;
        private float _dashDelayMaxTick;
        private int _dashCount;
        private bool _preparingDash;
        private bool _isDashing;
        
        //--------------------------------------------------
        // Max velocity

        private new float MaxMoveSpeed = 400.0f;

        //--------------------------------------------------
        // Requesting Shot

        private bool _requestingShot;
        public bool RequestingShot => _requestingShot;

        //--------------------------------------------------
        // Cannons

        private List<GameCannon> _cannons;
        private List<GameCannon> _cannonsToDestroy;

        //--------------------------------------------------
        // Collapse

        private bool _collapsing;
        public bool Collapsing => _collapsing;
        private class CollapseExplosion
        {
            public Vector2 Position;
            public float Delay;
        }
        private List<CollapseExplosion> _collapseExplosionsQueue;
        private List<AnimatedSprite> _collapseExplosions;
        private float _collapseOpacityTime;
        private const float CollapseOpacityTimeMax = 200.0f;
        
        private Texture2D _flashTexture;
        private float _flashScreenAlpha;
        private float _flashScreenTime;

        private bool _requestingHatDrop;
        public bool RequestingHatDrop => _requestingHatDrop;

        //--------------------------------------------------
        // SEs

        private SoundEffect[] _explosionsSes;

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
            CharacterSprite.CreateFrameList("melee_attack", 130, false);
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
            CharacterSprite.CreateFrameList("dash_preparation", 120, false);
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
            CharacterSprite.CreateFrameList("cannonballs", 120, false);
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

            // Collapsing
            CharacterSprite.CreateFrameList("collapsing", 0);
            CharacterSprite.AddCollider("collapsing", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("collapsing", new List<Rectangle>()
            {
                new Rectangle(512, 0, 128, 96),
            }, new int[] { 0 }, new int[] { -32 });

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
            _dashDelayMaxTick = 1000.0f;
            _dashDelayTick = _dashDelayMaxTick;

            // Direction init
            _direction = CharacterSprite.Effect == SpriteEffects.None ? Direction.Left : Direction.Right;

            // Cannons init
            _cannons = new List<GameCannon>();
            _cannonsToDestroy = new List<GameCannon>();

            // HP HUD init
            _hpBackRegion = new Rectangle(0, 0, 344, 18);
            _hpFullRegion = new Rectangle(344, 0, 12, 13);
            _hpHalfRegion = new Rectangle(356, 0, 12, 13);
            _hpBackPosition = new Vector2(18, 217);
            _hpSpritesPosition = _hpBackPosition + new Vector2(10, 3);
            _hpSpritesheetTexture = ImageManager.loadMisc("BossHPSpritesheet");
            _hudAlpha = 1.0f;

            // Collapse init
            _collapseExplosionsQueue = new List<CollapseExplosion>();
            _collapseExplosions = new List<AnimatedSprite>();

            // Flash texture
            _flashTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _flashTexture.SetData(new Color[] { Color.White });

            // SEs load
            _explosionsSes = new SoundEffect[3];
            for (var i = 0; i < 3; i++)
            {
                _explosionsSes[i] = SoundManager.LoadSe(String.Format("Explosion{0}", i + 1));
            }

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
                if (type == Cannonballs)
                {
                    CreateCannonballs();
                }
                base.RequestAttack(type);
            }
        }

        private void CreateCannonballs()
        {
            var cannonTexture = ImageManager.loadScene("map", "Cannon");
            var cannonFrames = new Rectangle[]
            {
                new Rectangle(0, 0, 64, 32),
                new Rectangle(64, 0, 64, 32),
                new Rectangle(128, 0, 64, 32),
                new Rectangle(192, 0, 64, 32),
                new Rectangle(256, 0, 64, 32),
                new Rectangle(0, 32, 64, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32),
                new Rectangle(192, 32, 64, 32),
                new Rectangle(256, 32, 64, 32)
            };
            var positions = GameMap.Instance.GetCannonPositions();
            var possiblePositionsIndex = new List<int>() { 0, 1, 2 };
            var i = _rand.Next(3);
            var pos1 = positions[possiblePositionsIndex[i]];

            _cannons.Add(new GameCannon(cannonTexture, cannonFrames, 130, (int)pos1.X - 32, (int)pos1.Y - 32));

            possiblePositionsIndex.Remove(i);
            var j = _rand.Next(2);

            if (_rand.Next(2) > 0)
            {
                var pos2 = positions[possiblePositionsIndex[j]];
                _cannons.Add(new GameCannon(cannonTexture, cannonFrames, 130, (int)pos2.X - 32, (int)pos2.Y - 32));
            }

            if (HP < MaxHP * 0.5)
            {
                possiblePositionsIndex.Remove(j);
                var pos3 = positions[possiblePositionsIndex[0]];
                _cannons.Add(new GameCannon(cannonTexture, cannonFrames, 130, (int)pos3.X - 32, (int)pos3.Y - 32));
            }
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (IsFreeToAttack())
                RequestAttack(Melee);
        }

        public override void Update(GameTime gameTime)
        {
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_collapsing)
            {
                UpdateCollapse(gameTime);
                UpdateSprite(gameTime);
                return;
            }

            if (_dashDelayTick > 0f)
                _dashDelayTick -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_isDashing)
                _velocity.X = _dashDirection == Direction.Right ? DashSpeed * deltaTime : -DashSpeed * deltaTime;

            base.Update(gameTime);

            if (_isDashing && _dashDirection == Direction.Left && Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
                _isDashing = false;
                _velocity.X = 0;
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
                _direction = Direction.Right;
                _dashDelayTick = _dashDelayMaxTick;
            }
            else if (_isDashing && _dashDirection == Direction.Right && Position.X > 288)
            {
                Position = new Vector2(288, Position.Y);
                _isDashing = false;
                _velocity.X = 0;
                CharacterSprite.Effect = SpriteEffects.None;
                _direction = Direction.Left;
                _dashDelayTick = _dashDelayMaxTick;
            }

            if (HP < MaxHP * 0.5f)
                _dashDelayMaxTick = 500.0f;

            // Update cannons
            foreach (var cannon in _cannons)
            {
                cannon.Update(gameTime);
                if (cannon.Complete)
                    _cannonsToDestroy.Add(cannon);
            }

            if (_cannonsToDestroy.Count > 0)
            {
                _cannonsToDestroy.ForEach(x => _cannons.Remove(x));
                _cannonsToDestroy.Clear();
            }

            UpdateSpriteEffect();
        }

        private void UpdateCollapse(GameTime gameTime)
        {
            if (_hudAlpha > 0.0f)
            {
                _hudAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            if (_collapseOpacityTime == 0)
            {
                _flashScreenAlpha += (float)gameTime.ElapsedGameTime.TotalMilliseconds / _flashScreenTime;
                if (_flashScreenAlpha > 1.3f)
                {
                    _collapseOpacityTime = CollapseOpacityTimeMax;
                    EraseCannons();
                }
            }
            else if (_flashScreenAlpha > 0.0f)
            {
                _flashScreenAlpha -= (float)gameTime.ElapsedGameTime.TotalSeconds * 2;
            }
            else if (CharacterSprite.Alpha > 0.0f)
            {
                CharacterSprite.Alpha -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / _collapseOpacityTime;
            }
            else if (CharacterSprite.Alpha <= 0.0f)
            {
                _requestingHatDrop = true;
                _requestErase = true;
                _flashTexture.Dispose();
            }

            var explosionsToRemove = new List<CollapseExplosion>();
            var explosionsSpriteToRemove = new List<AnimatedSprite>();
            for (var i = 0; i < _collapseExplosionsQueue.Count; i++)
            {
                var explosion = _collapseExplosionsQueue[i];
                if (explosion.Delay <= 0)
                {
                    var texture = ImageManager.loadMisc("Explosion");
                    var frames = new Rectangle[]
                    {
                        new Rectangle(0, 0, 96, 96),
                        new Rectangle(96, 0, 96, 96),
                        new Rectangle(192, 0, 96, 96),
                        new Rectangle(288, 0, 96, 96),
                        new Rectangle(384, 0, 96, 96),
                        new Rectangle(480, 0, 96, 96),
                        new Rectangle(576, 0, 96, 96)
                    };
                    var position = new Vector2(BoundingRectangle.Left - 48, BoundingRectangle.Top - 84) + explosion.Position;
                    var sprite = new AnimatedSprite(texture, frames, 120, position, false);
                    _collapseExplosions.Add(sprite);
                    explosionsToRemove.Add(explosion);
                    _explosionsSes[_rand.Next(3)].PlaySafe();
                }
                else
                {
                    explosion.Delay -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            foreach (var explosion in explosionsToRemove)
            {
                _collapseExplosionsQueue.Remove(explosion);
            }
            
            foreach (var explosionSprite in _collapseExplosions)
            {
                explosionSprite.Update(gameTime);
                if (explosionSprite.Looped)
                {
                    explosionsSpriteToRemove.Add(explosionSprite);
                }
            }

            foreach (var explosionSprite in explosionsSpriteToRemove)
            {
                _collapseExplosions.Remove(explosionSprite);
            }
        }

        public override void OnDie()
        {
            _collapsing = true;
            _contactDamageEnabled = false;
            _collapseExplosionsQueue.AddRange(new List<CollapseExplosion>()
            {
                new CollapseExplosion() { Position = new Vector2(30, 40), Delay = 0 },
                new CollapseExplosion() { Position = new Vector2(60, 60), Delay = 300 },
                new CollapseExplosion() { Position = new Vector2(50, 80), Delay = 500 },
                new CollapseExplosion() { Position = new Vector2(45, 45), Delay = 700 },
                new CollapseExplosion() { Position = new Vector2(55, 50), Delay = 1000 },
                new CollapseExplosion() { Position = new Vector2(22, 85), Delay = 1300 },
                new CollapseExplosion() { Position = new Vector2(45, 65), Delay = 1500 },
                new CollapseExplosion() { Position = new Vector2(17, 45), Delay = 1700 },
                new CollapseExplosion() { Position = new Vector2(25, 80), Delay = 2000 },
                new CollapseExplosion() { Position = new Vector2(48, 80), Delay = 2500 }
            });
            _flashScreenTime = _collapseExplosionsQueue[_collapseExplosionsQueue.Count - 1].Delay;
        }

        public void EraseCannons()
        {
            _cannons.Clear();
        }

        public override void ReceiveAttack(int damage, Vector2 subjectPosition)
        {
            if (_collapsing) return;
            base.ReceiveAttack(damage, subjectPosition);
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
                _dashDelayTick = _dashDelayMaxTick;
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
                _dashDelayTick = _dashDelayMaxTick * 1.4f;
            }

            if (!_preparingDash && !_isDashing)
                base.UpdateAttack(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_collapsing)
                CharacterSprite.SetFrameList("collapsing");
            else if (_preparingDash)
                CharacterSprite.SetFrameList("dash_preparation");
            else if (_isDashing)
                CharacterSprite.SetFrameList("dash_attack");
            else if (_isAttacking)
                CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            else if (CharacterSprite.ImmunityAnimationActive)
                CharacterSprite.SetIfFrameListExists("damage");
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

        protected override float GetMaxMoveSpeed()
        {
            return MaxMoveSpeed;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_hpSpritesheetTexture, _hpBackPosition, _hpBackRegion, Color.White * _hudAlpha);
            var halfHP = Math.Floor(HP / 2.0f);
            for (var i = 0; i < Math.Ceiling(HP / 2.0f); i++)
            {
                var region = i == halfHP ? _hpHalfRegion : _hpFullRegion;
                var position = _hpSpritesPosition + (region.Width * i + 1 * i) * Vector2.UnitX;
                spriteBatch.Draw(_hpSpritesheetTexture, position, region, Color.White * _hudAlpha);
            }
        }

        public void DrawInnerSprites(SpriteBatch spriteBatch)
        {
            var mapSize = new Vector2(GameMap.Instance.MapWidth, GameMap.Instance.MapHeight);
            _cannons.ForEach(x => x.Draw(spriteBatch));
            if (_flashScreenAlpha > 0.0f)
            {
                spriteBatch.Draw(_flashTexture, new Rectangle(0, 0, (int)mapSize.X, (int)mapSize.Y), Color.White * _flashScreenAlpha);
            }
            _collapseExplosions.ForEach(x => x.Draw(spriteBatch));
        }
    }
}
