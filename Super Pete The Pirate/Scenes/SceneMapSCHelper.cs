using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.Sprites;
using Super_Pete_The_Pirate.Managers;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMapSCHelper
    {
        //--------------------------------------------------
        // Scene

        private const string SceneResFolder = "map";

        //--------------------------------------------------
        // Phase

        private int _phase;

        private float _rowShowTick;
        private const float RowShowMaxTick = 800.0f;

        private float _rankShowTick;
        private const float RankShowMaxTick = 250.0f;

        //--------------------------------------------------
        // Data Class

        public struct StageCompletedData
        {
            public int CoinsCollected;
            public int MaxCoins;
            public int HeartsLost;
            public int EnemiesDefeated;
            public int MaxEnemies;
            public TimeSpan Time;
            public TimeSpan MaxTime;
            public bool Failed;
        }

        private StageCompletedData _data;

        //--------------------------------------------------
        // Background

        private Texture2D _background;

        //--------------------------------------------------
        // Positions

        private const int Current = 0;
        private const int Initial = 1;

        private float _initialRightX;
        private float[,] _rowsInnerPositionsLeft;
        private float[] _rowsInnerPositionsRight;
        private int _rowIndex;

        private Vector2 _titlePosition;

        //--------------------------------------------------
        // Strings

        private const string TitleCompleted = "Stage Completed!";
        private const string TitleFailed = "Stage Failed...";
        private const string CoinsEarned = "Coins earned:";
        private const string HeartsLost = "Hearts lost:";
        private const string EnemiesDefeated = "Enemies defeated:";
        private const string Time = "Time:";

        //--------------------------------------------------
        // Values

        private bool _numbersTime;
        private TimeSpan[] _timeValues;
        private int[,] _values;

        //--------------------------------------------------
        // Rank

        private string _rank;
        private Sprite _rankSprite;
        private Dictionary<string, string> _rankSentences;
        private Vector2 _rankSentencePosition;
        private float _rankSentenceAlpha;

        //--------------------------------------------------
        // Buttons

        private float _buttonsAlpha;

        //--------------------------------------------------
        // Sounds FX

        private SoundEffect _numberSe;

        //--------------------------------------------------
        // Colors

        private readonly Color _greenColor;
        private readonly Color _redColor;

        //--------------------------------------------------
        // Completed

        private bool _completed;

        //----------------------//------------------------//

        public SceneMapSCHelper()
        {
            _phase = 0;

            var screenSize = SceneManager.Instance.VirtualSize;
            var font = SceneManager.Instance.GameFontBig;

            _greenColor = new Color(29, 184, 127);
            _redColor = new Color(221, 13, 82);

            _initialRightX = screenSize.X + 20;

            _rankShowTick = 0.0f;
            _rowShowTick = 0.0f;
            _rowIndex = 0;

            _rankSentences = new Dictionary<string, string>()
            {
                { "S", "Perfect!" },
                { "A", "Well done" },
                { "B", "Good" },
                { "C", "Normal" },
                { "D", "Bad" },
                { "F", "F for fail" }
            };
            _rankSentenceAlpha = 0.0f;

            _buttonsAlpha = 0.0f;
            _numberSe = SoundManager.LoadSe("Numbers");
            
            _background = new Texture2D(SceneManager.Instance.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _background.SetData(new Color[] { Color.Black });
        }

        public void Initialize(StageCompletedData data)
        {
            _data = data;
            var screenSize = SceneManager.Instance.VirtualSize;
            var font = SceneManager.Instance.GameFont;
            var format = "{0}/{1}";

            var coins = data.CoinsCollected;
            var hearts = data.HeartsLost;
            var enemies = data.EnemiesDefeated;
            var time = data.Time;
            var failed = data.Failed;

            var titleMesured = SceneManager.Instance.GameFontBig.MeasureString(failed ? TitleFailed : TitleCompleted);
            _titlePosition = new Vector2((screenSize.X - titleMesured.X) / 2, -titleMesured.Y);

            var l1 = font.MeasureString(CoinsEarned);
            var l2 = font.MeasureString(HeartsLost);
            var l3 = font.MeasureString(EnemiesDefeated);
            var l4 = font.MeasureString(Time);
            
            _rowsInnerPositionsRight = new float[]
            {
                -font.MeasureString(String.Format(format, 0, data.MaxCoins)).X - 10,
                -font.MeasureString(String.Format(format, 0, data.HeartsLost)).X - 10,
                -font.MeasureString(String.Format(format, 0, data.MaxEnemies)).X - 10,
                -font.MeasureString(FormatTime(data.Time)).X - 10
            };

            _rowsInnerPositionsLeft = new float[,]
            {
                // Current, Initial
                { -l1.X, -l1.X },
                { -l2.X, -l2.X },
                { -l3.X, -l3.X },
                { -l4.X, -l4.X },
            };

            _values = new int[,]
            {
                { 0, coins },
                { 0, hearts },
                { 0, enemies }
            };

            _timeValues = new TimeSpan[] { new TimeSpan(), time };

            if (failed)
            {
                _rank = "F";
            }
            else
            {
                _completed = true;
                CalculateRank(data);
            }
            
            if (_rank == "S")
            {
                var texture = ImageManager.loadScene(SceneResFolder, "Rank" + _rank);
                var frames = new Rectangle[]
                {
                    new Rectangle(0, 0, 80, 80),
                    new Rectangle(80, 0, 80, 80),
                    new Rectangle(160, 0, 80, 80)
                };
                _rankSprite = new AnimatedSprite(texture, frames, 100, Vector2.Zero);
                _rankSprite.Origin = new Vector2(40, 40);
            }
            else
            {
                _rankSprite = new Sprite(ImageManager.loadScene(SceneResFolder, "Rank" + _rank));
            }

            _rankSprite.Position = new Vector2(315, 199);
            _rankSprite.IsVisible = false;
            _rankSprite.Scale = new Vector2(3, 3);
            var rm = font.MeasureString(_rankSentences[_rank]);
            _rankSentencePosition = new Vector2(270 - rm.X, 199 - rm.Y / 2);
        }

        private void CalculateRank(StageCompletedData data)
        {
            var ranks = new string[] { "D", "C", "B", "A", "S" };
            var count = 0;
            if (data.CoinsCollected >= data.MaxCoins)
                count++;
            if (data.HeartsLost == 0)
                count++;
            if (data.EnemiesDefeated >= data.MaxEnemies)
                count++;
            if (data.Time <= data.MaxTime)
                count++;
            _rank = ranks[count];
        }

        public void Update(GameTime gameTime)
        {
            if (InputManager.Instace.Pressed(InputCommand.A))
            {
                if (_phase == 0)
                {
                    if (_rowsInnerPositionsLeft[_rowIndex, Current] < 20.0f)
                    {
                        _rowsInnerPositionsRight[_rowIndex] = 20;
                        _rowsInnerPositionsLeft[_rowIndex, Current] = 20.0f;
                        _rowShowTick = 0.0f;
                        _numbersTime = true;
                    }
                    else
                    {
                        if (_rowIndex < _values.GetLength(0))
                        {
                            _values[_rowIndex, 0] = _values[_rowIndex, 1];
                            _rowIndex++;
                            _rowShowTick = 0.0f;
                            _numbersTime = false;
                        }
                        else
                        {
                            _timeValues[0] = _timeValues[1];
                            _phase++;
                        }
                    }
                }
            }

            if (InputManager.Instace.Pressed(InputCommand.A) && _phase >= 2)
            {
                if (PlayerManager.Instance.Lives > 0)
                {
                    if (PlayerManager.Instance.StagesCompleted < GameMap.Instance.CurrentMapId)
                        PlayerManager.Instance.CompleteStage();
                    SceneManager.Instance.TypeOfSceneSaves = SceneManager.SceneSavesType.Save;
                    SceneManager.Instance.ChangeScene("SceneSaves");
                }
                else
                {
                    if (!_completed)
                    {
                        SceneManager.Instance.ChangeScene("SceneGameover");
                    }
                    else
                    {
                        PlayerManager.Instance.ResetHeartsAndLives();
                        SceneManager.Instance.ChangeScene("SceneStageSelect");
                    }
                }
            }

            if (InputManager.Instace.Pressed(InputCommand.B))
            {
                PlayerManager.Instance.ResetHeartsAndLives();
                SceneManager.Instance.ChangeScene("SceneMap"); // Restart the stage
            }

            if (_phase == 0)
            {
                if (_titlePosition.Y < 20)
                    _titlePosition.Y = MathHelper.Lerp(_titlePosition.Y, 20, 0.1f);

                _rowShowTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                var delta = _rowShowTick / RowShowMaxTick;
                var i = _rowIndex;

                if (_rowsInnerPositionsLeft[i, Current] < 20.0f && !_numbersTime)
                {
                    _rowsInnerPositionsRight[i] = MathHelper.Lerp(-20, 20, delta);
                    _rowsInnerPositionsLeft[i, Current] = MathHelper.Lerp(_rowsInnerPositionsLeft[i, Initial], 20.0f, delta);
                    if (_rowsInnerPositionsLeft[i, Current] >= 20.0f)
                    {
                        _rowShowTick = 0.0f;
                        _numbersTime = true;
                    }
                }
                else
                {
                    if (_rowIndex < _values.GetLength(0))
                    {
                        var newValue = (int)MathHelper.Lerp(0, _values[i, 1], delta);
                        if (_values[i, 0] != newValue)
                        {
                            _numberSe.Play();
                        }
                        _values[i, 0] = newValue;

                        if (_values[i, 0] >= _values[i, 1])
                        {
                            _rowIndex++;
                            _rowShowTick = 0.0f;
                            _numbersTime = false;
                        }
                    }
                    else
                    {
                        var newValue = TimeSpan.FromTicks((long)MathHelper.Lerp(0.0f, _timeValues[1].Ticks, delta));
                        if ((int)_timeValues[0].TotalSeconds != (int)newValue.TotalSeconds)
                        {
                            _numberSe.Play();
                        }
                        _timeValues[0] = newValue;
                        if (_timeValues[0] >= _timeValues[1])
                            _phase++;
                    }
                }
            }

            if (_phase == 1)
            {
                if (!_rankSprite.IsVisible)
                    _rankSprite.IsVisible = true;

                _rankShowTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                var delta = _rankShowTick / RankShowMaxTick;

                var scale = MathHelper.Lerp(3.0f, 1.0f, delta);
                _rankSprite.Scale = new Vector2(scale, scale);
                _rankSentenceAlpha = MathHelper.Lerp(0.0f, 1.0f, delta);

                if (scale <= 1.0f)
                    _phase++;
            }

            if (_rank == "S")
                ((AnimatedSprite)_rankSprite).Update(gameTime);

            if (_buttonsAlpha < 1.0f)
                _buttonsAlpha += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var screenSize = SceneManager.Instance.VirtualSize;
            var fontBig = SceneManager.Instance.GameFontBig;
            var font = SceneManager.Instance.GameFont;
            var format = "{0}/{1}";

            spriteBatch.Draw(_background, new Rectangle(0, 0, (int)screenSize.X, (int)screenSize.Y), Color.White * 0.5f);
            
            var coinsLPos = new Vector2(_rowsInnerPositionsLeft[0, Current], 60);
            var coinsRPos = new Vector2(_rowsInnerPositionsRight[0], 60);

            var heartsLPos = new Vector2(_rowsInnerPositionsLeft[1, Current], 85);
            var heartsRPos = new Vector2(_rowsInnerPositionsRight[1], 85);

            var enemiesLPos = new Vector2(_rowsInnerPositionsLeft[2, Current], 110);
            var enemiesRPos = new Vector2(_rowsInnerPositionsRight[2], 110);

            var timeLPos = new Vector2(_rowsInnerPositionsLeft[3, Current], 135);
            var timeRPos = new Vector2(_rowsInnerPositionsRight[3], 135);

            spriteBatch.DrawTextWithShadow(fontBig, _completed ? TitleCompleted : TitleFailed, _titlePosition, Color.White);

            var coinsStr = String.Format(format, _values[0, 0], _data.MaxCoins);
            var coinsColor = CalculateColor(_values[0, 0], _data.MaxCoins);
            spriteBatch.DrawTextWithShadow(font, CoinsEarned, coinsLPos, Color.White);
            spriteBatch.DrawRightText(font, coinsStr, coinsRPos, coinsColor, Color.Black);

            var heartsColor = CalculateColor(_values[1, 0], 0);
            spriteBatch.DrawTextWithShadow(font, HeartsLost, heartsLPos, Color.White);
            spriteBatch.DrawRightText(font, _values[1, 0].ToString(), heartsRPos, heartsColor, Color.Black);

            var enemiesStr = String.Format(format, _values[2, 0], _data.MaxEnemies);
            var enemiesColor = CalculateColor(_values[2, 0], _data.MaxEnemies);
            spriteBatch.DrawTextWithShadow(font, EnemiesDefeated, enemiesLPos, Color.White);
            spriteBatch.DrawRightText(font, enemiesStr, enemiesRPos, enemiesColor, Color.Black);

            var timeColor = CalculateColor(_timeValues[0], _data.MaxTime);
            spriteBatch.DrawTextWithShadow(font, Time, timeLPos, Color.White);
            spriteBatch.DrawRightText(font, FormatTime(_timeValues[0]), timeRPos, timeColor, Color.Black);

            if (_phase >= 1)
            {
                spriteBatch.DrawTextWithShadow(font, _rankSentences[_rank], _rankSentencePosition, Color.White * _rankSentenceAlpha);

                if (_rank == "S")
                    ((AnimatedSprite)_rankSprite).Draw(spriteBatch);
                else
                    _rankSprite.Draw(spriteBatch);
            }

            IconsManager.Instance.DrawAButton(spriteBatch, new Vector2(5, screenSize.Y - 40), false, "Continue", _buttonsAlpha, true);
            IconsManager.Instance.DrawBButton(spriteBatch, new Vector2(5, screenSize.Y - 20), false, "Again", _buttonsAlpha, true);
        }

        private Color CalculateColor(int value, int maxValue)
        {
            if (maxValue == 0)
            {
                if (value == 0)
                    return _greenColor;
                if (value <= 5)
                    return Color.White;
                else
                    return _redColor;
            }

            var percent = (float)value / maxValue;
            if (percent == 1)
                return _greenColor;
            else if (percent >= 0.5f)
                return Color.White;
            else
                return _redColor;
        }

        private Color CalculateColor(TimeSpan time, TimeSpan maxTime)
        {
            var value = time.TotalMilliseconds;
            var maxValue = maxTime.TotalMilliseconds;
            if (value <= maxValue)
                return _greenColor;
            else if (value <= maxValue * 1.5)
                return Color.White;
            else
                return _redColor;
        }

        private string FormatTime(TimeSpan time)
        {
            return String.Format("{0}:{1}", (int)time.TotalMinutes, time.Seconds.ToString("D2"));
        }
    }
}
