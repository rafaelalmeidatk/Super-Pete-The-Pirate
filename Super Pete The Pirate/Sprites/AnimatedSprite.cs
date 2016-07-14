using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using MonoGame.Extended.TextureAtlases;

namespace Super_Pete_The_Pirate.Sprites
{
    class AnimatedSprite : Sprite
    {
        //--------------------------------------------------
        // Frames stuff

        private int _currentFrame;
        private Rectangle[] _frames;
        private int _delay;
        private int _delayTick;
        private bool _looped;
        public bool Looped { get { return _looped; } }
        private bool _repeat;

        //--------------------------------------------------
        // Collider

        private Texture2D _colliderTexture;
        
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

            _colliderTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _colliderTexture.SetData<Color>(new Color[] { Color.Orange });

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

        public void SetBoundingBox(Rectangle boundingBox)
        {
            _boundingBox = boundingBox;
        }

        public virtual void Update(GameTime gameTime)
        {
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

        public void DrawCollider(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_colliderTexture, BoundingBox, Color.White * 0.5f);
        }

        public AnimatedSprite Clone()
        {
            return (AnimatedSprite)MemberwiseClone();
        }
    }
}
