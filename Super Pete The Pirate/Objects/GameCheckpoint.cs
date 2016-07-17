using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Objects
{
    class GameCheckpoint : AnimatedSprite
    {
        //--------------------------------------------------
        // Is checked

        public bool _isChecked;

        public bool IsChecked { get { return _isChecked; } }

        //--------------------------------------------------
        // Running up Frames

        private Rectangle[] _runningUpFrames;

        private bool _runningUp;

        //--------------------------------------------------
        // Checked Frames

        private Rectangle[] _checkedFrames;

        //--------------------------------------------------
        // End Flag

        private bool _endFlag;
        public bool IsEndFlag => _endFlag;

        //--------------------------------------------------
        // Random

        protected Random _rand;

        //----------------------//------------------------//

        public GameCheckpoint(Texture2D texture, Rectangle[] frames, int delay, int x, int y, bool endFlag)
            : base(texture, frames, delay, x, y, true)
        {
            // Mechanics init
            _isChecked = false;

            // Frames init
            _runningUpFrames = new Rectangle[]
            {
                new Rectangle(192, 0, 64, 96),
                new Rectangle(256, 0, 64, 96),
                new Rectangle(0, 96, 64, 96),
                new Rectangle(64, 96, 64, 96)
            };

            _checkedFrames = new Rectangle[]
            {
                new Rectangle(128, 96, 64, 96),
                new Rectangle(192, 96, 64, 96),
                new Rectangle(256, 96, 64, 96)
            };

            _runningUp = false;
            _endFlag = endFlag;

            _rand = new Random();
        }

        public void OnPlayerCheck()
        {
            _isChecked = true;
            SetNewFrames(_runningUpFrames, 50, false);
            _runningUp = true;
            CreateCheckedParticles();
        }

        public override void Update(GameTime gameTime)
        {
            if (_runningUp && Looped)
            {
                _runningUp = false;
                SetNewFrames(_checkedFrames, 130);
            }
                
            base.Update(gameTime);
        }

        private void CreateCheckedParticles()
        {
            var texture = ImageManager.loadParticle("WhitePoint");
            for (var i = 0; i < 20; i++)
            {
                var position = new Vector2(Position.X + 2, Position.Y + 96);
                var velocity = new Vector2(_rand.NextFloat(-100f, 100f), _rand.NextFloat(-500f, -300f));
                var color = ColorUtil.HSVToColor(MathHelper.ToRadians(_rand.NextFloat(0, 359)), 0.6f, 1f);
                var scale = _rand.Next(0, 2) == 0 ? new Vector2(2, 2) : new Vector2(3, 3);

                var state = new ParticleState()
                {
                    Velocity = velocity,
                    Type = ParticleType.Confetti,
                    Gravity = 1.8f,
                    UseCustomVelocity = true,
                    VelocityMultiplier = 0.95f
                };

                SceneManager.Instance.ParticleManager.CreateParticle(texture, position, color, 1000f, scale, state);
            }
        }
    }
}
