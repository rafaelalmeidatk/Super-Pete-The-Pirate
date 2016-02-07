using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;
using System.Diagnostics;

namespace Super_Pete_The_Pirate.Characters
{
    class Enemy : CharacterBase
    {
        public Enemy(Texture2D texture) : base(texture)
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

            CharacterSprite.CreateFrameList("dying", 120);
            CharacterSprite.AddCollider("dying", new Rectangle(0, 0, 32, 32));
            CharacterSprite.AddFrames("dying", new List<Rectangle>()
            {
                new Rectangle(128, 0, 32, 32),
                new Rectangle(160, 0, 32, 32),
                new Rectangle(192, 0, 32, 32),
                new Rectangle(224, 0, 32, 32),
            });
        }
    }
}
