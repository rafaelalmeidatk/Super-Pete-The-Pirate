using System;
using System.Collections.Generic;
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

        //--------------------------------------------------
        // Stages completed

        private int _stagesCompleted;
        public int StagesCompleted { get { return _stagesCompleted; } }

        //--------------------------------------------------
        // Initialized?

        private bool _initialized;
        public bool Initialized { get { return _initialized; } }

        //----------------------//------------------------//

        public PlayerManager()
        {
            _initialized = false;

            _ammo = 0;
            _lives = 0;
            _hearts = 0;
            _coins = 0;

            _stagesCompleted = 3;
        }

        public void AddAmmo(int amount)
        {
            _ammo += amount;
        }

        public void AddLives(int ammount)
        {
            _lives += ammount;
        }

        public void AddHearts(int amount)
        {
            _hearts += amount;
        }

        public void AddCoins(int amount)
        {
            _coins += amount;
        }

        public void CompleteStage()
        {
            _stagesCompleted++;
        }
    }
}
