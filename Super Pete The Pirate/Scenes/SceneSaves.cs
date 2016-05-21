using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.BitmapFonts;
using Microsoft.Xna.Framework;
using Super_Pete_The_Pirate.Sprites;
using Microsoft.Xna.Framework.Input;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneSaves : SceneBase
    {
        //--------------------------------------------------
        // Strings

        private Dictionary<SceneManager.SceneSavesType, string> _titleStrings;

        //--------------------------------------------------
        // Textures

        private Texture2D _backgroundTexture;
        private Texture2D _peteSpritesheet;
        private Texture2D _stageSpritesheet;
        private Texture2D _iconsSpritesheet; // Lives, ammo, coins and HUD

        private AnimatedSprite _peteAnimatedSprite;
        private AnimatedSprite _nextStageMarkAnimatedSprite;

        private Rectangle _peteDefaultFrame;
        private Rectangle _stagePeteMarkFrame;
        private Rectangle _stageNextMarkFrame;
        private Rectangle _stageDivisorFrame;
        private Rectangle _heartFrame;
        private Rectangle _lifeFrame;
        private Rectangle _ammoFrame;
        private Rectangle _coinFrame;

        //--------------------------------------------------
        // Font color

        private Color _fontColor;

        //--------------------------------------------------
        // Slots

        private Rectangle[] _slotsPosition;
        private Texture2D _slotTexture;
        private int _slotIndex;

        //--------------------------------------------------
        // Positions

        private Vector2 _peteHeadPosition;
        private Vector2 _namePosition;
        private Vector2 _stagesPosition;
        private Vector2 _livesPosition;
        private Vector2 _heartsPosition;
        private Vector2 _ammoPosition;
        private Vector2 _ammoTextPosition;
        private Vector2 _coinsPosition;
        private Vector2 _coinsTextPosition;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            // Strings init

            _titleStrings = new Dictionary<SceneManager.SceneSavesType, string>()
            {
                { SceneManager.SceneSavesType.Save, "Save Game"},
                { SceneManager.SceneSavesType.Load, "Load Game" },
                { SceneManager.SceneSavesType.NewGame, "New Game" }
            };

            // Textures init

            _backgroundTexture = ImageManager.loadScene("saves", "SceneSavesBackground");
            _peteSpritesheet = ImageManager.loadScene("saves", "PeteSpritesheet");
            _stageSpritesheet = ImageManager.loadScene("saves", "StageSelectionSpritesheet");
            _iconsSpritesheet = ImageManager.loadSystem("IconsSpritesheet");

            var peteFrames = new Rectangle[]
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32),
                new Rectangle(96, 0, 32, 32),
                new Rectangle(128, 0, 32, 32),
                new Rectangle(160, 0, 32, 32),
                new Rectangle(192, 0, 32, 32),
                new Rectangle(224, 0, 32, 32)
            };
            _peteAnimatedSprite = new AnimatedSprite(_peteSpritesheet, peteFrames, 100, _peteHeadPosition);
            var nextStageMarkFrames = new Rectangle[]
            {
                new Rectangle(0, 0, 19, 19),
                new Rectangle(19, 0, 19, 19),
                new Rectangle(38, 0, 19, 19),
                new Rectangle(57, 0, 19, 19)
            };
            _nextStageMarkAnimatedSprite = new AnimatedSprite(_stageSpritesheet, nextStageMarkFrames, 130, _stagesPosition);

            _peteDefaultFrame = new Rectangle(0, 0, 32, 32);
            _stagePeteMarkFrame = new Rectangle(76, 0, 19, 19);
            _stageNextMarkFrame = new Rectangle(57, 0, 19, 19);
            _stageDivisorFrame = new Rectangle(95, 9, 2, 1);
            _heartFrame = new Rectangle(0, 0, 11, 10);
            _lifeFrame = new Rectangle(0, 10, 13, 14);
            _ammoFrame = new Rectangle(0, 24, 20, 9);
            _coinFrame = new Rectangle(0, 33, 20, 14);

            // Font color init

            _fontColor = new Color(31, 29, 28);

            // Slots init

            _slotsPosition = new Rectangle[]
            {
                new Rectangle(41, 46, 279, 45),
                new Rectangle(41, 109, 279, 45),
                new Rectangle(41, 169, 279, 45)
            };

            _slotIndex = 0;

            // Positions init

            _peteHeadPosition = new Vector2(0, 5);
            _namePosition = new Vector2(40, 1);
            _stagesPosition = new Vector2(39, 18);
            _livesPosition = new Vector2(3, 2);
            _heartsPosition = new Vector2(3, 17);
            _ammoPosition = new Vector2(184, 33);
            _ammoTextPosition = new Vector2(206, 30);
            _coinsPosition = new Vector2(228, 28);
            _coinsTextPosition = new Vector2(250, 30);

            // Slot texture (temporary)

            _slotTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _slotTexture.SetData<Color>(new Color[] { Color.Orange });
        }

        public override void Update(GameTime gameTime)
        {
            if (InputManager.Instace.KeyPressed(Keys.Down))
            {
                _slotIndex = _slotIndex >= 2 ? 0 : _slotIndex + 1;
            }
            if (InputManager.Instace.KeyPressed(Keys.Up))
            {
                _slotIndex = _slotIndex <= 0 ? 2 : _slotIndex - 1;
            }
            _peteAnimatedSprite.Update(gameTime);
            _nextStageMarkAnimatedSprite.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewportAdapter.GetScaleMatrix());

            // Background
            spriteBatch.Draw(_backgroundTexture, _backgroundTexture.Bounds, Color.White);

            // Title
            var titleString = _titleStrings[SceneManager.Instance.TypeOfSceneSaves];
            var titleX = (SceneManager.Instance.VirtualSize.X - SceneManager.Instance.GameFontBig.MeasureString(titleString).X) / 2;
            spriteBatch.DrawString(SceneManager.Instance.GameFontBig, titleString, new Vector2(titleX, 10), _fontColor);

            // Data just for simulation
            var stagesCompleted = new int[] { 1, 3, 0 };
            var lives = new int[] { 3, 5, 7 };
            var hearts = new int[] { 2, 3, 5 };
            var ammo = new int[] { 7, 13, 2 };
            var coins = new int[] { 25, 9, 16 };

            // Slots
            for (var i = 0; i < 3; i++)
            {
                var slotPosition = _slotsPosition[i].Location.ToVector2();
                // Background
                spriteBatch.Draw(_slotTexture, _slotsPosition[i], Color.White * 0.5f);
                // Pete Head
                if (_slotIndex == i)
                {
                    _peteAnimatedSprite.Position = slotPosition + _peteHeadPosition;
                    _peteAnimatedSprite.Draw(spriteBatch);
                }
                else
                {
                    spriteBatch.Draw(_peteSpritesheet, slotPosition + _peteHeadPosition, _peteDefaultFrame, Color.White);
                }
                // Save Name
                spriteBatch.DrawString(SceneManager.Instance.GameFont, "SAVE NAME", slotPosition + _namePosition, _fontColor);
                // Stages
                var divisorPosition = Vector2.Zero;
                for (var j = 0; j < stagesCompleted[i]; j++)
                {
                    var markPosition = slotPosition + _stagesPosition + ((_stagePeteMarkFrame.Width + 4) * j * Vector2.UnitX);
                    spriteBatch.Draw(_stageSpritesheet, markPosition, _stagePeteMarkFrame, Color.White);
                    divisorPosition = markPosition + (_stagePeteMarkFrame.Width + 1) * Vector2.UnitX + (9 * Vector2.UnitY);
                    spriteBatch.Draw(_stageSpritesheet, divisorPosition, _stageDivisorFrame, Color.White);
                }
                var nextMarkPos = (divisorPosition == Vector2.Zero) ? (slotPosition + _stagesPosition) : (divisorPosition - (9 * Vector2.UnitY) + (3 * Vector2.UnitX));
                if (_slotIndex == i)
                {
                    _nextStageMarkAnimatedSprite.Position = nextMarkPos;
                    _nextStageMarkAnimatedSprite.Draw(spriteBatch);
                }
                else
                {
                    spriteBatch.Draw(_stageSpritesheet, nextMarkPos, _stageNextMarkFrame, Color.White);
                }
                // Lives
                var livesWidth = (lives[i] * _lifeFrame.Width) + (lives[i] - 1);
                var livesPosition = slotPosition + (_slotsPosition[i].Width - livesWidth - _livesPosition.X) * Vector2.UnitX + _livesPosition.Y * Vector2.UnitY;
                for (var j = 0; j < lives[i]; j++)
                {
                    var lifePosition = livesPosition + ((_lifeFrame.Width + 1) * j * Vector2.UnitX);
                    spriteBatch.Draw(_iconsSpritesheet, lifePosition, _lifeFrame, Color.White);
                }
                // Hearts
                var heartsWidth = (hearts[i] * _heartFrame.Width) + (hearts[i] - 1) * 5;
                var heartsPosition = slotPosition + (_slotsPosition[i].Width - heartsWidth - _heartsPosition.X) * Vector2.UnitX + _heartsPosition.Y * Vector2.UnitY;
                for (var j = 0; j < hearts[i]; j++)
                {
                    var heartPosition = heartsPosition + ((_heartFrame.Width + 5) * j * Vector2.UnitX);
                    spriteBatch.Draw(_iconsSpritesheet, heartPosition, _heartFrame, Color.White);
                }
                // Ammo
                spriteBatch.Draw(_iconsSpritesheet, slotPosition + _ammoPosition, _ammoFrame, Color.White);
                spriteBatch.DrawString(SceneManager.Instance.GameFont, ammo[i].ToString(), slotPosition + _ammoTextPosition, _fontColor);
                // Coins
                spriteBatch.Draw(_iconsSpritesheet, slotPosition + _coinsPosition, _coinFrame, Color.White);
                spriteBatch.DrawString(SceneManager.Instance.GameFont, coins[i].ToString(), slotPosition + _coinsTextPosition, _fontColor);
            }

            spriteBatch.End();
        }
    }
}
