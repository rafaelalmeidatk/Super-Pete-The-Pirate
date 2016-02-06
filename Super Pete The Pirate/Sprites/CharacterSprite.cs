using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;

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
        private Dictionary<string, List<Rectangle>> _framesList;
        public string CurrentFrameList { get { return _currentFrameList; } }


        //--------------------------------------------------
        // Animation delay

        private int _delay;
        private int _delayTick;

        //----------------------//------------------------//

        public CharacterSprite(Texture2D file) : base(file)
        {
            _currentFrame = 0;
            _currentFrameList = "stand";
            _delay = 100;
            _delayTick = 0;
            _framesList = new Dictionary<string, List<Rectangle>>();
            Origin = Vector2.Zero;
        }

        public void AddFrames(string name, List<Rectangle> frames)
        {
            _framesList[name] = frames;
        }

        public void SetFrameList(string name)
        {
            if (_currentFrameList != name)
            {
                _currentFrame = 0;
                _delayTick = 0;
                _currentFrameList = name;
            }
        }

        public void SetPosition(Vector2 position)
        {
            Position = new Vector2((int)position.X, (int)position.Y);
        }

        public void SetDirection(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Left)
                Effect = SpriteEffects.FlipHorizontally;
            else
                Effect = SpriteEffects.None;
        }

        public List<Rectangle> GetCurrentFramesList()
        {
            return _framesList[_currentFrameList];
        }

        public int GetFrameWidth()
        {
            return _framesList[_currentFrameList][_currentFrame].Width;
        }

        public int GetFrameHeight()
        {
            return _framesList[_currentFrameList][_currentFrame].Height;
        }

        public void Update(GameTime gameTime)
        {
            _delayTick += gameTime.ElapsedGameTime.Milliseconds;
            if (_delayTick > _delay)
            {
                _delayTick -= _delay;
                _currentFrame++;
                if (_currentFrame == GetCurrentFramesList().Count)
                    _currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(TextureRegion.Texture, position, _framesList[_currentFrameList][_currentFrame],
                Color, Rotation, Origin, Scale, Effect, 0);
        }
    }
}
