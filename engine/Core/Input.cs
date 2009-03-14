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
            LeftTop,
            LeftBottom,
            RightTop,
            RightBottom,
            LT = LeftTop,
            LB = LeftBottom,
            RT = RightTop,
            RB = RightBottom
        };

        /* Keyboard keys are defined as Keys in Microsoft.Xna.Framework.Keys.cs,
         * so we just use the Keys. */

        static GamePad  gamePad   = new GamePad();
        static Keyboard keyboard  = new Keyboard();
        static Mouse    mouse     = new Mouse();

        public static void Update()
        {
            keyboard.PreviousState = keyboard.CurrentState;
            keyboard.CurrentState  = Microsoft.Xna.Framework.Input.Keyboard.GetState();
        }

        public static bool IsKeyPressed(Keys key)
        {
            return keyboard.CurrentState.IsKeyDown(key);
        }

        #region Xbox 360 Controller
        
        private class GamePad
        {
            private List<GamePadButtons> gamePadButtons;
            public GamePad()
            {
                gamePadButtons = new List<GamePadButtons>();
            }
        };

        #endregion

        #region Keyboard

        private class Keyboard
        {
            public List<Keys> keyboardKeys;

            /* The state of the keyboard as of the last update. */
            private static KeyboardState currentState;
            public KeyboardState CurrentState
            {
                get { return currentState; }
                set { currentState = value; }
            }

            /* The state of the keyboard as of the previous update. */
            private static KeyboardState previousState;
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

        private class Mouse
        {
        };

        #endregion Mouse
    };
}
