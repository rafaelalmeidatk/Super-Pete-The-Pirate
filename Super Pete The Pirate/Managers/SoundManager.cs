using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Super_Pete_The_Pirate.Managers
{
    public class SoundManager
    {
        //--------------------------------------------------
        // Content Manager

        private static Dictionary<string, SoundEffect> _seCache = new Dictionary<string, SoundEffect>();
        private static ContentManager _contentManager = new ContentManager(SceneManager.Instance.Content.ServiceProvider, "Content");

        //----------------------//------------------------//

        public static Song loadBgm(string filename)
        {
            return _contentManager.Load<Song>("sounds/bgm/" + filename);
        }

        public static SoundEffect loadSe(string filename)
        {
            if (!_seCache.ContainsKey(filename))
            {
                try
                {
                    _seCache[filename] = _contentManager.Load<SoundEffect>("sounds/se/" + filename);
                }
                catch (Exception ex)
                {
                    _seCache[filename] = null;
                    Debug.WriteLine(ex.ToString());
                }
            }
            return _seCache[filename];
        }
    }
}
