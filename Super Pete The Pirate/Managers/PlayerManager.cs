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
        public int Ammo => _ammo;

        private int _lives;
        public int Lives => _lives;

        private int _hearts;
        public int Hearts => _hearts;

        private int _coins;
        public int Coins => _coins;

        //--------------------------------------------------
        // Initial data

        public const int InitialAmmo = 5;
        public const int InitialLives = 3;
        public const int InitialHearts = 5;

        //--------------------------------------------------
        // Demo picture

        public bool DemoPicture { get; set; }

        //--------------------------------------------------
        // Stages completed

        private int _stagesCompleted;
        public int StagesCompleted => _stagesCompleted;

        //----------------------//------------------------//

        private PlayerManager()
        {
            _ammo = InitialAmmo;
            _lives = InitialLives;
            _hearts = InitialHearts;
            _coins = 0;
            _stagesCompleted = 0;
            DemoPicture = false;
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
            _coins = coins;
            _stagesCompleted = stagesCompleted;
        }

        public void ResetHeartsAndLives()
        {
            _hearts = InitialHearts;
            _lives = InitialLives;
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
