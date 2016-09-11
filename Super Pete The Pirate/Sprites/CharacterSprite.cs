using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Sprites;
using Super_Pete_The_Pirate.Sprites;
using System.Collections.Generic;

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
        public bool ImmunityAnimationActive { get { return _immunityAnimation; } }

        private bool _dyingAnimation;
        private bool _skipDyingAnimationFrames;
        private bool _dyingAnimationEnded;
        public bool DyingAnimationEnded { get { return _dyingAnimationEnded; } }

        //--------------------------------------------------
        // Collider

        private Texture2D _colliderRedTexture;
        private Texture2D _colliderYellowTexture;

        public SpriteCollider Collider
        {
            get { return GetCurrentFramesList().Collider; }
        }

        //--------------------------------------------------
        // Bouding Box

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

            _colliderRedTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _colliderRedTexture.SetData<Color>(new Color[] { Color.Red });

            _colliderYellowTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _colliderYellowTexture.SetData<Color>(new Color[] { Color.Yellow });
        }

        public void CreateFrameList(string name, int delay)
        {
            _framesList[name] = new FramesList(delay);
        }

        public void CreateFrameList(string name, int delay, bool reset)
        {
            _framesList[name] = new FramesList(delay);
            _framesList[name].Reset = reset;
        }

        public void AddFrames(string name, List<Rectangle> frames, int[] offsetX, int[] offsetY)
        {
            for (var i = 0; i < frames.Count; i++)
            {
                _framesList[name].Frames.Add(new FrameInfo(frames[i], offsetX[i], offsetY[i]));
            }
        }

        public void AddCollider(string name, Rectangle rectangle)
        {
            var collider = new SpriteCollider(rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            collider.Type = SpriteCollider.ColliderType.Block;
            _framesList[name].Collider = collider;
        }

        public void AddAttackCollider(string name, List<List<Rectangle>> rectangleFrames, int attackWidth)
        {
            for (var i = 0; i < rectangleFrames.Count; i++)
            {
                for (var j = 0; j < rectangleFrames[i].Count; j++)
                {
                    var collider = new SpriteCollider(rectangleFrames[i][j].X, rectangleFrames[i][j].Y, rectangleFrames[i][j].Width, rectangleFrames[i][j].Height);
                    collider.Type = SpriteCollider.ColliderType.Attack;
                    collider.AttackWidth = attackWidth;
                    _framesList[name].Frames[i].AttackColliders.Add(collider);
                }
            }
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
                if (!_framesList[_currentFrameList].Reset)
                {
                    _framesList[_currentFrameList].Loop = true;
                }
            }
        }

        public void SetFrameListOnly(string name)
        {
            if (_currentFrameList != name)
            {
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

            for (var i = 0; i < GetCurrentFramesList().Frames.Count; i++)
            {
                for (var j = 0; j < GetCurrentFramesList().Frames[i].AttackColliders.Count; j++)
                {
                    var collider = GetCurrentFramesList().Frames[i].AttackColliders[j];
                    var offsetX = 0;
                    if (Effect == SpriteEffects.FlipHorizontally)
                        offsetX = 2 * (collider.OffsetX - GetBlockCollider().OffsetX) - GetBlockCollider().Width + collider.Width;
                    collider.Position = new Vector2(position.X - offsetX, position.Y);
                }
            }

            GetCurrentFramesList().Collider.Position = position;
        }

        public void SetDirection(SpriteDirection direction)
        {
            if (direction == SpriteDirection.Left)
                Effect = SpriteEffects.FlipHorizontally;
            else
                Effect = SpriteEffects.None;
        }

        public void RemoveImmunity()
        {
            _immunityAnimation = false;
            _immunityTick = 0;
            _immunityTimeElapsed = 0;
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
            return GetCurrentFramesList().Frames[_currentFrame].SpriteSheetInfo;
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
            return GetCurrentFrameRectangle().Width;
        }

        public int GetFrameHeight()
        {
            return GetCurrentFrameRectangle().Height;
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
                        if (!_framesList[_currentFrameList].Reset)
                        {
                            _currentFrame--;
                            _framesList[_currentFrameList].Loop = false;
                        }
                        else _currentFrame = 0;
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

        private Texture2D GetColliderTexture(SpriteCollider collider)
        {
            return collider.Type == SpriteCollider.ColliderType.Block ? _colliderRedTexture : _colliderYellowTexture;
        }

        public void DrawColliders(SpriteBatch spriteBatch)
        {
            var blockColider = GetCurrentFramesList().Collider;
            spriteBatch.Draw(GetColliderTexture(blockColider), blockColider.BoundingBox, Color.White * 0.5f);

            foreach (var collider in GetCurrentFramesList().Frames[CurrentFrame].AttackColliders)
                spriteBatch.Draw(GetColliderTexture(collider), collider.BoundingBox, Color.White * 0.5f);
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            if (!IsVisible) return;

            if (Effect == SpriteEffects.FlipHorizontally)
                position.X -= GetCurrentFrameRectangle().Width - (GetBlockCollider().OffsetX + GetBlockCollider().Width) + GetCurrentFramesList().Frames[_currentFrame].OffsetX;
            else
                position.X -= GetBlockCollider().OffsetX - GetCurrentFramesList().Frames[_currentFrame].OffsetX;

            position.Y -= GetBlockCollider().OffsetY - GetCurrentFramesList().Frames[_currentFrame].OffsetY;
            spriteBatch.Draw(TextureRegion.Texture, position, GetCurrentFrameRectangle(),
                Color * Alpha, Rotation, Origin, Scale, Effect, 0);
        }

        public CharacterSprite Clone()
        {
            return (CharacterSprite)MemberwiseClone();
        }
    }
}
