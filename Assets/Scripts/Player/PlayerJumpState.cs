﻿using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerMoveStateMchine
    {

        /// <summary>
        /// プレイヤーのジャンプ時の動きを制御するクラス
        /// </summary>
        private class PlayerJumpState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());
                //if prevState is CrouchingState, cancel crouching; 

                //start by canceling out the vertical component of our velocity
                owner.characterVelocity = new Vector3(owner.characterVelocity.x, 0f, owner.characterVelocity.z);

                //prevState = WallRunState or OnGroundState によって加える力を分ける

                //then,add the jumpSpeed value upwards
                owner.characterVelocity += Vector3.up * owner.jumpForce;

                //play jump sound

                //force grounding to false
                owner.isGround = false;
                owner.groundNormal = Vector3.up;

                owner.lastTimeJumped = Time.time;
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {

            }

            /// <summary>
            /// move in air
            /// </summary>
            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.isGround)
                {
                    owner.stateMachine.ChangeState(owner.OnGroundState);
                    return;
                }

                //check can or can't wall run

                //add air acceleration
                owner.characterVelocity += owner.WorldSpaceMoveInput * owner.accelerationSpeedInAir * Time.deltaTime;

                //limit air speed to maximum, but only horizontally
                float verticalVelocity = owner.characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(owner.characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, owner.maxSpeedInAir * owner.SpeedModifier);
                owner.characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                owner.characterVelocity += Vector3.down * owner.gravityDownForce * Time.deltaTime;
            }
        }
    }
}