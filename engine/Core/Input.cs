/**
 * Input.cs
 * 
 * Keyboard, Xbox 360 Controller and Mouse input Implementation.
 * 
 * @file
 * @author YongChul [Chris] Jin / id: anecia
 *
 * Copyright (C) XNA Naver cafe, 2009. All rights reserved.
 * $Id: $
 */

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Diagnostics;

namespace Galaxy.Core
{
    public static class Input
    {
        /* Xbox 360 Controller buttons */
        public enum GamePadButtons
        {
            Start,
            Back,
            A,
            B,
            X,
            Y,
            Up,
            Down,
            Left,
            Right,
            LeftTrigger,
            LeftShoulder,
            RightTrigger,
            RightShoulder,
            LeftBumper = LeftShoulder,
            RightBumper = RightShoulder,
            LB = LeftBumper,
            RB = RightBumper
        };

        /* Keyboard keys are defined as Keys in Microsoft.Xna.Framework.Keys.cs,
         * so we just use the Keys. */

        static GamePad  gamePad   = new GamePad();
        static Keyboard keyboard  = new Keyboard();
        static Mouse    mouse     = new Mouse();

        public static void Update()
        {
            keyboard.PreviousState = keyboard.CurrentState;
            keyboard.CurrentState  = Microsoft.Xna.Framework.Input.
                                        Keyboard.GetState();

            gamePad.PreviousState  = gamePad.CurrentState;
            gamePad.CurrentState   = Microsoft.Xna.Framework.Input.
                                        GamePad.GetState(PlayerIndex.One);
        }


        #region Xbox 360 Controller

        /**
         * Returns whether specified input device buttons are pressed.
         * 
         * @param button    Buttons to query. Specify a single button,
         *                  or combine multiple buttons using.
         * 
         * @return  true if all specified buttons are pressed,
         *          false otherwise.
         */
        public static bool IsButtonDown(Buttons button)
        {
            return gamePad.CurrentState.IsButtonDown(button);
        }

        /**
         * Returns whether specified input device buttons are not pressed.
         * 
         * @param button    Buttons to query. Specify a single button,
         *                  or combine multiple buttons using.
         * 
         * @return  true if all specified buttons are up (not pressed),
         *          false otherwise.
         */
        public static bool IsButtonUp(Buttons button)
        {
            return gamePad.CurrentState.IsButtonUp(button);
        }

        /**
         * Detects whether a specified button of Xbox 360 Controller has been
         * pressed.
         * 
         * @param button   Enumerated value that specifies the button to query.
         * 
         * @return  true if the button specified by button has just been pressed,
         *          false otherwise.
         */
        public static bool IsButtonPressed(Buttons button)
        {
            return (gamePad.PreviousState.IsButtonUp(button) &&
                                gamePad.CurrentState.IsButtonDown(button));
        }

        /**
         * Detects whether a specified button of Xbox 360 Controller has been
         * releasedd.
         * 
         * @param button   Enumerated value that specifies the button to query.
         * 
         * @return  true if the button specified by button has just been
         *          released, false otherwise.
         */
        public static bool IsButtonReleased(Buttons button)
        {
            return (gamePad.PreviousState.IsButtonDown(button) &&
                                gamePad.CurrentState.IsButtonUp(button));
        }
        
        private sealed class GamePad
        {
            private List<GamePadButtons> gamePadButtons;

            /* The state of the Xbox 360 Controller buttons as of the last
             * update. */
            private GamePadState currentState;
            public GamePadState CurrentState
            {
                get { return currentState; }
                set { currentState = value; }
            }

            /* The state of the Xbox 360 Controller buttons as of the
             * previous update. */
            private GamePadState previousState;
            public GamePadState PreviousState
            {
                get { return previousState; }
                set { previousState = value; }
            }

            public GamePad()
            {
                gamePadButtons = new List<GamePadButtons>();
            }
        };

        #endregion

        #region Keyboard

        /**
         * Returns whether a specified key is currently being pressed.
         * 
         * @param key   Enumerated value that specifies the key to query.
         * 
         * @return  true if the key specified by key is being held down,
         *          false otherwise.
         */
        public static bool IsKeyDown(Keys key)
        {
            return keyboard.CurrentState.IsKeyDown(key);
        }

        /**
         * Returns whether a specified key is currently not pressed.
         * 
         * @param key   Enumerated value that specifies the key to query.
         * 
         * @return  true if the key specified by key is not pressed,
         *          false otherwise.
         */
        public static bool IsKeyUp(Keys key)
        {
            return keyboard.CurrentState.IsKeyUp(key);
        }

        /**
         * Detects whether a specified key has been pressed.
         * 
         * @param key   Enumerated value that specifies the key to query.
         * 
         * @return  true if the key specified by key has just been pressed,
         *          false otherwise.
         */
        public static bool IsKeyPressed(Keys key)
        {
            return (keyboard.PreviousState.IsKeyUp(key) &&
                                keyboard.CurrentState.IsKeyDown(key));
        }

        /**
         * Detects whether a specified key has been releasedd.
         * 
         * @param key   Enumerated value that specifies the key to query.
         * 
         * @return  true if the key specified by key has just been released,
         *          false otherwise.
         */
        public static bool IsKeyReleased(Keys key)
        {
            return (keyboard.PreviousState.IsKeyDown(key) &&
                                keyboard.CurrentState.IsKeyUp(key));
        }


        private sealed class Keyboard
        {
            public List<Keys> keyboardKeys;

            /* The state of the keyboard as of the last update. */
            private KeyboardState currentState;
            public KeyboardState CurrentState
            {
                get { return currentState; }
                set { currentState = value; }
            }

            /* The state of the keyboard as of the previous update. */
            private KeyboardState previousState;
            public KeyboardState PreviousState
            {
                get { return previousState; }
                set { previousState = value; }
            }

            public Keyboard()
            {
                keyboardKeys = new List<Keys>();
            }
        };

        #endregion

        #region Mouse

        private sealed class Mouse
        {
        };

        #endregion Mouse
    };
}
