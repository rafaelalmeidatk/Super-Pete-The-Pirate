using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Super_Pete_The_Pirate.Managers
{
    //--------------------------------------------------
    // Input Commands

    public enum InputCommand
    {
        Up,
        Down,
        Left,
        Right,
        Jump,
        Attack,
        Shot,
        A,
        B,
        Confirm,
        Cancel,
        Pause
    }

    public class InputManager
    {
        //--------------------------------------------------
        // Input Commands

        private Dictionary<InputCommand, Keys> _keyCommands;
        private Dictionary<InputCommand, Buttons> _buttonCommands;

        //--------------------------------------------------
        // Keys control

        private KeyboardState _currentKeyState, _prevKeyState;
        private GamePadState _currentPadState, _prevPadState;
        private GamePadCapabilities _padCapabilities;

        //--------------------------------------------------
        // Thumbstick tolerance

        private const float ThumbstickTolerance = 0.5f;

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

        public InputManager()
        {
            _keyCommands = new Dictionary<InputCommand, Keys>
            {
                { InputCommand.Up, Keys.Up },
                { InputCommand.Down, Keys.Down },
                { InputCommand.Left, Keys.Left },
                { InputCommand.Right, Keys.Right },
                { InputCommand.Jump, Keys.C },
                { InputCommand.Attack, Keys.X },
                { InputCommand.Shot, Keys.Z },
                { InputCommand.Confirm, Keys.Enter },
                { InputCommand.Pause, Keys.Escape },
                { InputCommand.Cancel, Keys.Escape },
                { InputCommand.A, Keys.Z },
                { InputCommand.B, Keys.X }
            };
            _buttonCommands = new Dictionary<InputCommand, Buttons>
            {
                { InputCommand.Up, Buttons.LeftThumbstickUp },
                { InputCommand.Down, Buttons.LeftThumbstickDown },
                { InputCommand.Left, Buttons.LeftThumbstickLeft },
                { InputCommand.Right, Buttons.LeftThumbstickRight },
                { InputCommand.Jump, Buttons.A },
                { InputCommand.Attack, Buttons.X },
                { InputCommand.Shot, Buttons.B },
                { InputCommand.Confirm, Buttons.Start },
                { InputCommand.Pause, Buttons.Start },
                { InputCommand.A, Buttons.A },
                { InputCommand.B, Buttons.B },
                { InputCommand.Cancel, Buttons.B },
            };
        }

        public void Update()
        {
            _prevKeyState = _currentKeyState;
            _prevPadState = _currentPadState;
            _padCapabilities = GamePad.GetCapabilities(PlayerIndex.One);

            if (!SceneManager.Instance.IsTransitioning)
            {
                _currentKeyState = Keyboard.GetState();
                _currentPadState = GamePad.GetState(PlayerIndex.One);
            }
        }

        public bool Pressed(params InputCommand[] commands)
        {
            foreach (var command in commands)
            {
                if (KeyPressed(_keyCommands[command]))
                    return true;

                if (IsPadConnected() && ButtonPressed(_buttonCommands[command]))
                    return true;
            }
            return false;
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

        public bool ButtonPressed(params Buttons[] buttons)
        {
            if (_padCapabilities.IsConnected)
            {
                foreach (Buttons button in buttons)
                {
                    if (_currentPadState.IsButtonDown(button) && _prevPadState.IsButtonUp(button))
                        return true;
                }
            }
            return false;
        }

        public bool Released(params InputCommand[] commands)
        {
            foreach (var command in commands)
            {
                if (KeyReleased(_keyCommands[command]))
                    return true;

                if (IsPadConnected() && ButtonReleased(_buttonCommands[command]))
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

        public bool ButtonReleased(params Buttons[] buttons)
        {
            if (_padCapabilities.IsConnected)
            {
                foreach (Buttons button in buttons)
                {
                    if (_currentPadState.IsButtonUp(button) && _prevPadState.IsButtonDown(button))
                        return true;
                }
            }
            return false;
        }

        public bool Down(params InputCommand[] commands)
        {
            foreach (var command in commands)
            {
                if (KeyDown(_keyCommands[command]))
                    return true;

                if (IsPadConnected() && ButtonDown(_buttonCommands[command]))
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

        public bool ButtonDown(params Buttons[] buttons)
        {
            if (_padCapabilities.IsConnected)
            {
                foreach (Buttons button in buttons)
                {
                    if (_currentPadState.IsButtonDown(button))
                        return true;
                }
            }
            return false;
        }

        public Buttons GetPadDirection()
        {
            Vector2 direction = _currentPadState.ThumbSticks.Left;

            float absX = Math.Abs(direction.X);
            float absY = Math.Abs(direction.Y);

            if (absX > absY && absX > ThumbstickTolerance)
            {
                return direction.X > 0 ? Buttons.DPadRight : Buttons.DPadLeft;
            }
            else if (absX < absY && absY > ThumbstickTolerance)
            {
                return direction.Y > 0 ? Buttons.DPadUp : Buttons.DPadDown;
            }
            return 0;
        }

        public bool IsPadConnected()
        {
            return _padCapabilities.IsConnected;
        }

        private bool IsDirectionalCommand(InputCommand command)
        {
            return command == InputCommand.Up || command == InputCommand.Down ||
                command == InputCommand.Left || command == InputCommand.Right;
        }
    }
}
