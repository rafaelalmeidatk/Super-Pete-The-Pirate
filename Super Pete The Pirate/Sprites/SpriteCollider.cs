using Microsoft.Xna.Framework;

namespace Super_Pete_The_Pirate.Sprites
{
    public class SpriteCollider
    {
        public enum ColliderType
        {
            Block,
            Attack
        }
        public int Width { get; set; }
        public int Height { get; set; }
        public int AttackWidth { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public Vector2 Position { get; set; }
        public ColliderType Type { get; set;}
        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)Position.X + OffsetX, (int)Position.Y + OffsetY, Width, Height); }
        }

        public SpriteCollider(int offsetX, int offsetY, int width, int height)
        {
            OffsetX = offsetX;
            OffsetY = offsetY;
            Width = width;
            Height = height;
        }
    }
}
