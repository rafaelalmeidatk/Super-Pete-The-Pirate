using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Sprites
{
    public class FrameInfo
    {
        public Rectangle SpriteSheetInfo { get; set; }
        public List<SpriteCollider> AttackColliders { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }

        public FrameInfo(Rectangle spriteSheetInfo, int offsetX, int offsetY)
        {
            SpriteSheetInfo = spriteSheetInfo;
            AttackColliders = new List<SpriteCollider>();
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
