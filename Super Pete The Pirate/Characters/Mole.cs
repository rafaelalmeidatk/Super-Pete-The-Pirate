using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Characters
{
    class Mole : Enemy
    {
        //--------------------------------------------------
        // Mechanics

        private bool _onHole;
        private bool _stuckOnGroud;
        private Vector2 _holePoint;

        private float _stuckCounter;
        private const float StuckTime = 3000f;


        new const float GravityAcceleration = 1850.0f;

        //--------------------------------------------------
        // Hole Point Texture

        private Texture2D _holeTexture;

        //----------------------//------------------------//

        public Mole(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.Mole;

            // Digging
            CharacterSprite.CreateFrameList("stand", 150);
            CharacterSprite.AddCollider("stand", new Rectangle(5, -7, 22, 39));
            CharacterSprite.AddFrames("stand", new List<Rectangle>()
            {
                new Rectangle(0, 64, 32, 64),
                new Rectangle(32, 64, 32, 64),
                new Rectangle(64, 64, 32, 64),
                new Rectangle(96, 64, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Digging
            CharacterSprite.CreateFrameList("digging", 150);
            CharacterSprite.AddCollider("digging", new Rectangle(5, -7, 22, 39));
            CharacterSprite.AddFrames("digging", new List<Rectangle>()
            {
                new Rectangle(0, 64, 32, 64),
                new Rectangle(32, 64, 32, 64),
                new Rectangle(64, 64, 32, 64),
                new Rectangle(96, 64, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Jump Up
            CharacterSprite.CreateFrameList("jump_up", 0);
            CharacterSprite.AddCollider("jump_up", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("jump_up", new List<Rectangle>()
            {
                new Rectangle(0, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Jump Zero
            CharacterSprite.CreateFrameList("jump_zero", 0);
            CharacterSprite.AddCollider("jump_zero", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("jump_zero", new List<Rectangle>()
            {
                new Rectangle(32, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Jump Down
            CharacterSprite.CreateFrameList("jump_down", 0);
            CharacterSprite.AddCollider("jump_down", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("jump_down", new List<Rectangle>()
            {
                new Rectangle(64, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Dying
            CharacterSprite.CreateFrameList("dying", 0);
            CharacterSprite.AddCollider("dying", new Rectangle(2, 8, 29, 17));
            CharacterSprite.AddFrames("dying", new List<Rectangle>()
            {
                new Rectangle(96, 0, 32, 64)
            }, new int[] { 0, 0, 0, 0 }, new int[] { -32, -32, -32, -32 });

            // Combat system init
            _hp = 2;
            _damage = 1;
            _viewRangeSize = new Vector2(5, 100);
            _viewRangeOffset = new Vector2(0, -60);

            // Mechanics init
            _onHole = true;
            _stuckOnGroud = false;
            _holePoint = Vector2.Zero;

            // Texture init
            _holeTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _holeTexture.SetData<Color>(new Color[] { Color.Blue });

            CreateViewRange();
        }

        public void SetHolePoint(int x, int y)
        {
            _holePoint = new Vector2(x, y);
        }

        public override void PlayerOnSight(Vector2 playerPosition)
        {
            if (_onHole)
            {
                _velocity.Y = -150000f;
                _onHole = false;
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void OnGroundLand()
        {
            base.OnGroundLand();
            _stuckOnGroud = true;
            _stuckCounter = StuckTime;
        }

        public override void UpdateFrameList()
        {
            CharacterSprite.IsVisible = !_onHole;
            if (_onHole)
            {
                CharacterSprite.SetFrameList("digging");
            }
            else
            {
                var frameList = _velocity.Y == 0 ? "jump_zero" : _velocity.Y > 0 ? "jump_down" : "jump_up";
                CharacterSprite.SetFrameList(frameList);
            }
        }

        public override void DrawColliderBox(SpriteBatch spriteBatch)
        {
            base.DrawColliderBox(spriteBatch);
            DrawHolePoint(spriteBatch);
        }

        public void DrawHolePoint(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_holeTexture, new Rectangle((int)_holePoint.X, (int)_holePoint.Y, 32, 32), Color.White * 0.5f);
        }
    }
}
