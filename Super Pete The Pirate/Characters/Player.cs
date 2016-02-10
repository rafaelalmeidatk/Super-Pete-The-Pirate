﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;
using System.Diagnostics;
using Super_Pete_The_Pirate.Scenes;

namespace Super_Pete_The_Pirate
{
    class Player : CharacterBase
    {
        //--------------------------------------------------
        // Attacks constants

        private const int NormalAttack = 0;
        private const int AerialAttack = 1;
        private const int ShotAttack = 2;

        public bool tes = false;

        //----------------------//------------------------//

        //CharacterSprite.AddFrames("stand", new List<Rectangle>()
        //    {
        //        new Rectangle(0, 0, 32, 32),
        //        new Rectangle(32, 0, 32, 32),
        //        new Rectangle(64, 0, 32, 32),
        //        new Rectangle(96, 0, 32, 32),
        //    }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

        public Player(Texture2D texture) : base(texture)
        {
            // Stand
            CharacterSprite.CreateFrameList("stand", 150);
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
            CharacterSprite.CreateFrameList("attack_sword", 50);
            CharacterSprite.AddCollider("attack_sword", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFramesToAttack("attack_sword", 1, 2);
            CharacterSprite.AddFrames("attack_sword", new List<Rectangle>()
            {
                new Rectangle(32, 32, 32, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            CharacterSprite.AddAttackCollider("attack_sword", new List<List<Rectangle>>()
            {
                new List<Rectangle>() { },
                new List<Rectangle>() { new Rectangle(31, 14, 32, 6) },
                new List<Rectangle>() { new Rectangle(31, 14, 32, 6) }
            }, 64);

            // Aerial Attack
            CharacterSprite.CreateFrameList("attack_aerial", 50);
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

            Position = new Vector2(32, 160);

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_sword",
                "attack_aerial",
                "attack_shot"
            };

            AttackCooldown = 300f;
        }

        public override void Update(GameTime gameTime)
        {
            CheckKeys(gameTime);
            base.Update(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (_dying)
                CharacterSprite.SetIfFrameListExists("dying");
            else if (_isAttacking)
                CharacterSprite.SetFrameList(_attackFrameList[_attackType]);
            else if (Velocity.Y != 0)
                CharacterSprite.SetFrameList("jumping");
            else if (InputManager.Instace.KeyDown(Keys.Left) || InputManager.Instace.KeyDown(Keys.Right))
                CharacterSprite.SetFrameList("walking");
            else
                CharacterSprite.SetFrameList("stand");
        }

        private void CheckKeys(GameTime gameTime)
        {
            // Movement
            if (InputManager.Instace.KeyDown(Keys.Left))
            {
                CharacterSprite.SetDirection(SpriteDirection.Left);
                _movement = -1.0f; 
            }
            else if (InputManager.Instace.KeyDown(Keys.Right))
            {
                CharacterSprite.SetDirection(SpriteDirection.Right);
                _movement = 1.0f;
            }

            _isJumping = InputManager.Instace.KeyDown(Keys.Up);

            // Attack
            if (InputManager.Instace.KeyPressed(Keys.A) && !_isAttacking)
            {
                StartAttack();
            }
        }

        private void StartAttack()
        {
            if (_isOnGround)
                RequestAttack(NormalAttack);
            else
                RequestAttack(AerialAttack);
        }

        public override void UpdateAttack(GameTime gameTime)
        {
            base.UpdateAttack(gameTime);
            if (_isOnGround && _attackType == AerialAttack)
            {
                _isAttacking = false;
                _attackType = -1;
                _attackCooldownTick = 0;
            }
        }

        public override void DoAttack()
        {
            var damage = _attackType == ShotAttack ? 2 : 1;
            var enemies = ((SceneMap)SceneManager.Instance.GetCurrentScene()).Enemies;
            foreach (var attackCollider in CharacterSprite.GetCurrentFramesList().Frames[CharacterSprite.CurrentFrame].AttackColliders)
            {
                foreach (var enemy in enemies)
                {
                    if (attackCollider.BoundingBox.Intersects(enemy.BoundingRectangle))
                    {
                        enemy.ReceiveAttack(damage, this.Position);
                    }
                }
            }
        }
    }
}