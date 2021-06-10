using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerCharacterStateMchine
    {

        /// <summary>
        /// プレイヤーのジャンプ時の動きを制御するクラス
        /// </summary>
        private class PlayerJumpState : IState<PlayerCharacterStateMchine>
        {
            public void OnEnter(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> prevState = null)
            {
                //if prevState is CrouchingState, cancel crouching; 

                //start by canceling out the vertical component of our velocity
                owner.characterVelocity = new Vector3(owner.characterVelocity.x, 0f, owner.characterVelocity.z);

                //prevState = WallRunState or OnGroundState によって加える力を分ける

                //make sure prevState and add jump velocity 
                var jumpVelocity = prevState is PlayerWallRunState ? owner.WallRunState.GetWallJumpDirection : Vector3.up;
                owner.characterVelocity += jumpVelocity * owner.jumpForce;

                //play jump sound
                owner.audioSource.Play(owner.jumpSFX);

                //force grounding to false
                owner.isGround = false;
                owner.groundNormal = Vector3.up;

                owner.lastTimeJumped = Time.time;
            }

            public void OnExit(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> nextState = null)
            {

            }

            /// <summary>
            /// move in air
            /// </summary>
            public void OnUpdate(PlayerCharacterStateMchine owner)
            {
                if (owner.isGround)
                {
                    owner.audioSource.Play(owner.landSFX);
                    owner.stateMachine.ChangeState(owner.OnGroundState);
                    return;
                }

                if (owner.WallRunState.CanWallRun(owner))
                {
                    owner.stateMachine.ChangeState(owner.WallRunState);
                    return;
                }

             

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