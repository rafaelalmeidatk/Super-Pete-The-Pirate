using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate
{
    public enum ParticleType
    {
        Smoke,
        GroundPieces,
        Spark,
        Confetti
    }

    public struct ParticleState
    {
        //--------------------------------------------------
        // Physics variables

        public float Gravity;
        public Vector2 Velocity;
        public float VelocityMultiplier;
        public bool UseCustomVelocity;

        //--------------------------------------------------
        // Particle variables

        public float AlphaBase;
        public ParticleType Type;
        public int Width;

        // H amount of HSV
        public float H;

        public float Rotation;

        //----------------------//------------------------//

        public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle, GameTime gameTime)
        {
            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            var vel = particle.State.Velocity;

            particle.Sprite.Position += vel * elapsed;
            particle.Sprite.Position += particle.State.Gravity * Vector2.UnitY;

            float alpha = Math.Max(0, particle.PercentLife * 2 - particle.State.AlphaBase);
            alpha *= alpha;
            if (particle.State.Type == ParticleType.Spark)
            {
                particle.State.H -= 1.4f;
                particle.Sprite.Color = ColorUtil.HSVToColor(MathHelper.ToRadians(particle.State.H), 0.5f, 0.91f);
                particle.Sprite.Rotation = vel.ToAngle();
                var x = Math.Min(Math.Min(particle.State.Width, 0.2f * vel.Length() + 0.1f), alpha);
                particle.Sprite.Scale = new Vector2(x, particle.Sprite.Scale.Y);
            }
            else if (particle.State.Type == ParticleType.Confetti)
            {
                particle.Sprite.Rotation = vel.ToAngle() + particle.State.Rotation;
            }
            else
            {
                particle.Sprite.Alpha = alpha;
            }

            if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
                vel = Vector2.Zero;
            if (particle.State.UseCustomVelocity)
                vel *= particle.State.VelocityMultiplier;
            else
                vel *= 0.97f;

            particle.State.Rotation += 0.08f;
            particle.State.Velocity = vel;
        }
    }
}
