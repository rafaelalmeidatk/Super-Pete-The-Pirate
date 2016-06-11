using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Scenes
{
    class SceneMapSFHelper
    {
        //--------------------------------------------------
        // Phase

        private int _phase;

        private float _rowShowTick;
        private const float RowShowMaxTick = 800.0f;

        private float _secondPhaseDuration;
        private const float SecondPhaseDuration = 1000.0f;

        //--------------------------------------------------
        // Positions

        private float _initialLeftX;
        private float _leftX;

        private float _initialRightX;

        private float _finalCoinsX;
        private float _coinsX;
        
        private float _finalHeartsX;
        private float _heartsX;
        
        private float _finalEnemiesX;
        private float _enemiesX;

        private float _finalTimeX;
        private float _timeX;

        private float[,] _rowsInnerPositionsLeft;
        private float[,] _rowsInnerPositionsRight;
        private int _rowsIndex;

        private Vector2 _titlePosition;

        //--------------------------------------------------
        // Strings

        private const string Title = "Stage Complete!";
        private const string CoinsEarned = "Coins earned:";
        private const string HeartsLost = "Hearts lost:";
        private const string EnemiesDefeated = "Enemies defeated:";
        private const string Time = "Time:";

        //--------------------------------------------------
        // Values

        private int _coins;
        private int _finalCoins;
        private int _hearts;
        private int _finalHearts;
        private int _enemies;
        private int _finalEnemies;

        private TimeSpan[] _timeValues;
        private int[,] _values;

        //----------------------//------------------------//

        public SceneMapSFHelper()
        {
            _phase = 0;

            var screenSize = SceneManager.Instance.VirtualSize;
            var font = SceneManager.Instance.GameFontBig;

            _initialRightX = screenSize.X + 20;
            _initialLeftX = 0.0f;
            _leftX = 0.0f;

            var titleMesured = font.MeasureString(Title);
            _titlePosition = new Vector2((screenSize.X - titleMesured.X) / 2, -titleMesured.Y);

            _rowShowTick = 0.0f;
            _rowsIndex = 0;
        }

        public void Initialize(int coins, int hearts, int enemies, TimeSpan time)
        {
            var screenSize = SceneManager.Instance.VirtualSize;
            var font = SceneManager.Instance.GameFont;

            var l1 = font.MeasureString(CoinsEarned);
            var l2 = font.MeasureString(HeartsLost);
            var l3 = font.MeasureString(EnemiesDefeated);
            var l4 = font.MeasureString(Time);

            // 0: current, 1: initial, 2: final
            _rowsInnerPositionsRight = new float[,]
            {
                { screenSize.X + 20, screenSize.X + 20, screenSize.X - font.MeasureString(coins.ToString()).X - 20 },
                { screenSize.X + 20, screenSize.X + 20, screenSize.X - font.MeasureString(hearts.ToString()).X - 20 },
                { screenSize.X + 20, screenSize.X + 20, screenSize.X - font.MeasureString(enemies.ToString()).X - 20 },
                { screenSize.X + 20, screenSize.X + 20, screenSize.X - font.MeasureString(FormatTime(time)).X - 20 },
            };

            // 0: current, 1: initial
            _rowsInnerPositionsLeft = new float[,]
            {
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
        }

        public void Update(GameTime gameTime)
        {
            if (_titlePosition.Y < 20)
                _titlePosition.Y = MathHelper.Lerp(_titlePosition.Y, 20, 0.1f);

            if (_phase == 0)
            {
                _rowShowTick += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                var delta = _rowShowTick / RowShowMaxTick;
                var i = _rowsIndex;

                if (_rowsInnerPositionsLeft[i, 0] < 20.0f)
                {
                    _rowsInnerPositionsRight[i, 0] = MathHelper.Lerp(_rowsInnerPositionsRight[i, 1], _rowsInnerPositionsRight[i, 2], delta);
                    _rowsInnerPositionsLeft[i, 0] = MathHelper.Lerp(_rowsInnerPositionsLeft[i, 1], 20.0f, delta);
                    if (_rowsInnerPositionsLeft[i, 0] >= 20.0f)
                        _rowShowTick = 0.0f;
                }
                else
                {
                    if (_rowsIndex < _values.GetLength(0))
                    {
                        _values[i, 0] = (int)MathHelper.Lerp(0, _values[i, 1], delta);
                        if (_values[i, 0] >= _values[i, 1])
                        {
                            _rowsIndex++;
                            _rowShowTick = 0.0f;
                        }
                    }
                    else
                    {
                        _timeValues[0] = TimeSpan.FromTicks((long)MathHelper.Lerp(0.0f, _timeValues[1].Ticks, delta));
                        if (_timeValues[0] >= _timeValues[1])
                            _phase++;
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            var screenSize = SceneManager.Instance.VirtualSize;
            var fontBig = SceneManager.Instance.GameFontBig;
            var font = SceneManager.Instance.GameFont;
            
            var coinsLPos = new Vector2(_rowsInnerPositionsLeft[0, 0], 60);
            var coinsRPos = new Vector2(_rowsInnerPositionsRight[0, 0], 60);

            var heartsLPos = new Vector2(_rowsInnerPositionsLeft[1, 0], 85);
            var heartsRPos = new Vector2(_rowsInnerPositionsRight[1, 0], 85);

            var enemiesLPos = new Vector2(_rowsInnerPositionsLeft[2, 0], 110);
            var enemiesRPos = new Vector2(_rowsInnerPositionsRight[2, 0], 110);

            var timeLPos = new Vector2(_rowsInnerPositionsLeft[3, 0], 135);
            var timeRPos = new Vector2(_rowsInnerPositionsRight[3, 0], 135);

            spriteBatch.DrawTextWithShadow(fontBig, Title, _titlePosition, Color.White);

            spriteBatch.DrawTextWithShadow(font, CoinsEarned, coinsLPos, Color.White);
            spriteBatch.DrawTextWithShadow(font, _values[0, 0].ToString(), coinsRPos, Color.White);

            spriteBatch.DrawTextWithShadow(font, HeartsLost, heartsLPos, Color.White);
            spriteBatch.DrawTextWithShadow(font, _values[1, 0].ToString(), heartsRPos, Color.White);

            spriteBatch.DrawTextWithShadow(font, EnemiesDefeated, enemiesLPos, Color.White);
            spriteBatch.DrawTextWithShadow(font, _values[2, 0].ToString(), enemiesRPos, Color.White);

            spriteBatch.DrawTextWithShadow(font, Time, timeLPos, Color.White);
            spriteBatch.DrawTextWithShadow(font, FormatTime(_timeValues[0]), timeRPos, Color.White);
        }

        private string FormatTime(TimeSpan time)
        {
            return String.Format("{0}:{1}", (int)time.TotalMinutes, time.Seconds.ToString("D2"));
        }
    }
}
