using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Super_Pete_The_Pirate.Managers
{
    class SettingsManager
    {
        //--------------------------------------------------
        // Singleton variables

        private static SettingsManager _instance = null;
        private static readonly object _padlock = new object();

        public static SettingsManager Instance
        {
            get
            {
                lock (_padlock)
                {
                    if (_instance == null)
                        _instance = new SettingsManager();
                    return _instance;
                }
            }
        }

        //--------------------------------------------------
        // Settings

        [Serializable]
        public struct Settings
        {
            public float BGMVolume;
            public float SEVolume;
            public bool WindowedMode;
        }

        private Settings _gameSettings;
        public Settings GameSettings => _gameSettings;

        //--------------------------------------------------
        // General variables

        private StorageDevice _storageDevice;
        private const string StorageContainerName = "Super Pete The Pirate";
        private const string Filename = "settings.dat";

        //----------------------//------------------------//

        public SettingsManager()
        {
            _gameSettings = CreateNewSettings();
        }


        private Settings CreateNewSettings()
        {
            return new Settings
            {
                BGMVolume = 1.0f,
                SEVolume = 1.0f,
                WindowedMode = true
            };
        }

        public void AddBGMVolume(float ammount)
        {
            _gameSettings.BGMVolume += ammount;
            _gameSettings.BGMVolume = MathHelper.Clamp(_gameSettings.BGMVolume, 0.0f, 1.0f);
            SoundManager.SetBgmVolume(_gameSettings.BGMVolume);
        }

        public void AddSEVolume(float ammout)
        {
            _gameSettings.SEVolume += ammout;
            _gameSettings.SEVolume = MathHelper.Clamp(_gameSettings.SEVolume, 0.0f, 1.0f);
            SoundManager.SetSeVolume(_gameSettings.SEVolume);
        }

        public void SetWindowedMode(bool isWindowed)
        {
            _gameSettings.WindowedMode = isWindowed;
            SceneManager.Instance.SetFullscreen(!isWindowed);
        }

        private void OnSettingsLoaded()
        {
            SoundManager.SetBgmVolume(_gameSettings.BGMVolume);
            SoundManager.SetSeVolume(_gameSettings.SEVolume);

            if (!_gameSettings.WindowedMode)
            {
                SceneManager.Instance.RequestFullscreen();
            }
        }

        public void SaveSettings()
        {
            _storageDevice = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, Save, null);
        }

        private void Save(IAsyncResult result)
        {
            _storageDevice = StorageDevice.EndShowSelector(result);
            if (_storageDevice != null && _storageDevice.IsConnected)
            {
                var player = PlayerManager.Instance;
                IAsyncResult r = _storageDevice.BeginOpenContainer(StorageContainerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = _storageDevice.EndOpenContainer(r);
                if (container.FileExists(Filename))
                    container.DeleteFile(Filename);
                Stream stream = container.CreateFile(Filename);
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, _gameSettings);
                stream.Close();
                container.Dispose();
                result.AsyncWaitHandle.Close();
            }
        }

        public void LoadSettings()
        {
            _storageDevice = null;
            StorageDevice.BeginShowSelector(PlayerIndex.One, Load, null);
        }

        private void Load(IAsyncResult result)
        {
            _storageDevice = StorageDevice.EndShowSelector(result);
            if (_storageDevice != null && _storageDevice.IsConnected)
            {
                _gameSettings = CreateNewSettings();
                IAsyncResult r = _storageDevice.BeginOpenContainer(StorageContainerName, null, null);
                result.AsyncWaitHandle.WaitOne();
                StorageContainer container = _storageDevice.EndOpenContainer(r);
                if (container.FileExists(Filename))
                {
                    Stream stream = container.OpenFile(Filename, FileMode.Open);
                    try
                    {
                        IFormatter formatter = new BinaryFormatter();
                        _gameSettings = (Settings)formatter.Deserialize(stream);
                        stream.Close();
                        container.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        stream.Close();
                        container.Dispose();
                        container.DeleteFile(Filename);
                    }
                }

                result.AsyncWaitHandle.Close();

                OnSettingsLoaded();
            }
        }
    }
}
