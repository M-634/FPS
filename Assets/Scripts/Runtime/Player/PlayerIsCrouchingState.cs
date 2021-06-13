using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerCharacterStateMchine
    {
   
        /// <summary>
        /// プレイヤーのしゃがみ状態を制限するクラス
        /// </summary>
        private class PlayerIsCrouchingState : IState<PlayerCharacterStateMchine>
        {
            public void OnEnter(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> prevState = null)
            {
                owner.targetCharacterHeight = owner.capsuleHeightCrouching;
            }

            public void OnExit(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> nextState = null)
            {
                owner.inputProvider.CanCrouch = false;
            }

            public void OnUpdate(PlayerCharacterStateMchine owner)
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
            private bool CheckTheObtaclesAboveThePlayer(PlayerCharacterStateMchine owner)
            {
                return Physics.Raycast(owner.GetCapsuleTopHemisphere(owner.controller.height), Vector3.up, owner.controller.height + owner.controller.radius);
            }
        }
    }
}