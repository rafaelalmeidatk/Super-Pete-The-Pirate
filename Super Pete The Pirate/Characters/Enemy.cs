using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;
using System.Diagnostics;

namespace Super_Pete_The_Pirate.Characters
{
    //--------------------------------------------------
    // Enemy Type
    public enum EnemyType
    {
        None,
        SniperPig
    }

    class Enemy : CharacterBase
    {
        //--------------------------------------------------
        // Combat system

        protected EnemyType _enemyType;
        public EnemyType EnemyType { get { return _enemyType; } }

        protected Rectangle _viewRange;
        public Rectangle ViewRange { get { return _viewRange; } }
        protected Vector2 _viewRangeSize;
        private Vector2 _lastPosition;

        public float _viewRangeCooldown;
        public float ViewRangeCooldown { get { return _viewRangeCooldown; } }

        protected int _damage;

        //--------------------------------------------------
        // Textures

        private Texture2D _viewRangeTexture;

        //----------------------//------------------------//

        public Enemy(Texture2D texture) : base(texture) {
            _viewRangeTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _viewRangeTexture.SetData<Color>(new Color[] { Color.Green });
            _lastPosition = Position;
            _enemyType = EnemyType.None;
            _viewRangeCooldown = 0f;
            _damage = 0;
        }

        public void CreateViewRange()
        {
            var width = ((int)_viewRangeSize.X + CharacterSprite.Collider.BoundingBox.Width / 2) * 2;
            var height = (int)_viewRangeSize.Y;
            _viewRange = new Rectangle(0, 0, width, height);
        }

        public virtual void PlayerOnSight(Vector2 playerPosition) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_lastPosition.X != CharacterSprite.Collider.BoundingBox.X ||
                _lastPosition.Y != CharacterSprite.Collider.BoundingBox.Y)
                UpdateViewRange();

            UpdateViewRangeCooldown(gameTime);
        }

        private void UpdateViewRangeCooldown(GameTime gameTime)
        {
            if (_viewRangeCooldown > 0f)
                _viewRangeCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
        }

        public void UpdateViewRange()
        {
            _viewRange.X = CharacterSprite.Collider.BoundingBox.Center.X - _viewRange.Width / 2;
            _viewRange.Y = CharacterSprite.Collider.BoundingBox.Y;
            _lastPosition.X = CharacterSprite.Collider.BoundingBox.X;
            _lastPosition.Y = CharacterSprite.Collider.BoundingBox.Y;
        }

        public override void DrawColliderBox(SpriteBatch spriteBatch)
        {
            base.DrawColliderBox(spriteBatch);
            DrawViewRange(spriteBatch);
        }

        private void DrawViewRange(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_viewRangeTexture, _viewRange, Color.White * 0.2f);
        }
    }
}
