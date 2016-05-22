using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Super_Pete_The_Pirate.Managers
{
    public class SavesManager
    {
        //--------------------------------------------------
        // Singleton variables

        private static SavesManager _instance = null;
        public static readonly object _padlock = new object();

        public static SavesManager Instance
        {
            get
            {
                lock(_padlock)
                {
                    if (_instance == null)
                        _instance = new SavesManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // GameSave struct

        [Serializable]
        public struct GameSave
        {
            public int Ammo;
            public int Lives;
            public int Hearts;
            public int Coins;
            public int StagesCompleted;
        }

        //--------------------------------------------------
        // General variables

        private StorageDevice _storageDevice;
        private string _storageContainerName;
        private int _slot;
        private Action _callback;
        private Action _failCallback;

        //----------------------//------------------------//

        private SavesManager()
        {
            _storageContainerName = "Super Pete The Pirate";
            _slot = 0;
            _callback = null;
            _failCallback = null;
        }

        public void ExecuteSave(int slot, Action callback = null)
        {
            _slot = slot;
            _callback = callback;
            _storageDevice = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, Save, null);
        }

        private void Save(IAsyncResult result)
        {
            _storageDevice = StorageDevice.EndShowSelector(result);
            if (_storageDevice != null && _storageDevice.IsConnected)
            {
                var filename = String.Format("save{0:00}.dat", _slot);
                var player = PlayerManager.Instance;
                GameSave save = new GameSave()
                {
                    Ammo = player.Ammo,
                    Lives = player.Lives,
                    Hearts = player.Hearts,
                    Coins = player.Coins,
                    StagesCompleted = player.StagesCompleted
                };
                IAsyncResult r = _storageDevice.BeginOpenContainer(_storageContainerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = _storageDevice.EndOpenContainer(r);
                if (container.FileExists(filename))
                    container.DeleteFile(filename);
                Stream stream = container.CreateFile(filename);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, save);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
                if (_callback != null)
                    _callback();
            }
        }

        public void ExecuteLoad(int slot, Action sucessCallback, Action failCallback)
        {
            _slot = slot;
            _callback = sucessCallback;
            _failCallback = failCallback;
            _storageDevice = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, Load, null);
        }

        private void Load(IAsyncResult result)
        {
            _storageDevice = StorageDevice.EndShowSelector(result);
            if (_storageDevice != null && _storageDevice.IsConnected)
            {
                var error = false;
                var filename = String.Format("save{0:00}.dat", _slot);
                IAsyncResult r = _storageDevice.BeginOpenContainer(_storageContainerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = _storageDevice.EndOpenContainer(r);
                if (container.FileExists(filename))
                {
                    Stream stream = container.OpenFile(filename, FileMode.Open);
                    try
                    {
                        IFormatter formatter = new BinaryFormatter();
                        GameSave saveData = (GameSave)formatter.Deserialize(stream);
                        stream.Close();
                        container.Dispose();
                        PlayerManager.Instance.SetData(saveData.Ammo, saveData.Lives, saveData.Hearts, saveData.Coins, saveData.StagesCompleted);
                    }
                    catch (Exception)
                    {
                        stream.Close();
                        container.Dispose();
                        container.DeleteFile(filename);
                        error = true;
                    }
                }
                else
                {
                    error = true;
                }

                result.AsyncWaitHandle.Close();

                if (error && _failCallback != null)
                {
                    _failCallback();
                }
                else if (_callback != null)
                {
                    _callback();
                }
            }
        }
    }
}
