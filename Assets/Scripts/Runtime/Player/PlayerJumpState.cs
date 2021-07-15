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
            bool hasSecondJumped = false;

            private float SpeedModifierInAir(PlayerCharacterStateMchine owner)
            {
                return owner.isDushJump ? owner.sprintSpeedModifier : 1f;
            }

            public void OnEnter(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> prevState = null)
            {
                //start by canceling out the vertical component of our velocity
                owner.characterVelocity = new Vector3(owner.characterVelocity.x, 0f, owner.characterVelocity.z);

                //make sure prevState and add jump velocity 
                var jumpVelocity = prevState is PlayerWallRunState ? owner.WallRunState.GetWallJumpDirection : Vector3.up;
                owner.characterVelocity += jumpVelocity * owner.jumpForce;

                //play jump sound
                owner.audioSource.Play(owner.jumpSFX);

                //force grounding to false
                owner.isGround = false;
                owner.groundNormal = Vector3.up;

                owner.lastTimeFirstJumped = Time.time;
            }

            public void OnExit(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> nextState = null)
            {
                hasSecondJumped = false;
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

                if (CanSecondJump(owner))
                {
                    CalcSecondJump(owner);
                }


                //add air acceleration
                owner.characterVelocity += owner.WorldSpaceMoveInput * owner.accelerationSpeedInAir * Time.deltaTime;

                //limit air speed to maximum, but only horizontally
                float verticalVelocity = owner.characterVelocity.y;
                Vector3 horizontalVelocity = Vector3.ProjectOnPlane(owner.characterVelocity, Vector3.up);
                horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, owner.maxSpeedInAir * SpeedModifierInAir(owner));
                owner.characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

                // apply the gravity to the velocity
                owner.characterVelocity += Vector3.down * owner.gravityDownForce * Time.deltaTime;
            }

            private bool CanSecondJump(PlayerCharacterStateMchine owner)
            {
                return !hasSecondJumped && Time.time > owner.lastTimeFirstJumped + owner.firstJumpDuration && owner.inputProvider.Jump;
            }

            /// <summary>
            /// 2回目のジャンプをする時に呼び出す関数
            /// </summary>
            private void CalcSecondJump(PlayerCharacterStateMchine owner)
            {
                owner.characterVelocity += Vector3.up * owner.jumpForce * owner.secondJumpModifier;
                owner.isDushJump = false;
                hasSecondJumped = true;
            }
        }
    }
}