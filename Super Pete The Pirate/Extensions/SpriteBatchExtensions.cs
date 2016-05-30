using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate
{
    static class SpriteBatchExtensions
    {
        public static void DrawTextWithShadow(this SpriteBatch spriteBatch, BitmapFont font, string text,
            Vector2 position, Color color, Color shadowColor)
        {
            spriteBatch.DrawString(font, text, position + Vector2.One, shadowColor);
            spriteBatch.DrawString(font, text, position, color);
        }

        public static void DrawTextWithShadow(this SpriteBatch spriteBatch, BitmapFont font, string text,
            Vector2 position, Color color)
        {
            spriteBatch.DrawTextWithShadow(font, text, position, color, Color.Black);
        }
    }
}
