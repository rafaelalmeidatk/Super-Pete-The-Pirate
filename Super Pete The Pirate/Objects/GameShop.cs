using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Super_Pete_The_Pirate.Managers;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Objects
{
    enum GameShopType {
        Hearts,
        Lives,
        Ammo,
        None
    }

    class GameShop
    {
        //--------------------------------------------------
        // Shop texture and Arrows texture

        private Texture2D _shopTexture;
        private Texture2D _arrowsTexture;

        //--------------------------------------------------
        // Shop type

        private GameShopType _shopType;

        //--------------------------------------------------
        // Shop position and Arrow position

        private Vector2 _position;
        private Vector2 _arrowPosition;

        //--------------------------------------------------
        // Arrows sprites

        private Rectangle[] _arrowSprites;
        private const int ArrowDefault = 0;
        private const int ArrowClick = 1;
        private const int ArrowDeny = 2;
        private const int ArrowWidth = 15;

        //--------------------------------------------------
        // Arrow state

        private int _arrowState;

        //--------------------------------------------------
        // Is Active

        private bool _isActive;
        public bool IsActive { get { return _isActive; } }

        //--------------------------------------------------
        // Click timer

        private int _clickTimer;

        //--------------------------------------------------
        // Prices

        private Dictionary<GameShopType, int> _prices;

        //--------------------------------------------------
        // Buy SE

        private SoundEffect _buySe;

        //--------------------------------------------------
        // Bounding Rectangle

        public Rectangle BoundingRectangle
        {
            get
            {
                return new Rectangle(_position.ToPoint(), new Point(_shopTexture.Width, _shopTexture.Height));
            }
        }

        //----------------------//------------------------//        


        public GameShop(GameShopType type, Texture2D shopTexture, Texture2D arrowsTexture, Vector2 position)
        {
            // Default init
            _shopTexture = shopTexture;
            _arrowsTexture = arrowsTexture;
            _shopType = type;
            _position = position - (Vector2.UnitY * shopTexture.Height);
            _arrowPosition = new Vector2(_position.X + (_shopTexture.Width - ArrowWidth) / 2,
                _position.Y - _arrowsTexture.Height - 5);

            // Arrows init
            _arrowState = 0;
            _arrowSprites = new Rectangle[3];
            _arrowSprites[ArrowDefault] = new Rectangle(0, 0, 15, 16);
            _arrowSprites[ArrowClick] = new Rectangle(15, 0, 15, 16);
            _arrowSprites[ArrowDeny] = new Rectangle(30, 0, 15, 16);

            // Prices init
            _prices = new Dictionary<GameShopType, int>()
            {
                { GameShopType.Ammo, 10 },
                { GameShopType.Hearts, 15 },
                { GameShopType.Lives, 15 }
            };

            // Mechanics init
            _isActive = false;
            _clickTimer = 0;

            // SE init
            _buySe = SoundManager.LoadSe("Buy");
        }

        public void SetActive(bool active)
        {
            _isActive = active;
        }

        public void SetArrowDenyState()
        {
            _arrowState = ArrowDeny;
        }

        public void SetArrowNormalState()
        {
            _arrowState = ArrowDefault;
        }

        public bool IsDenied()
        {
            return _arrowState == ArrowDeny;
        }

        public bool NeedDeny()
        {
            if (_shopType == GameShopType.Hearts)
            {
                return PlayerManager.Instance.Hearts >= 5;
            }
            return false;
        }

        public void Update(GameTime gameTime)
        {
            if (_clickTimer > 0)
            {
                _clickTimer--;
                if (_clickTimer <= 0)
                {
                    _clickTimer = 0;
                    _arrowState = ArrowDefault;
                }
            }

            if (_isActive && _arrowState == ArrowDefault && InputManager.Instace.KeyPressed(Keys.Up)) {
                ProcessBuy();
            }
        }

        private void ProcessBuy()
        {
            var playerManager = PlayerManager.Instance;
            if (playerManager.Coins >= _prices[_shopType])
            {
                playerManager.AddCoins(-_prices[_shopType]);
                switch (_shopType)
                {
                    case GameShopType.Ammo:
                        playerManager.AddAmmo(1);
                        break;
                    case GameShopType.Hearts:
                        playerManager.AddHearts(1);
                        break;
                    case GameShopType.Lives:
                        playerManager.AddLives(1);
                        break;
                }
                PerformClick();
            }
        }

        private void PerformClick()
        {
            _arrowState = ArrowClick;
            _clickTimer = 5;
            _buySe.PlaySafe();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_shopTexture, _position, Color.White);
            if (_isActive)
                spriteBatch.Draw(_arrowsTexture, _arrowPosition, _arrowSprites[_arrowState], Color.White);
        }
    }
}
