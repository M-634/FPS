using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerMoveStateMchine
    {
        /// <summary>
        /// プレイヤーが地上にいる時の動きを制御するクラス
        /// </summary>
        private class PlayerOnGroundState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                owner.targetCharacterHeight = owner.capsuleHeightStanding;
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {

            }

            /// <summary>
            /// move on ground
            /// </summary>
            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.isGround && owner.inputProvider.Jump)
                {
                    owner.stateMachine.ChangeState(owner.JumpState);
                    return;
                }

                if (!owner.IsSprinting && owner.inputProvider.CanCrouch)
                {
                    owner.stateMachine.ChangeState(owner.CrouchingState);
                    return;
                }

                if (owner.isGround)
                {
                    owner.HandleGroundedMovment(this);
                }
                else
                {
                    //playerがジャンプ以外で空中にいる時は、重力をかけて地面落とす
                    owner.characterVelocity += Vector3.down * owner.gravityDownForce * Time.deltaTime;
                }
            }
        }
    }
}