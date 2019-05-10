﻿using Game.Common;
using Scripts.UI.Controllers;
using UnityEngine;

namespace Scripts.Gameplay.Actors
{
    public class PlayerIdleState : IPlayerStateBehaviour
    {
        private readonly PlayerController playerController;
        private readonly FocusStateController focusStateController;
        private readonly Rigidbody2D rigidbody2D;

        public PlayerIdleState(
            PlayerController playerController,
            FocusStateController focusStateController)
        {
            this.playerController = playerController;
            this.focusStateController = focusStateController;

            var collider = playerController.GetComponent<Collider2D>();
            rigidbody2D = collider.attachedRigidbody;
        }

        public void OnStateEnter()
        {
            // Left blank intentionally
        }

        public void OnStateUpdate()
        {
            if (playerController.IsGrounded())
            {
                if (IsUnFocused())
                {
                    return;
                }

                if (IsJumpKeyClicked())
                {
                    playerController.ChangePlayerState(PlayerState.Jumping);
                    return;
                }

                if (IsMove())
                {
                    playerController.ChangePlayerState(PlayerState.Moving);
                    return;
                }

                rigidbody2D.velocity = Vector2.zero;
            }
            else
            {
                playerController.ChangePlayerState(PlayerState.Falling);
            }
        }

        public void OnStateFixedUpdate()
        {
            // Left blank intentionally
        }

        public void OnStateExit()
        {
            // Left blank intentionally
        }

        private bool IsUnFocused()
        {
            if (focusStateController != null 
                && focusStateController.GetFocusState() != FocusState.Game)
            {
                return true;
            }

            return false;
        }

        private bool IsJumpKeyClicked()
        {
            var jumpKey = playerController.Configuration.JumpKey;
            if (Input.GetKeyDown(jumpKey))
            {
                return true;
            }

            return false;
        }

        private bool IsMove()
        {
            // TODO: Move to the configuration
            var horizontal = Input.GetAxisRaw("Horizontal");
            if (Mathf.Abs(horizontal) > 0)
            {
                return true;
            }

            return false;
        }
    }
}