using Microsoft.Xna.Framework;
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

        public Player(Texture2D texture) : base(texture)
        {
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32),
            });

            CharacterSprite.CreateFrameList("walking", 120);
            CharacterSprite.AddCollider("walking", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("walking", new List<Rectangle>()
            {
                new Rectangle(128, 0, 32, 32),
                new Rectangle(160, 0, 32, 32),
                new Rectangle(192, 0, 32, 32),
                new Rectangle(224, 0, 32, 32),
            });

            CharacterSprite.CreateFrameList("jumping", 0);
            CharacterSprite.AddCollider("jumping", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            });

            CharacterSprite.CreateFrameList("attack_sword", 50);
            CharacterSprite.AddCollider("attack_sword", new Rectangle(9, 2, 17, 30));
            CharacterSprite.AddAttackCollider("attack_sword", new Rectangle(31, 14, 32, 6), 64); // new Rectangle(41, 14, 22, 5)
            CharacterSprite.AddFramesToAttack("attack_sword", 1, 2);
            CharacterSprite.AddFrames("attack_sword", new List<Rectangle>()
            {
                new Rectangle(32, 32, 32, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32)
            });

            CharacterSprite.CreateFrameList("attack_aerial", 50);
            //CharacterSprite.AddCollider("attack_aerial", new Rectangle()

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
                RequestAttack(NormalAttack);
            }
        }

        public override void DoAttack()
        {
            var damage = _attackType == ShotAttack ? 2 : 1;
            var enemies = ((SceneMap)SceneManager.Instance.GetCurrentScene()).Enemies;
            foreach (var attackCollider in CharacterSprite.GetCurrentFramesList().Colliders)
            {
                if (attackCollider.Type != SpriteCollider.ColliderType.Attack) continue;
                foreach (var enemy in enemies)
                {
                    if (attackCollider.BoundingBox.Intersects(enemy.BoundingRectangle))
                    {
                        enemy.ReceiveAttack(damage);
                    }
                }
            }
        }
    }
}
