using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.BitmapFonts;
using Microsoft.Xna.Framework;
using Super_Pete_The_Pirate.Sprites;
using Microsoft.Xna.Framework.Input;
using Super_Pete_The_Pirate.Managers;
using System.Diagnostics;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneSaves : SceneBase
    {
        //--------------------------------------------------
        // Strings

        private Dictionary<SceneManager.SceneSavesType, string> _titleStrings;
        private string _emptySlotText;

        //--------------------------------------------------
        // Textures

        private Texture2D _backgroundTexture;
        private Texture2D _loadingBackgroundTexture;
        private Texture2D _peteSpritesheet;
        private Texture2D _stageSpritesheet;
        private Texture2D _iconsSpritesheet; // Lives, ammo, hearts and coins

        private AnimatedSprite _loadingAnimatedSprite;
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

        //--------------------------------------------------
        // Save system

        private SavesManager.GameSave[] _gameSaves;
        private bool _loadingVisible;
        private int _loadResponses;

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

            _emptySlotText = "Empty";

            // Textures init

            _loadingBackgroundTexture = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _loadingBackgroundTexture.SetData<Color>(new Color[] { Color.Black });

            _backgroundTexture = ImageManager.loadScene("saves", "SceneSavesBackground");
            _peteSpritesheet = ImageManager.loadScene("saves", "PeteSpritesheet");
            _stageSpritesheet = ImageManager.loadScene("saves", "StageSelectionSpritesheet");
            _iconsSpritesheet = ImageManager.loadSystem("IconsSpritesheet");

            var loadingSpritesheet = ImageManager.loadScene("saves", "SavesLoadingSpritesheet");
            var loadingFrames = new Rectangle[]
            {
                new Rectangle(0, 0, 32, 32),
                new Rectangle(32, 0, 32, 32),
                new Rectangle(64, 0, 32, 32)
            };
            _loadingAnimatedSprite = new AnimatedSprite(loadingSpritesheet, loadingFrames, 200,
                new Vector2(SceneManager.Instance.VirtualSize.X - 35, SceneManager.Instance.VirtualSize.Y - 36));

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

            // General variables
            
            _loadingVisible = true;
            _loadResponses = 0;

            ReadSaves();
        }

        private void ReadSaves()
        {
            _gameSaves = new SavesManager.GameSave[3];
            SavesManager.Instance.ExecuteLoad(0, AfterLoad);
        }

        private void AfterLoad(int slot, SavesManager.GameSave gameSave)
        {
            _loadResponses++;
            _gameSaves[slot] = gameSave;
            if (_loadResponses == 3)
            {
                _loadingVisible = false;
            }
            else
            {
                SavesManager.Instance.ExecuteLoad(_loadResponses, AfterLoad);
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!_loadingVisible)
            {
                if (InputManager.Instace.KeyPressed(Keys.Down, Keys.Right))
                {
                    if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Load)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            _slotIndex = _slotIndex >= 2 ? 0 : _slotIndex + 1;
                            if (_gameSaves[_slotIndex].StagesCompleted > 0)
                                break;
                        }
                    }
                    else
                    {
                        _slotIndex = _slotIndex >= 2 ? 0 : _slotIndex + 1;
                    }
                }
                if (InputManager.Instace.KeyPressed(Keys.Up, Keys.Left))
                {
                    if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Load)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            _slotIndex = _slotIndex <= 0 ? 2 : _slotIndex - 1;
                            if (_gameSaves[_slotIndex].StagesCompleted > 0)
                                break;
                        }
                    }
                    else
                    {
                        _slotIndex = _slotIndex <= 0 ? 2 : _slotIndex - 1;
                    }
                }
                if (InputManager.Instace.KeyPressed(Keys.Z, Keys.Enter))
                {
                    HandleConfirm();
                }
            }

            _peteAnimatedSprite.Update(gameTime);
            _nextStageMarkAnimatedSprite.Update(gameTime);

            if (_loadingVisible)
                _loadingAnimatedSprite.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleConfirm()
        {
            switch (SceneManager.Instance.TypeOfSceneSaves)
            {
                case SceneManager.SceneSavesType.NewGame:
                    break;
                case SceneManager.SceneSavesType.Load:
                    break;
                case SceneManager.SceneSavesType.Save:
                    Save();
                    break;
            }
        }

        private void Save()
        {
            if (!_loadingVisible)
            {
                _loadingVisible = true;
                SavesManager.Instance.ExecuteSave(_slotIndex, AfterActionExec);
            }
        }
        
        private void AfterActionExec()
        {
            _loadingVisible = false;
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

            // Slots
            for (var i = 0; i < 3; i++)
            {
                var slotPosition = _slotsPosition[i].Location.ToVector2();
                var gameFont = SceneManager.Instance.GameFont;
                // Background
                spriteBatch.Draw(_slotTexture, _slotsPosition[i], Color.White * 0.5f);

                // Check if the slot isn't empty
                if (_gameSaves[i].StagesCompleted > 0)
                {
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
                    spriteBatch.DrawString(gameFont, "SAVE NAME", slotPosition + _namePosition, _fontColor);
                    // Stages
                    var divisorPosition = Vector2.Zero;
                    for (var j = 0; j < _gameSaves[i].StagesCompleted; j++)
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
                    var lives = _gameSaves[i].Lives;
                    var livesWidth = (lives * _lifeFrame.Width) + (lives - 1);
                    var livesPosition = slotPosition + (_slotsPosition[i].Width - livesWidth - _livesPosition.X) * Vector2.UnitX + _livesPosition.Y * Vector2.UnitY;
                    for (var j = 0; j < lives; j++)
                    {
                        var lifePosition = livesPosition + ((_lifeFrame.Width + 1) * j * Vector2.UnitX);
                        spriteBatch.Draw(_iconsSpritesheet, lifePosition, _lifeFrame, Color.White);
                    }
                    // Hearts
                    var hearts = _gameSaves[i].Hearts;
                    var heartsWidth = (hearts * _heartFrame.Width) + (hearts - 1) * 5;
                    var heartsPosition = slotPosition + (_slotsPosition[i].Width - heartsWidth - _heartsPosition.X) * Vector2.UnitX + _heartsPosition.Y * Vector2.UnitY;
                    for (var j = 0; j < hearts; j++)
                    {
                        var heartPosition = heartsPosition + ((_heartFrame.Width + 5) * j * Vector2.UnitX);
                        spriteBatch.Draw(_iconsSpritesheet, heartPosition, _heartFrame, Color.White);
                    }
                    // Ammo
                    spriteBatch.Draw(_iconsSpritesheet, slotPosition + _ammoPosition, _ammoFrame, Color.White);
                    spriteBatch.DrawString(gameFont, _gameSaves[i].Ammo.ToString(), slotPosition + _ammoTextPosition, _fontColor);
                    // Coins
                    spriteBatch.Draw(_iconsSpritesheet, slotPosition + _coinsPosition, _coinFrame, Color.White);
                    spriteBatch.DrawString(gameFont, _gameSaves[i].Coins.ToString(), slotPosition + _coinsTextPosition, _fontColor);
                }
                else
                {
                    // Empty slot text
                    var emptySlotTextSize = gameFont.MeasureString(_emptySlotText);
                    var emptyPosition = new Vector2(slotPosition.X + (_slotsPosition[i].Width - emptySlotTextSize.X) / 2,
                        slotPosition.Y + (_slotsPosition[i].Height - emptySlotTextSize.Y) / 2);
                    spriteBatch.DrawString(gameFont, _emptySlotText, emptyPosition, _fontColor);
                }
            }

            if (_loadingVisible)
            {
                var screenSize = SceneManager.Instance.GraphicsDevice.Viewport.Bounds;
                spriteBatch.Draw(_loadingBackgroundTexture, new Rectangle(0, 0, screenSize.Width, screenSize.Height),
                    Color.White * 0.5f);
                _loadingAnimatedSprite.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
