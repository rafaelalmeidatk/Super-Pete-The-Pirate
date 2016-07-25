using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Scenes;
using Super_Pete_The_Pirate.Managers;

namespace Super_Pete_The_Pirate.Characters
{
    class SniperPig : Enemy
    {
        //--------------------------------------------------
        // Attacks constants
        
        private const int ShotAttack = 0;

        //--------------------------------------------------
        // Mechanics

        private bool _alertAnimation;

        //----------------------//------------------------//

        public SniperPig(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.SniperPig;

            // Stand
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(4, -2, 26, 34));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 0, 64, 64),
                new Rectangle(64, 0, 64, 64),
                new Rectangle(128, 0, 64, 64),
                new Rectangle(192, 0, 64, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Alert
            CharacterSprite.CreateFrameList("alert", 120);
            CharacterSprite.AddCollider("alert", new Rectangle(4, -2, 26, 34));
            CharacterSprite.AddFrames("alert", new List<Rectangle>()
            {
                new Rectangle(0, 64, 64, 64),
                new Rectangle(64, 64, 64, 64),
                new Rectangle(128, 64, 64, 64),
                new Rectangle(192, 64, 64, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Attack shot
            CharacterSprite.CreateFrameList("attack_shot", 120);
            CharacterSprite.AddCollider("attack_shot", new Rectangle(4, -2, 26, 34));
            CharacterSprite.AddFrames("attack_shot", new List<Rectangle>()
            {
                new Rectangle(0, 128, 64, 64),
                new Rectangle(64, 128, 64, 64),
                new Rectangle(128, 128, 64, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });
            CharacterSprite.AddFramesToAttack("attack_shot", 1);

            // Jumping
            CharacterSprite.CreateFrameList("jumping", 0);
            CharacterSprite.AddCollider("jumping", new Rectangle(3, 0, 25, 32));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 32, 32, 32)
            }, new int[] { 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0 });

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_shot"
            };

            // Combat system init
            _hp = 2;
            _viewRangeSize = new Vector2(300, 20);
            _alertAnimation = false;
            _shot = false;
            _damage = 2;

            // SE init
            _hitSe = SoundManager.LoadSe("PigHit");

            CreateViewRange();
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (Position.X - playerPosition.X > 0)
                CharacterSprite.Effect = SpriteEffects.FlipHorizontally;
            else
                CharacterSprite.Effect = SpriteEffects.None;
            if (!_alertAnimation && !_isAttacking)
            {
                _alertAnimation = true;
                _viewRangeCooldown = 1500f;
                CharacterSprite.SetFrameList("alert");
            }
        }

        public override void UpdateAttack(GameTime gameTime)
        {
            if (_alertAnimation && CharacterSprite.Looped)
            {
                var teste = CharacterSprite.CurrentFrameList;
                _alertAnimation = false;
                RequestAttack(ShotAttack);
                base.UpdateAttack(gameTime);
            }
            else base.UpdateAttack(gameTime);
        }

        public override void UpdateFrameList()
        {
            if (!_alertAnimation)
                base.UpdateFrameList();
        }

        public override void DoAttack()
        {
            if (_shot || _dying) return;

            var position = Position;
            var dx = 5;

            // Initial position of the projectile
            if (CharacterSprite.Effect == SpriteEffects.FlipHorizontally)
            {
                position += new Vector2(-25, 16);
                dx *= -1;
            }
            else position += new Vector2(56, 16);
            ((SceneMap)SceneManager.Instance.GetCurrentScene()).CreateProjectile("common", position, dx, 0, _damage, Objects.ProjectileSubject.FromEnemy);
            _shot = true;
        }
    }
}
