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

    public class CharacterSprite : Sprite
    {
        //--------------------------------------------------
        // Frames stuff

        private int _currentFrame;
        private string _currentFrameList;
        private Dictionary<string, FramesList> _framesList;
        public int CurrentFrame { get { return _currentFrame; } }
        public string CurrentFrameList { get { return _currentFrameList; } }

        private bool _looped;
        public bool Looped { get { return _looped; } }
        
        //--------------------------------------------------
        // Animation delay
        
        private int _delayTick;

        //--------------------------------------------------
        // Battle System visual stuff

        private int _immunityTick;
        private float _immunityTimeElapsed;
        private float _immunityMaxTime;
        private bool _immunityAnimation;
        private float _immunityAlphaStore;

        private bool _dyingAnimation;
        private bool _skipDyingAnimationFrames;
        private bool _dyingAnimationEnded;
        public bool DyingAnimationEnded { get { return _dyingAnimationEnded; } }

        //--------------------------------------------------
        // Collider

        public SpriteCollider Collider
        {
            get { return GetCurrentFramesList().Collider; }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return new Rectangle((int)Position.X, (int)Position.Y, GetCurrentFrameRectangle().Width, GetCurrentFrameRectangle().Height);
            }
        }

        //----------------------//------------------------//

        public CharacterSprite(Texture2D file) : base(file)
        {
            _currentFrame = 0;
            _currentFrameList = "stand";
            _delayTick = 0;
            _framesList = new Dictionary<string, FramesList>();
            _looped = false;
            
            _immunityMaxTime = 0.5f;
            _immunityTick = 0;
            _immunityTimeElapsed = 0;
            _immunityAnimation = false;
            _immunityAlphaStore = Alpha;

            _dyingAnimation = false;
            _dyingAnimationEnded = false;

            Origin = Vector2.Zero;
        }

        public void CreateFrameList(string name, int delay)
        {
            _framesList[name] = new FramesList(delay);
            _framesList[name].Colliders = new List<SpriteCollider>();
            _framesList[name].FramesToAttack = new List<int>();
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

        public void AddAttackCollider(string name, Rectangle rectangle, int attackWidth)
        {
            var collider = new SpriteCollider(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            collider.Type = SpriteCollider.ColliderType.Attack;
            collider.AttackWidth = attackWidth;
            _framesList[name].Colliders.Add(collider);
        }

        public void AddFramesToAttack(string name, params int[] frames)
        {
            for (var i = 0; i < frames.Length; i++)
                _framesList[name].FramesToAttack.Add(frames[i]);
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

        public void SetIfFrameListExists(string name)
        {
            if (_framesList.ContainsKey(name))
                SetFrameList(name);
        }

        public void SetPosition(Vector2 position)
        {
            Position = new Vector2((int)position.X, (int)position.Y);
            foreach (var collider in GetCurrentFramesList().Colliders)
            {
                var offsetX = 0f;
                if (Effect == SpriteEffects.FlipHorizontally && collider.Type == SpriteCollider.ColliderType.Attack)
                    offsetX = (collider.AttackWidth - GetBlockCollider().Width) + collider.OffsetX - (collider.AttackWidth - (collider.OffsetX + collider.Width));
                collider.Position = new Vector2(Position.X - offsetX, position.Y);
            }
        }

        public void SetDirection(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Left)
                Effect = SpriteEffects.FlipHorizontally;
            else
                Effect = SpriteEffects.None;
        }

        public void RequestImmunityAnimation()
        {
            _immunityAnimation = true;
            _immunityTick = 0;
            _immunityTimeElapsed = 0;
        }

        public void RequestDyingAnimation()
        {
            _dyingAnimation = true;
            if (!_framesList.ContainsKey("dying"))
                _skipDyingAnimationFrames = true;
        }

        public Rectangle GetCurrentFrameRectangle()
        {
            return GetCurrentFramesList().Frames[_currentFrame];
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
            if (_immunityAnimation)
                UpdateImmunityAnimation(gameTime);

            if (_dyingAnimation)
                UpdateDying(gameTime);

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

        private void UpdateImmunityAnimation(GameTime gameTime)
        {
            Alpha = _immunityTick == 0 ? 1f : 0.2f;
            _immunityTimeElapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_immunityTimeElapsed > _immunityMaxTime)
            {
                _immunityAnimation = false;
                _immunityTick = 0;
                _immunityTimeElapsed = 0f;
                Alpha = _immunityAlphaStore;
            }
            else
            {
                _immunityTick = _immunityTick == 0 ? 1 : 0;
            }
        }

        public void UpdateDying(GameTime gameTime)
        {
            if ((_framesList.ContainsKey("dying") && _currentFrameList == "dying" && _looped) || _skipDyingAnimationFrames)
            {
                Alpha -= 0.05f;
                if (Alpha <= 0f)
                {
                    _dyingAnimationEnded = true;
                    _dyingAnimation = false;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (Effect == SpriteEffects.FlipHorizontally && GetBlockCollider().Width < GetCurrentFramesList().Frames[_currentFrame].Width)
            {
                var offsetX = GetCurrentFramesList().Frames[_currentFrame].Width - GetBlockCollider().Width;
                position.X -= offsetX;
            }
            spriteBatch.Draw(TextureRegion.Texture, position, _framesList[_currentFrameList].Frames[_currentFrame],
                Color * Alpha, Rotation, Origin, Scale, Effect, 0);
        }
    }
}
