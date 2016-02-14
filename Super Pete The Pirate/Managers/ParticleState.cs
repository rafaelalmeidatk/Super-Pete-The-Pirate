using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate
{
    public enum ParticleType
    {
        Smoke,
        GroundPieces
    }

    public struct ParticleState
    {
        public float Gravity;
        public Vector2 Velocity;
        public float AlphaBase;
        public ParticleType Type;

        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle, GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var vel = particle.State.Velocity;

            particle.Sprite.Position += vel * elapsed;
            particle.Sprite.Position += particle.State.Gravity * Vector2.UnitY;
            
            float alpha = Math.Max(0, particle.PercentLife * 2 - particle.State.AlphaBase);
            alpha *= alpha;
            particle.Sprite.Alpha = alpha;

            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;
            else
                vel *= 0.97f;

            particle.State.Velocity = vel;
        }
    }
}
