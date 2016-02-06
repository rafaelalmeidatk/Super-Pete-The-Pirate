using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using Super_Pete_The_Pirate.Sprites;

namespace Super_Pete_The_Pirate
{
    //--------------------------------------------------
    // Sprite direction

    public enum SpriteDirection
    {
        Left,
        Right
    }

    //----------------------//------------------------//

    class CharacterSprite : Sprite
    {
        //--------------------------------------------------
        // Frames stuff

        private int _currentFrame;
        private string _currentFrameList;
        private Dictionary<string, FramesList> _framesList;
        public string CurrentFrameList { get { return _currentFrameList; } }

        private bool _looped;
        public bool Looped { get { return _looped; } }
        
        //--------------------------------------------------
        // Animation delay
        
        private int _delayTick;

        //--------------------------------------------------
        // Collider

        public SpriteCollider Collider
        {
            get { return GetCurrentFramesList().Collider; }
        }

        //----------------------//------------------------//

        public CharacterSprite(Texture2D file) : base(file)
        {
            _currentFrame = 0;
            _currentFrameList = "stand";
            _delayTick = 0;
            _framesList = new Dictionary<string, FramesList>();
            _looped = false;
            Origin = Vector2.Zero;
        }

        public void CreateFrameList(string name, int delay)
        {
            _framesList[name] = new FramesList(delay);
            _framesList[name].Colliders = new List<SpriteCollider>();
        }

        public void AddFrames(string name, List<Rectangle> frames)
        {
            _framesList[name].Frames = frames;
        }

        public void AddCollider(string name, Rectangle rectangle)
        {
            var collider = new SpriteCollider(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            _framesList[name].Colliders.Add(collider);
        }

        public void AddCollider(string name, Rectangle rectangle, SpriteCollider.ColliderType type)
        {
            var collider = new SpriteCollider(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            collider.Type = type;
            _framesList[name].Colliders.Add(collider);
        }

        public void SetFrameList(string name)
        {
            if (_currentFrameList != name)
            {
                _currentFrame = 0;
                _delayTick = 0;
                _currentFrameList = name;
                _looped = false;
            }
        }

        public void SetPosition(Vector2 position)
        {
            Position = new Vector2((int)position.X, (int)position.Y);
            foreach (var collider in GetCurrentFramesList().Colliders)
                collider.Position = position;
        }

        public void SetDirection(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Left)
                Effect = SpriteEffects.FlipHorizontally;
            else
                Effect = SpriteEffects.None;
        }

        public FramesList GetCurrentFramesList()
        {
            return _framesList[_currentFrameList];
        }

        public SpriteCollider GetBlockCollider()
        {
            return _framesList[_currentFrameList].Collider;
        }

        public int GetFrameWidth()
        {
            return _framesList[_currentFrameList].Frames[_currentFrame].Width;
        }

        public int GetFrameHeight()
        {
            return _framesList[_currentFrameList].Frames[_currentFrame].Height;
        }

        public int GetColliderWidth()
        {
            return _framesList[_currentFrameList].Collider.Width;
        }

        public int GetColliderHeight()
        {
            return _framesList[_currentFrameList].Collider.Height;
        }

        public bool LoopFinished()
        {
            return _currentFrame == GetCurrentFramesList().Frames.Count - 1;
        }

        public void Update(GameTime gameTime)
        {
            if (_framesList[_currentFrameList].Loop)
            {
                _delayTick += gameTime.ElapsedGameTime.Milliseconds;
                if (_delayTick > _framesList[_currentFrameList].Delay)
                {
                    _delayTick -= _framesList[_currentFrameList].Delay;
                    _currentFrame++;
                    if (_currentFrame == GetCurrentFramesList().Frames.Count)
                    {
                        _currentFrame = 0;
                        if (!_looped) _looped = true;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(TextureRegion.Texture, position, _framesList[_currentFrameList].Frames[_currentFrame],
                Color, Rotation, Origin, Scale, Effect, 0);
        }
    }
}
