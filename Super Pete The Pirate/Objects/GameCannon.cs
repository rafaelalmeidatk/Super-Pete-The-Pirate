using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Super_Pete_The_Pirate.Scenes;
using Super_Pete_The_Pirate.Sprites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Objects
{
    class GameCannon : AnimatedSprite
    {
        //--------------------------------------------------
        // Shot frame

        private const int ShotFrame = 7;

        //--------------------------------------------------
        // Complete

        public bool Complete => Looped;

        //--------------------------------------------------
        // Shot

        private bool _shot;

        //----------------------//------------------------//

        public GameCannon(Texture2D texture, Rectangle[] frames, int delay, int x, int y)
            : base(texture, frames, delay, x, y, false) { }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (CurrentFrame == ShotFrame && !_shot)
            {
                Shot();
            }
        }

        private void Shot()
        {
            _shot = true;
            var sceneMap = (SceneMap)SceneManager.Instance.GetCurrentScene();
            var position = new Vector2(334, Position.Y + 6);
            sceneMap.CreateProjectile("cannonball", position, -7, 0, 1, ProjectileSubject.FromEnemy);
        }
    }
}
