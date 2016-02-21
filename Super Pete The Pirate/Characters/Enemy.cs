using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;
using Super_Pete_The_Pirate.Sprites;
using System.Diagnostics;
using Super_Pete_The_Pirate.Scenes;

namespace Super_Pete_The_Pirate.Characters
{
    //--------------------------------------------------
    // Enemy Type
    public enum EnemyType
    {
        None,
        SniperPig,
        TurtleWheel,
        Parrot
    }

    class Enemy : CharacterBase
    {
        //--------------------------------------------------
        // Combat system

        protected EnemyType _enemyType;
        public EnemyType EnemyType { get { return _enemyType; } }

        protected Rectangle _viewRange;
        public Rectangle ViewRange { get { return _viewRange; } }
        protected Vector2 _viewRangeOffset;
        protected Vector2 _viewRangeSize;
        private Vector2 _lastPosition;

        private bool _hasViewRange;
        public bool HasViewRange { get { return _hasViewRange; } }
        protected float _viewRangeCooldown;
        public float ViewRangeCooldown { get { return _viewRangeCooldown; } }

        //--------------------------------------------------
        // TurtleWheel use

        protected bool _wheelMode;
        public bool InWheelMode { get { return _wheelMode; } }

        protected int _damage;

        //--------------------------------------------------
        // Coin value

        protected int _coins;

        //--------------------------------------------------
        // Textures

        private Texture2D _viewRangeTexture;

        //----------------------//------------------------//

        public Enemy(Texture2D texture) : base(texture) {
            _viewRangeTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _viewRangeTexture.SetData<Color>(new Color[] { Color.Green });
            _lastPosition = Position;
            _enemyType = EnemyType.None;
            _hasViewRange = false;
            _viewRangeCooldown = 0f;
            _viewRangeOffset = Vector2.Zero;
            _damage = 0;
            _coins = 3;
        }

        public void CreateViewRange()
        {
            var width = ((int)_viewRangeSize.X + CharacterSprite.Collider.BoundingBox.Width / 2) * 2;
            var height = (int)_viewRangeSize.Y;
            _viewRange = new Rectangle(0, 0, width, height);
            _hasViewRange = true;
        }

        public virtual void PlayerOnSight(Vector2 playerPosition) { }

        public override void OnDie()
        {
            base.OnDie();
            var boundingBox = CharacterSprite.BoundingBox;
            var sceneMap = (SceneMap)SceneManager.Instance.GetCurrentScene();
            for (var i = 0; i < _coins; i++)
            {
                var ax = _rand.Next(0, 2) == 0 ? _rand.Next(-90 + (i % 5) * -40, -10) : _rand.Next(10, 90 + (i % 5) * 40);
                var vy = _rand.Next(-4500, -500);
                var coin = sceneMap.CreateCoin(boundingBox.Left, boundingBox.Bottom - boundingBox.Height, new Vector2(0, vy), true);
                coin.SetXAcceleration(ax);
            }
        }

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
            _viewRange.X = (CharacterSprite.Collider.BoundingBox.Center.X - _viewRange.Width / 2) + (int)_viewRangeOffset.X;
            _viewRange.Y = (CharacterSprite.Collider.BoundingBox.Y) + (int)_viewRangeOffset.Y;
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
