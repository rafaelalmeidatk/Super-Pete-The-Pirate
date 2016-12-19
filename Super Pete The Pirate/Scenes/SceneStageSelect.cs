using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Sprites;
using System;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneStageSelect : SceneBase
    {
        //--------------------------------------------------
        // Constants

        private const string ScenePathName = "stageSelect";
        private const string MapPressMessageKey = "Press Z to start!";
        private const string MapPressMessagePad = "Press A to start!";

        //--------------------------------------------------
        // Positions

        private Vector2 _hpTextPosition;
        private Rectangle _hpSpritesArea;
        private Vector2 _livesTextPosition;
        private Rectangle _livesSpritesArea;

        private Vector2 _ammoPosition;
        private Vector2 _coinsPosition;
        private Vector2 _shadowPositionIncrease;

        private Vector2 _hearthSpritePosition;
        private Vector2 _lifeSpritePosition;

        //--------------------------------------------------
        // Sprites on icon texture

        private Rectangle _heartSprite;
        private Rectangle _lifeSprite;

        //--------------------------------------------------
        // Map

        private Texture2D _stageSelectionSpritesheet;
        private AnimatedSprite _stageSelectionSprite;
        private Vector2[] _stageSelectionPositions;

        private AnimatedSprite _stageSelectionPeteSprite;

        private Vector2 _pressZTextPosition;
        private float _pressZTextInitY;
        private bool _pressZTextSide;

        //--------------------------------------------------
        // Font color

        private Color _fontColor;
        private Color _fontShadowColor;

        //--------------------------------------------------
        // Textures

        private Texture2D _backgroundTexture;
        private Texture2D _iconsTexture;

        //--------------------------------------------------
        // Rank S Mark

        private Rectangle[] _sStageMarkFrames;
        private int _sStageIndex;
        private float _sStageTick;
        private const float SStageInterval = 200.0f;

        //--------------------------------------------------
        // Selected index

        private int _selectedIndex;

        //----------------------//------------------------//

        public override void LoadContent()
        {
            base.LoadContent();

            // Positions init
            _ammoPosition = new Vector2(33, 166);
            _coinsPosition = new Vector2(33, 217);
            _shadowPositionIncrease = new Vector2(1, 1);

            _hpTextPosition = new Vector2(22, 10);
            _livesTextPosition = new Vector2(13, 78);

            _hpSpritesArea = new Rectangle(2, 24, 60, 45);
            _livesSpritesArea = new Rectangle(2, 95, 60, 45);

            _hearthSpritePosition = new Vector2(0, 0);
            _lifeSpritePosition = new Vector2(0, 11);

            // Sprites init
            _heartSprite = new Rectangle(1, 16, 13, 11);
            _lifeSprite = new Rectangle(0, 0, 15, 14);

            // Font color init
            _fontColor = new Color(8, 11, 24);
            _fontShadowColor = new Color(169, 113, 82);

            // Textures init
            _backgroundTexture = ImageManager.loadScene(ScenePathName, "IslandMapBackground");
            _iconsTexture = ImageManager.loadScene(ScenePathName, "IconMap");

            // Index init
            _selectedIndex = GetCurrentStage() >= SceneManager.MaxLevels ? SceneManager.MaxLevels - 1 : GetCurrentStage();

            // Start BGM
            SoundManager.StartBgm("AchaidhCheide");

            SetupMap();
        }

        private void SetupMap()
        {
            var maxLevels = SceneManager.MaxLevels;
            _stageSelectionPositions = new Vector2[]
            {
                new Vector2(111, 130),
                new Vector2(151, 88),
                new Vector2(192, 172),
                new Vector2(231, 126),
                new Vector2(278, 134)
            };
            var currentStage = MathHelper.Clamp(GetCurrentStage(), 0, 4);
            var nextLevelPos = _stageSelectionPositions[currentStage];
            var currentLevelPos = _stageSelectionPositions[Math.Max(currentStage, 0)];
            var stageSelectionPosition = GetCurrentStage() > maxLevels ? _stageSelectionPositions[0] : GetCurrentStage() >= maxLevels ? currentLevelPos : nextLevelPos;

            _stageSelectionSpritesheet = ImageManager.loadScene(ScenePathName, "StageSelectionSpritesheet");
            var stageSelectionFrames = new Rectangle[]
            {
                new Rectangle(0, 0, 19, 19),
                new Rectangle(19, 0, 19, 19),
                new Rectangle(38, 0, 19, 19),
                new Rectangle(57, 0, 19, 19)
            };
            _stageSelectionSprite = new AnimatedSprite(_stageSelectionSpritesheet, stageSelectionFrames, 130,
                (int)stageSelectionPosition.X, (int)stageSelectionPosition.Y);

            _stageSelectionSprite.Origin = new Vector2(9, 9);
            
            if (GetCurrentStage() == maxLevels)
                _stageSelectionSprite.IsVisible = false;

            var _stageSelectionPeteSpritesheet = ImageManager.loadScene(ScenePathName, "StageSelectionPeteSpritesheet");
            var _stageSelectionPeteFrames = new Rectangle[]
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
            _stageSelectionPeteSprite = new AnimatedSprite(_stageSelectionPeteSpritesheet, _stageSelectionPeteFrames, 100,
                (int)stageSelectionPosition.X, (int)stageSelectionPosition.Y);
            _stageSelectionPeteSprite.Origin = new Vector2(16, 24);

            _sStageMarkFrames = new Rectangle[]
            {
                new Rectangle(114, 0, 19, 19),
                new Rectangle(133, 0, 19, 19)
            };

            var stringSize = SceneManager.Instance.GameFont.MeasureString(MapPressMessage());
            _pressZTextPosition = new Vector2(
                62 + (288 - (int)stringSize.X) / 2,
                SceneManager.Instance.VirtualSize.Y - stringSize.Y - 15
            );
            _pressZTextInitY = _pressZTextPosition.Y;
            _pressZTextSide = true;

            UpdatePeteHeadPosition();
        }

        public override void Update(GameTime gameTime)
        {
            DebugValues["Selected Index"] = _selectedIndex.ToString();

            if (InputManager.Instace.Pressed(InputCommand.Right))
            {
                var lastIndex = _selectedIndex;
                var limit = GetCurrentStage() == SceneManager.MaxLevels ? SceneManager.MaxLevels - 1 : GetCurrentStage();
                _selectedIndex = _selectedIndex + 1 > limit ? 0 : _selectedIndex + 1;
                if (lastIndex != _selectedIndex)
                {
                    UpdatePeteHeadPosition();
                    SoundManager.PlaySelectSe();
                }
            }

            if (InputManager.Instace.Pressed(InputCommand.Left))
            {
                var lastIndex = _selectedIndex;
                var limit = GetCurrentStage() == SceneManager.MaxLevels ? SceneManager.MaxLevels - 1 : GetCurrentStage();
                _selectedIndex = _selectedIndex - 1 < 0 ? limit : _selectedIndex - 1;
                if (lastIndex != _selectedIndex)
                {
                    UpdatePeteHeadPosition();
                    SoundManager.PlaySelectSe();
                }
            }

            if (InputManager.Instace.Pressed(InputCommand.A))
            {
                SoundManager.PlayConfirmSe();
                LoadMap();
            }

            if (InputManager.Instace.Pressed(InputCommand.Cancel))
            {
                SoundManager.PlayCancelSe();
                SceneManager.Instance.ChangeScene("SceneTitle");
            }

            _stageSelectionSprite.Update(gameTime);
            _stageSelectionPeteSprite.Update(gameTime);

            if (_pressZTextPosition.Y <= _pressZTextInitY + 7 && _pressZTextSide)
            {
                _pressZTextPosition.Y += 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_pressZTextPosition.Y > _pressZTextInitY + 7)
                {
                    _pressZTextSide = false;
                }
            }
            else if (_pressZTextPosition.Y >= _pressZTextInitY && !_pressZTextSide)
            {
                _pressZTextPosition.Y -= 10 * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_pressZTextPosition.Y <= _pressZTextInitY)
                {
                    _pressZTextSide = true;
                }
            }

            _sStageTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            if (_sStageTick >= SStageInterval)
            {
                _sStageTick = 0;
                _sStageIndex = _sStageIndex == 0 ? 1 : 0;
            }

            base.Update(gameTime);
        }

        private void UpdatePeteHeadPosition()
        {
            _stageSelectionPeteSprite.Position = _stageSelectionPositions[_selectedIndex];
        }

        private void LoadMap()
        {
            SceneManager.Instance.MapToLoad = _selectedIndex + 1;
            SceneManager.Instance.ChangeScene("SceneMap");
        }

        #region Draws

        public override void Draw(SpriteBatch spriteBatch, ViewportAdapter viewportAdapter)
        {
            base.Draw(spriteBatch, viewportAdapter);

            spriteBatch.Begin(samplerState: SamplerState.PointClamp, transformMatrix: viewportAdapter.GetScaleMatrix());

            // Background
            spriteBatch.Draw(_backgroundTexture, _backgroundTexture.Bounds, Color.White);

            // Strings
            DrawWithShadow(spriteBatch, PlayerManager.Instance.Ammo.ToString(), _ammoPosition);
            DrawWithShadow(spriteBatch, PlayerManager.Instance.Coins.ToString(), _coinsPosition);
            DrawWithShadow(spriteBatch, "HP", _hpTextPosition);
            DrawWithShadow(spriteBatch, "Lives", _livesTextPosition);

            // Hearts and lives
            DrawCenteredSpritesOnRectangle(spriteBatch, _hpSpritesArea, _heartSprite, PlayerManager.Instance.Hearts);
            DrawCenteredSpritesOnRectangle(spriteBatch, _livesSpritesArea, _lifeSprite, PlayerManager.Instance.Lives);

            var stages = PlayerManager.Instance.StagesCompleted;
            for (var i = 0; i < GetCurrentStage(); i++)
            {
                var position = new Rectangle((int)_stageSelectionPositions[i].X, (int)_stageSelectionPositions[i].Y, 19, 19);
                if (stages[i].RankS)
                {
                    spriteBatch.Draw(_stageSelectionSpritesheet, position, _sStageMarkFrames[_sStageIndex], Color.White, 0f,
                        new Vector2(9, 9), SpriteEffects.None, 0f);
                }
                else
                {
                    spriteBatch.Draw(_stageSelectionSpritesheet, position, new Rectangle(76, 0, 19, 19), Color.White, 0f,
                        new Vector2(9, 9), SpriteEffects.None, 0f);
                }
            }

            spriteBatch.DrawString(SceneManager.Instance.GameFont, MapPressMessage(), _pressZTextPosition, _fontColor);

            // Map sprites
            _stageSelectionSprite.Draw(spriteBatch);
            _stageSelectionPeteSprite.Draw(spriteBatch);

            spriteBatch.End();
        }

        private void DrawWithShadow(SpriteBatch spriteBatch, String text, Vector2 position)
        {
            spriteBatch.DrawString(SceneManager.Instance.GameFont, text, position + _shadowPositionIncrease, _fontShadowColor);
            spriteBatch.DrawString(SceneManager.Instance.GameFont, text, position, _fontColor);
        }

        private void DrawCenteredSpritesOnRectangle(SpriteBatch spriteBatch, Rectangle area, Rectangle spriteRect, int quantity)
        {
            var spritesSpacing = 2;
            var rows = (quantity + 2) / 3;
            var columns = quantity < 3 ? quantity : 3;
            var spritesContainer = new Rectangle(0, 0,
                (columns * spriteRect.Width) + (spritesSpacing * (columns - 1)),
                (rows * spriteRect.Height) + (spritesSpacing * (rows - 1))
            );
            spritesContainer.X = (area.Width - spritesContainer.Width) / 2;
            spritesContainer.Y = (area.Height - spritesContainer.Height) / 2;

            for (var i = 0; i < quantity; i++)
            {
                var position = new Vector2(
                    area.X + spritesContainer.X + ((i % 3) * (spriteRect.Width + spritesSpacing) -
                        (quantity > 1 ? spritesSpacing / 2 : 0)),
                    area.Y + spritesContainer.Y + ((i / 3) * (spriteRect.Height + spritesSpacing) -
                        (quantity > 1 ? spritesSpacing / 2 : 0))
                );
                spriteBatch.Draw(_iconsTexture, position, spriteRect, Color.White);
            }
        }

        #endregion

        private int GetCurrentStage()
        {
            var stagesCompleted = PlayerManager.Instance.StagesCompleted;
            var count = 0;
            for (var i = 0; i < stagesCompleted.Length; i++)
            {
                if (stagesCompleted[i].Completed)
                    count++;
            }
            return count;
        }

        private string MapPressMessage()
        {
            if (InputManager.Instace.IsPadConnected())
                return MapPressMessagePad;
            else
                return MapPressMessageKey;
        }
    }
}
