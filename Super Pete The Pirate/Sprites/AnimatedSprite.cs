using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;
using System;

namespace Super_Pete_The_Pirate.Sprites
{
    class AnimatedSprite : Sprite
    {
        //--------------------------------------------------
        // Frames stuff

        private int _currentFrame;
        public int CurrentFrame => _currentFrame;
        private Rectangle[] _frames;
        private int _delay;
        private int _delayTick;
        private bool _looped;
        public bool Looped => _looped;
        private bool _repeat;
        private bool _paused;
        
        //--------------------------------------------------
        // Bouding Box

        private Rectangle _boundingBox;
        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X + _boundingBox.X, (int)Position.Y + _boundingBox.Y, _boundingBox.Width, _boundingBox.Height);
            }
        }

        //----------------------//------------------------//

        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int delay, Vector2 position, bool repeat = true)
            : this(texture, frames, delay, (int)position.X, (int)position.Y, repeat) { }
         
        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int delay, int x, int y, bool repeat = true) : base(texture)
        {
            _frames = frames;
            _delay = delay;
            _looped = false;
            _repeat = repeat;

            Position = new Vector2(x, y);
            OriginNormalized = new Vector2(0, 0);

            _boundingBox = Rectangle.Empty;
        }

        public void SetTexture(Texture2D texture, bool repeat = true)
        {
            TextureRegion = new TextureRegion2D(texture);
            _currentFrame = 0;
            _repeat = repeat;
            _looped = false;
        }

        public void SetNewFrames(Rectangle[] frames, int delay, bool repeat = true)
        {
            _currentFrame = 0;
            _repeat = repeat;
            _delay = delay;
            _delayTick = 0;
            _looped = false;
            _frames = frames;
        }

        public void SetDelay(int delay)
        {
            _delay = delay;
        }

        public void Pause()
        {
            _paused = true;
        }

        public void SetBoundingBox(Rectangle boundingBox)
        {
            _boundingBox = boundingBox;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_paused) return;
            _delayTick += gameTime.ElapsedGameTime.Milliseconds;
            if (_delayTick > _delay)
            {
                _currentFrame++;
                _delayTick = 0;
                if (_currentFrame == _frames.Length)
                {
                    _looped = true;
                    if (_repeat)
                        _currentFrame = 0;
                    else
                        _currentFrame = _frames.Length - 1;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible)
            {
                spriteBatch.Draw(TextureRegion.Texture, Position, _frames[_currentFrame], Color * Alpha, Rotation, Origin, Scale, Effect, 0);
            }
        }

        public AnimatedSprite Clone()
        {
            return (AnimatedSprite)MemberwiseClone();
        }
    }
}
