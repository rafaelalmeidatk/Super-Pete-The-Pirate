using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Super_Pete_The_Pirate
{
    public static class ImageManager
    {
        //--------------------------------------------------
        // Cache & Content Manager

        private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();
        private static ContentManager _contentManager = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");

        //----------------------//------------------------//

        public static Texture2D loadSystem(string filename)
        {
            return loadBitmap("imgs/system/" + filename);
        }

        public static Texture2D loadCharacter(string filename)
        {
            return loadBitmap("imgs/characters/" + filename);
        }

        public static Texture2D loadProjectile(string filename)
        {
            return loadBitmap("imgs/projectiles/" + filename);
        }

        public static Texture2D loadScene(string scene, string filename)
        {
            return loadBitmap(String.Format("imgs/scenes/{0}/{1}", scene, filename));
        }

        public static Texture2D loadBitmap(string filename)
        {
            if (!_cache.ContainsKey(filename))
            {
                try
                {
                    _cache[filename] = _contentManager.Load<Texture2D>(filename);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }
            return _cache[filename];
        }
    }
}
