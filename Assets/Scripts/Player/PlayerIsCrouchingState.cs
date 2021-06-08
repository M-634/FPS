using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerMoveStateMchine
    {
        #endregion

        /// <summary>
        /// プレイヤーのしゃがみ状態を制限するクラス
        /// </summary>
        private class PlayerIsCrouchingState : IState<PlayerMoveStateMchine>
        {
            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());

                owner.targetCharacterHeight = owner.capsuleHeightCrouching;
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {
                owner.inputProvider.CanCrouch = false;
            }

            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.IsSprinting || !owner.inputProvider.CanCrouch || owner.inputProvider.Jump)
                {
                    if (!CheckTheObtaclesAboveThePlayer(owner))
                    {
                        owner.stateMachine.ChangeState(owner.OnGroundState);
                        return;
                    }
                    owner.inputProvider.CanCrouch = true;
                }

                owner.HandleGroundedMovment(this);
            }

            /// <summary>
            /// プレイヤーの頭上にオブジェットがないかチェックする関数
            /// </summary>
            /// <param name="owner"></param>
            /// <returns></returns>
            private bool CheckTheObtaclesAboveThePlayer(PlayerMoveStateMchine owner)
            {
                return Physics.Raycast(owner.GetCapsuleTopHemisphere(owner.controller.height), Vector3.up, owner.controller.height + owner.controller.radius);
            }
        }
    }
}