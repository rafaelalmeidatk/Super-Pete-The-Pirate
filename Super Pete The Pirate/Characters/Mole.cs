using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Super_Pete_The_Pirate.Characters
{
    class Mole : Enemy
    {

        public Mole(Texture2D texture) : base(texture)
        {
            _enemyType = EnemyType.Mole;

            // Combat system init
            _hp = 2;
            _damage = 1;
        }
    }
}
