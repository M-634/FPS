using System;
using System.Linq;
using UnityEngine;
using DG.Tweening;

namespace Musashi.Player
{
    public partial class PlayerCharacterStateMchine
    {
        /// <summary>
        /// プレイヤーが壁歩きしている時の動きを制御するクラス。
        /// プレイヤーがジャンプして、壁に接触したら壁歩き状態に遷移する
        /// </summary>
        [Serializable]
        public class PlayerWallRunState : IState<PlayerCharacterStateMchine>
        {
            [SerializeField] float wallMaxDistance = 1f;
            [SerializeField] float wallSpeedMultiplier = 1.2f;
            [SerializeField] float minimumHeight = 1.2f;
            [SerializeField] float maxAngleRoll = 20f;

            [SerializeField, Range(0.0f, 1.0f)] float normalizedAngleThreshold = 0.1f;

            [SerializeField] float wallJumpDuration = 1f;
            [SerializeField] float wallBouncing = 3f;
            [SerializeField] float cameraTransitionDuration = 1f;

            [SerializeField] float wallGravityDownForce = 20f;

            bool isWallRunning;
            float elapsedTimeSinceWallAttach;
            float elapsedTimeSinceWallDettach;
            float lastTimeWallJumped = 0f;
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
            public bool CanWallRun(PlayerCharacterStateMchine owner)
            {
                //壁ジャンプしてから(jumpDuration)秒は、壁走りは出来ない。
                if (Time.time < lastTimeWallJumped + wallJumpDuration)
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
            bool VerticalCheck(PlayerCharacterStateMchine owner)
            {
                return !Physics.Raycast(owner.transform.position, Vector3.down, minimumHeight);
            }

            public void OnEnter(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> prevState = null)
            {
                isWallRunning = true;
                elapsedTimeSinceWallDettach = 0f;
            }

            public void OnExit(PlayerCharacterStateMchine owner, IState<PlayerCharacterStateMchine> nextState = null)
            {
                isWallRunning = false;
                elapsedTimeSinceWallAttach = 0f;
            }

            public void OnUpdate(PlayerCharacterStateMchine owner)
            {
                if (owner.inputProvider.Jump)
                {
                    lastTimeWallJumped = Time.time;
                    owner.stateMachine.ChangeState(owner.JumpState);
                    return;
                }

                if (CanWallRun(owner))
                {
                    MoveOnWall(owner);
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
            private void HitCheckTheWall(PlayerCharacterStateMchine owner)
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
            private bool CanAttachTheWall(PlayerCharacterStateMchine owner)
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
            /// プレイヤーが壁走りする時の処理をする関数
            /// </summary>
            /// <param name="owner"></param>
            private void MoveOnWall(PlayerCharacterStateMchine owner)
            {
                float vertical = owner.inputProvider.GetMoveInput.z;
                Vector3 alongWall = owner.transform.TransformDirection(Vector3.forward);
                owner.characterVelocity = alongWall * vertical * wallSpeedMultiplier;
                owner.characterVelocity += Vector3.down * wallGravityDownForce * Time.deltaTime;
            }


            /// <summary>
            /// 壁の法線とプレイヤーの進行方向から、カメラをプレイヤーの進行方向軸を基準にカメラの傾ける角度を計算する関数。
            /// 壁走りしていない時は、0を返す。
            /// </summary>
            /// <param name="owner"></param>
            public float GetCameraAngle(PlayerCharacterStateMchine owner)
            {
                float cameraAngle = owner.playerCamera.transform.eulerAngles.z;
                float targetAngle = 0f;
                if (isWallRunning)
                {
                    Vector3 cross = Vector3.Cross(lastWallNormal, owner.transform.forward);
                    float dot = Vector3.Dot(cross, owner.transform.up);//dot > 0 左回転, dot < 0  右回転 
                    targetAngle = maxAngleRoll;
                    if (dot < 0)
                    {
                        targetAngle *= -1;
                    }
                    elapsedTimeSinceWallAttach += Time.deltaTime;
                }
                else
                {
                    elapsedTimeSinceWallDettach += Time.deltaTime;
                }
                return Mathf.LerpAngle(cameraAngle, targetAngle, Mathf.Max(elapsedTimeSinceWallAttach, elapsedTimeSinceWallDettach) / cameraTransitionDuration);
            }
        }
    }
}