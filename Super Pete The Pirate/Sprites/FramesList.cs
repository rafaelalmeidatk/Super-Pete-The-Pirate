using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Sprites
{
    public class FramesList
    {
        public int Delay { get; set; }
        public List<Rectangle> Frames { get; set; }
        public bool Loop { get; set; }
        public List<SpriteCollider> Colliders { get; set; }
        public SpriteCollider Collider { get { return Colliders[0]; } }
        public FramesList(int delay)
        {
            Delay = delay;
            Loop = Delay > 0;
        }
    }
}
