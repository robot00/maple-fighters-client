﻿using UnityEngine;

namespace Scripts.UI.Focus
{
    public class FocusStateController : MonoBehaviour
    {
        private FocusState focusState = FocusState.Game;

        public void ChangeFocusState(FocusState focusState)
        {
            this.focusState = focusState;
        }

        public FocusState GetFocusState()
        {
            return focusState;
        }
    }
}