using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using System;

namespace Super_Pete_The_Pirate
{
    public class ParticleManager<T>
    {
        public class Particle
        {
            public Sprite Sprite;
            public float Duration;
            public float PercentLife = 1f;
            public T State;
            public SpriteEffects SpriteEffects;

            public Rectangle BoundingRectangle
            {
                get
                {
                    return new Rectangle((int)Sprite.Position.X, (int)Sprite.Position.Y, Sprite.TextureRegion.Width, Sprite.TextureRegion.Height);
                }
            }
        }

        private class CircularParticleArray
        {
            private int _start;
            public int Start
            {
                get { return _start; }
                set { _start = value % _list.Length; }
            }

            public int Count { get; set; }
            public int Capacity { get { return _list.Length; } }
            private Particle[] _list;

            public CircularParticleArray(int capacity)
            {
                _list = new Particle[capacity];
            }

            public Particle this[int i]
            {
                get { return _list[(_start + i) % _list.Length]; }
                set { _list[(_start + i) % _list.Length] = value; }
            }
        }

        private Action<Particle, GameTime> updateParticle;
        private CircularParticleArray particleList;

        public ParticleManager(int capacity, Action<Particle, GameTime> updateParticle)
        {
            this.updateParticle = updateParticle;
            particleList = new CircularParticleArray(capacity);

            for (var i = 0; i < capacity; i++)
                particleList[i] = new Particle();
        }

        public void CreateParticle(Texture2D texture, Vector2 position, Color color, float duration, Vector2 scale, T state, SpriteEffects effects = SpriteEffects.None)
        {
            Particle particle;
            if (particleList.Count == particleList.Capacity)
            {
                particle = particleList[0];
                particleList.Start++;
            }
            else
            {
                particle = particleList[particleList.Count];
                particleList.Count++;
            }

            particle.Sprite = new Sprite(texture);
            particle.Sprite.Position = position;
            particle.Sprite.Color = color;
            particle.Sprite.Scale = scale;
            particle.Sprite.Effect = effects;

            particle.Duration = duration;
            particle.PercentLife = 1f;
            particle.State = state;
        }

        public void Update(GameTime gameTime)
        {
            int removalCount = 0;
            for (int i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];

                updateParticle(particle, gameTime);

                particle.PercentLife -= (float)gameTime.ElapsedGameTime.TotalMilliseconds / particle.Duration;
                
                Swap(particleList, i - removalCount, i);
                
                if (particle.PercentLife < 0f)
                    removalCount++;
            }
            particleList.Count -= removalCount;
        }

        private static void Swap(CircularParticleArray list, int index1, int index2)
        {
            var temp = list[index1];
            list[index1] = list[index2];
            list[index2] = temp;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var i = 0; i < particleList.Count; i++)
            {
                var particle = particleList[i];
                spriteBatch.Draw(particle.Sprite);
            }
        }
    }
}
