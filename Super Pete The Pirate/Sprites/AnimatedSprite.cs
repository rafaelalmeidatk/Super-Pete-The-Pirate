using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

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

        public AnimatedSprite(Texture2D texture, Rectangle[] frames, int delay, int x, int y) : base(texture)
        {
            _frames = frames;
            _delay = delay;
            Position = new Vector2(x, y);
            OriginNormalized = new Vector2(0, 0);

            _colliderTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _colliderTexture.SetData<Color>(new Color[] { Color.Orange });

            _boundingBox = Rectangle.Empty;
        }

        public void SetBoundingBox(Rectangle boundingBox)
        {
            _boundingBox = boundingBox;
        }

        public void Update(GameTime gameTime)
        {
            _delayTick += gameTime.ElapsedGameTime.Milliseconds;
            if (_delayTick > _delay)
            {
                _currentFrame++;
                _delayTick = 0;
                if (_currentFrame == _frames.Length)
                    _currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(TextureRegion.Texture, Position, _frames[_currentFrame], Color * Alpha, Rotation, Origin, Scale, Effect, 0);
        }

        public void DrawCollider(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_colliderTexture, BoundingBox, Color.White * 0.5f);
        }
    }
}
