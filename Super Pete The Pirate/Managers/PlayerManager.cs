using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Managers
{
    class PlayerManager
    {
        //--------------------------------------------------
        // Singleton variables

        private static PlayerManager _instance = null;
        private static readonly object _padlock = new object();

        public static PlayerManager Instance
        {
            get {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new PlayerManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // Player info

        private int _ammo;
        public int Ammo { get { return _ammo; } }

        private int _lives;
        public int Lives { get { return _lives; } }

        private int _hearts;
        public int Hearts { get { return _hearts; } }

        private int _coins;
        public int Coins { get { return _coins; } }

        public const int InitialAmmo = 5;
        public const int InitialLives = 1;
        public const int InitialHearts = 100;

        //--------------------------------------------------
        // Stages completed

        private int _stagesCompleted;
        public int StagesCompleted { get { return _stagesCompleted; } }

        //----------------------//------------------------//

        private PlayerManager()
        {
            _ammo = InitialAmmo;
            _lives = InitialLives;
            _hearts = InitialHearts;
            _coins = 250;
            _stagesCompleted = 0;
        }

        public void CreateNewGame()
        {
            SetData(InitialAmmo, InitialLives, InitialHearts, 0, 0);
        }

        public void SetData(int ammo, int lives, int hearts, int coins, int stagesCompleted)
        {
            _ammo = ammo;
            _lives = lives;
            _hearts = hearts;
            _stagesCompleted = stagesCompleted;
        }

        public void HandleRespawn()
        {
            _lives--;
            _hearts = InitialHearts;
        }

        public void AddAmmo(int amount)
        {
            _ammo += amount;
        }

        public void SetAmmo(int ammo)
        {
            _ammo = ammo;
        }

        public void AddLives(int amount)
        {
            _lives += amount;
        }

        public void AddHearts(int amount)
        {
            _hearts += amount;
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
        }

        public void SetCoins(int coins)
        {
            _coins = coins;
        }

        public void CompleteStage()
        {
            if (_stagesCompleted < 5)
                _stagesCompleted++;
        }
    }
}
