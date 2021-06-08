using System;
using System.Linq;
using UnityEngine;

namespace Musashi.Player
{
    public partial class PlayerMoveStateMchine
    {
        /// <summary>
        /// プレイヤーが壁歩きしている時の動きを制御するクラス。
        /// プレイヤーがジャンプして、壁に接触したら壁歩き状態に遷移する
        /// </summary>
        [Serializable]
        public class PlayerWallRunState : IState<PlayerMoveStateMchine>
        {
            [SerializeField] float wallMaxDistance = 1;
            [SerializeField] float wallSpeedMultiplier = 1.2f;
            [SerializeField] float minimumHeight = 1.2f;
            [SerializeField] float maxAngleRoll = 20;

            [SerializeField, Range(0.0f, 1.0f)] float normalizedAngleThreshold = 0.1f;

            [SerializeField] float jumpDuration = 1;
            [SerializeField] float wallBouncing = 3;
            [SerializeField] float cameraTransitionDuration = 1;

            [SerializeField] float wallGravityDownForce = 20f;

            float lastTimeJump = 0f;
            Vector3[] directions;
            RaycastHit[] hits;
            Vector3 lastWallNormal;


            public Vector3 GetWallJumpDirection => lastWallNormal * wallBouncing + Vector3.up; 

            private PlayerWallRunState()
            {
                directions = new Vector3[]
                {
                    Vector3.right,
                    Vector3.right + Vector3.forward,
                    Vector3.forward,
                    Vector3.left + Vector3.forward,
                    Vector3.left
                };
            }

            /// <summary>
            /// 最終的にプレイヤーが壁走りできるかどうか決める関数。
            /// Update()で呼ぶこと
            /// </summary>
            /// <param name="owner"></param>
            /// <returns></returns>
            public bool CanWallRun(PlayerMoveStateMchine owner)
            {
                //壁ジャンプしてから(jumpDuration)秒は、壁走りは出来ない。
                if(Time.time < lastTimeJump + jumpDuration)
                {
                    return false;
                }
                return !owner.isGround && VerticalCheck(owner) && CanAttachTheWall(owner) && owner.inputProvider.GetMoveInput.z > 0f;
            }

            /// <summary>
            /// 壁走りできる最低限の高さかどうか判定する関数
            /// </summary>
            /// <param name="owner"></param>
            /// <returns></returns>
            bool VerticalCheck(PlayerMoveStateMchine owner)
            {
                return !Physics.Raycast(owner.transform.position, Vector3.down, minimumHeight);
            }


            public void OnEnter(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> prevState = null)
            {
                Debug.Log(this.ToString());

                //カメラを傾ける
            }

            public void OnExit(PlayerMoveStateMchine owner, IState<PlayerMoveStateMchine> nextState = null)
            {
                //カメラを元に戻す
            }

            public void OnUpdate(PlayerMoveStateMchine owner)
            {
                if (owner.inputProvider.Jump)
                {
                    lastTimeJump = Time.time;
                    owner.stateMachine.ChangeState(owner.JumpState);
                    return;
                }

                if (CanWallRun(owner))
                {
                    CalculateVelocityOnWall(owner);
                }
                else
                {
                    owner.stateMachine.ChangeState(owner.OnGroundState);
                }
            }

            /// <summary>
            /// rayを飛ばして、壁に当たったか確認する関数
            /// </summary>
            /// <param name="owner"></param>
            private void HitCheckTheWall(PlayerMoveStateMchine owner)
            {
                hits = new RaycastHit[directions.Length];
                for (int i = 0; i < directions.Length; i++)
                {
                    Vector3 dir = owner.transform.TransformDirection(directions[i]);
                    Physics.Raycast(owner.transform.position, dir, out hits[i], wallMaxDistance);
                    if (hits[i].collider != null)
                    {
                        Debug.DrawRay(owner.transform.position, dir * hits[i].distance, Color.green);
                    }
                    else
                    {
                        Debug.DrawRay(owner.transform.position, dir * wallMaxDistance, Color.red);
                    }
                }
            }

            /// <summary>
            /// 壁走りできる壁かどうか判別する関数
            /// </summary>
            /// <param name="owner"></param>
            /// <returns></returns>
            private bool CanAttachTheWall(PlayerMoveStateMchine owner)
            {
                var canAttach = false;
                HitCheckTheWall(owner);

                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if (hits.Length > 0)
                {
                    float d = Vector3.Dot(hits[0].normal, Vector3.up);
                    if (d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
                    {
                        canAttach = true;
                    }
                    lastWallNormal = hits[0].normal;
                }
                return canAttach;
            }

            /// <summary>
            /// プレイヤーが壁走りする時の速度を計算する関数
            /// </summary>
            /// <param name="owner"></param>
            private void CalculateVelocityOnWall(PlayerMoveStateMchine owner)
            {
                float vertical = owner.inputProvider.GetMoveInput.z;
                Vector3 alongWall = owner.transform.TransformDirection(Vector3.forward);
                owner.characterVelocity = alongWall * vertical * wallSpeedMultiplier;
                owner.characterVelocity += Vector3.down * wallGravityDownForce * Time.deltaTime;
            }
        }
    }
}