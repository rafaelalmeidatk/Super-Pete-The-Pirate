using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Characters
{
    class Boss : Enemy
    {
        //--------------------------------------------------
        // Mechanics

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

            // Jumping
            CharacterSprite.CreateFrameList("jumping", 0);
            CharacterSprite.AddCollider("jumping", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("jumping", new List<Rectangle>()
            {
                new Rectangle(0, 0, 96, 96)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Damage
            CharacterSprite.CreateFrameList("damage", 90);
            CharacterSprite.AddCollider("damage", new Rectangle(15, 0, 70, 64));
            CharacterSprite.AddFrames("damage", new List<Rectangle>()
            {
                new Rectangle(512, 0, 128, 96),
                new Rectangle(384, 0, 128, 96),
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Attacks setup
            _attackFrameList = new string[]
            {
                "attack_dash"
            };

            // Combat system init
            _hp = 4;
            _viewRangeSize = new Vector2(10, 74);
            _viewRangeOffset = new Vector2(0, -5);
            _damage = 2;

            CreateViewRange();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
