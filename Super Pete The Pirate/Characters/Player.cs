using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;

namespace Super_Pete_The_Pirate
{
    class Player : CharacterBase
    {


        private const int NormalAttack = 0;
        private const int AerialAttack = 1;
        private const int Shot = 2;

        //----------------------//------------------------//

        public Player(Texture2D texture) : base(texture)
        {
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(0, 0, 32, 32));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32),
            });

            CharacterSprite.CreateFrameList("walking", 120);
            CharacterSprite.AddCollider("walking", new Rectangle(0, 0, 32, 32));
            CharacterSprite.AddFrames("walking", new List<Rectangle>()
            {
                new Rectangle(128, 0, 32, 32),
                new Rectangle(160, 0, 32, 32),
                new Rectangle(192, 0, 32, 32),
                new Rectangle(224, 0, 32, 32),
            });

            CharacterSprite.CreateFrameList("jumping", 0);
            CharacterSprite.AddCollider("jumping", new Rectangle(0, 0, 32, 32));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            });

            CharacterSprite.CreateFrameList("attack_sword", 40);
            CharacterSprite.AddCollider("attack_sword", new Rectangle(0, 0, 32, 32));
            CharacterSprite.AddCollider("attack_sword", new Rectangle(41, 14, 22, 5), SpriteCollider.ColliderType.Attack);
            CharacterSprite.AddFrames("attack_sword", new List<Rectangle>()
            {
                new Rectangle(32, 32, 32, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(128, 32, 64, 32)
            });

            Position = new Vector2(50, 160);

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_sword",
                "attack_aero",
                "attack_shot"
            };
        }

        public override void Update(GameTime gameTime)
        {
            CheckKeys(gameTime);
            base.Update(gameTime);
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
                _isAttacking = true;
                _attackType = NormalAttack;
            }
        }
    }
}
