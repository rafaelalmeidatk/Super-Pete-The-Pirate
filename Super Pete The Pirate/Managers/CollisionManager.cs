using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate
{
    public static class CollisionManager
    {
        public static bool IntersectCharacterSpritePixels(CharacterSprite spriteA, CharacterSprite spriteB)
        {
            if (!spriteA.BoundingBox.Intersects(spriteB.BoundingBox))
            {
                return false;
            }

            // Get the current frame for each sprite
            var rectangleA = spriteA.GetCurrentFrameRectangle();
            var rectangleB = spriteB.GetCurrentFrameRectangle();

            // Create the color data based on the current frame
            Color[] dataA = new Color[rectangleA.Width * rectangleA.Height];
            spriteA.TextureRegion.Texture.GetData(0, new Rectangle(rectangleA.X, rectangleA.Y, rectangleA.Width, rectangleA.Height), dataA, 0, dataA.Length);

            Color[] dataB = new Color[rectangleB.Width * rectangleB.Height];
            spriteB.TextureRegion.Texture.GetData(0, new Rectangle(rectangleB.X, rectangleB.Y, rectangleB.Width, rectangleB.Height), dataB, 0, dataB.Length);

            // Find the bounds of the rectangle intersection
            var boundingA = spriteA.BoundingBox;
            var boundingB = spriteB.BoundingBox;
            
            int top = Math.Max(boundingA.Y, boundingB.Y);
            int bottom = Math.Min(boundingA.Bottom, boundingB.Bottom);
            int left = Math.Max(boundingA.Left, boundingB.Left);
            int right = Math.Min(boundingA.Right, boundingB.Right);

            // Check every point within the intersectionbounds
            for (int y = top; y <bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - boundingA.X) + (y - boundingA.Y) * rectangleA.Width];
                    Color colorB = dataB[(x - boundingB.X) + (y - boundingB.Y) * rectangleB.Width];

                    // Ifboth pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}
