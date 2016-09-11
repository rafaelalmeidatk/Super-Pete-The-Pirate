using Microsoft.Xna.Framework.Input;

namespace Super_Pete_The_Pirate
{
    public class InputManager
    {
        //--------------------------------------------------
        // Keys control

        private KeyboardState _currentKeyState, _prevKeyState;
        public KeyboardState CurrentKeyState { get { return _currentKeyState; } }
        
        //--------------------------------------------------
        // Singleton instance

        private static InputManager _instance;

        //----------------------//------------------------//

        public static InputManager Instace
        {
            get
            {
                if (_instance == null)
                    _instance = new InputManager();
                return _instance;
            }
        }

        public void Update()
        {
            _prevKeyState = _currentKeyState;
            if (!SceneManager.Instance.IsTransitioning)
                _currentKeyState = Keyboard.GetState();
        }

        public bool KeyPressed(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (_currentKeyState.IsKeyDown(key) && _prevKeyState.IsKeyUp(key))
                    return true;
            }
            return false;
        }

        public bool KeyReleased(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (_currentKeyState.IsKeyUp(key) && _prevKeyState.IsKeyDown(key))
                    return true;
            }
            return false;
        }

        public bool KeyDown(params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                if (_currentKeyState.IsKeyDown(key))
                    return true;
            }
            return false;
        }
    }
}
