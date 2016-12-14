using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Extensions;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Sprites;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneSaves : SceneBase
    {
        //--------------------------------------------------
        // Strings

        private Dictionary<SceneManager.SceneSavesType, string> _titleStrings;
        private const string EmptySlotText = "Empty";
        private const string ConfirmButtonLabel = "Confirm";
        private const string CancelButtonLabel = "Return";

        //--------------------------------------------------
        // Textures

        private Texture2D _backgroundTexture;
        private Texture2D _loadingBackgroundTexture;
        private Texture2D _peteSpritesheet;
        private Texture2D _stageSpritesheet;
        private Texture2D _iconsSpritesheet;

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
        private Color _secFontColor;

        //--------------------------------------------------
        // Slots

        private Rectangle[] _slotsPosition;
        private int _slotIndex;
        private Vector2 _arrowPosition;
        private float _arrowPositionInc;

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
                { SceneManager.SceneSavesType.Load, "Load Game" }
            };

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
            _secFontColor = new Color(222, 196, 158);

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

            // Arrow init
            _arrowPosition = new Vector2(10, _slotsPosition[_slotIndex].Y + (_slotsPosition[_slotIndex].Height - 15) / 2);

            // General variables
            _loadingVisible = true;
            _loadResponses = 0;

            // Play BGM
            SoundManager.StartBgm("AchaidhCheide");

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
                if (InputManager.Instace.Pressed(InputCommand.Down))
                {
                    var lastIndex = _slotIndex;
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
                    if (lastIndex != _slotIndex)
                    {
                        UpdateArrowPosition();
                        SoundManager.PlaySelectSe();
                    }
                }

                if (InputManager.Instace.Pressed(InputCommand.Up))
                {
                    var lastIndex = _slotIndex;
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
                    if (lastIndex != _slotIndex)
                    {
                        UpdateArrowPosition();
                        SoundManager.PlaySelectSe();
                    }
                }

                if (InputManager.Instace.Pressed(InputCommand.A))
                {
                    HandleConfirm();
                }

                if (InputManager.Instace.Pressed(InputCommand.Cancel))
                {
                    SoundManager.PlayCancelSe();
                    HandleExit();
                }
            }

            _peteAnimatedSprite.Update(gameTime);
            _nextStageMarkAnimatedSprite.Update(gameTime);
            _arrowPositionInc = (float)MathUtils.SinInterpolation(-1.2, 1.2, gameTime.TotalGameTime.TotalMilliseconds / 3);

            DebugValues["slot index"] = _slotIndex.ToString();

            if (_loadingVisible)
                _loadingAnimatedSprite.Update(gameTime);

            base.Update(gameTime);
        }

        private void UpdateArrowPosition()
        {
            _arrowPosition.Y = _slotsPosition[_slotIndex].Y + ((_slotsPosition[_slotIndex].Height - 15) / 2) - 1;
        }

        private void HandleConfirm()
        {
            if (_loadingVisible) return;
            if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Load)
                Load();
            else if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Save)
                Save();
        }

        private void HandleExit()
        {
            if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Load)
                SceneManager.Instance.ChangeScene("SceneTitle");
            else if (SceneManager.Instance.TypeOfSceneSaves == SceneManager.SceneSavesType.Save)
                SceneManager.Instance.ChangeScene("SceneStageSelect");
        }

        private void Load()
        {
            var save = _gameSaves[_slotIndex];
            if (save.StagesCompleted > 0)
            {
                SoundManager.PlayConfirmSe();
                PlayerManager.Instance.SetData(save.Ammo, save.Lives, save.Hearts, save.Coins, save.StagesCompleted);
                SceneManager.Instance.ChangeScene("SceneStageSelect");
            }
            else
            {
                SoundManager.PlayCancelSe();
            }
        }

        private void Save()
        {
            _loadingVisible = true;
            SavesManager.Instance.ExecuteSave(_slotIndex, AfterActionExec);
        }
        
        private void AfterActionExec()
        {
            _loadingVisible = false;
            SceneManager.Instance.ChangeScene("SceneStageSelect");
        }

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            var screenSize = SceneManager.Instance.VirtualSize;
            var gameFont = SceneManager.Instance.GameFont;

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewportAdapter.GetScaleMatrix());

            // Background
            spriteBatch.Draw(_backgroundTexture, _backgroundTexture.Bounds, Color.White);

            // Title
            var titleString = _titleStrings[SceneManager.Instance.TypeOfSceneSaves];
            var titleX = (SceneManager.Instance.VirtualSize.X - SceneManager.Instance.GameFontBig.MeasureString(titleString).X) / 2;
            spriteBatch.DrawTextWithShadow(SceneManager.Instance.GameFontBig, titleString, new Vector2(titleX, 10), Color.White);

            // Slots
            for (var i = 0; i < 3; i++)
            {
                var slotPosition = _slotsPosition[i].Location.ToVector2();

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
                        if (j < SceneManager.MaxLevels - 1)
                        {
                            divisorPosition = markPosition + (_stagePeteMarkFrame.Width + 1) * Vector2.UnitX + (9 * Vector2.UnitY);
                            spriteBatch.Draw(_stageSpritesheet, divisorPosition, _stageDivisorFrame, Color.White);
                        }
                    }

                    if (_gameSaves[i].StagesCompleted < SceneManager.MaxLevels)
                    {
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
                    var emptySlotTextSize = gameFont.MeasureString(EmptySlotText);
                    var emptyPosition = new Vector2(slotPosition.X + (_slotsPosition[i].Width - emptySlotTextSize.X) / 2,
                        slotPosition.Y + (_slotsPosition[i].Height - emptySlotTextSize.Y) / 2);
                    spriteBatch.DrawString(gameFont, EmptySlotText, emptyPosition, _fontColor);
                }
            }

            IconsManager.Instance.DrawRightArrow(spriteBatch, _arrowPosition + _arrowPositionInc * Vector2.UnitY, false);

            var xOffset = SceneManager.Instance.GameFontSmall.MeasureString(ConfirmButtonLabel).X + 30;
            IconsManager.Instance.DrawAButton(spriteBatch, new Vector2(5, screenSize.Y - 18), false, ConfirmButtonLabel, 1.0f, true);
            IconsManager.Instance.DrawBButton(spriteBatch, new Vector2(xOffset, screenSize.Y - 18), false, CancelButtonLabel, 1.0f, true);

            if (_loadingVisible)
            {
                spriteBatch.Draw(_loadingBackgroundTexture, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y),
                    Color.White * 0.5f);
                _loadingAnimatedSprite.Draw(spriteBatch);
            }

            spriteBatch.End();
        }
    }
}
