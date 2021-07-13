using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;

namespace Musashi.NPC
{
    /// <summary>
    /// リファクタリングメモ ⇒ ユーティリティークラスとしてまとめる
    /// </summary>
    public static class AnimatorName
    {
        public const string Idle = "Idle";
        public const string Walk = "Walk";
        public const string Run = "Run";
        public const string Attack = "Attack";
        public const string OnDamage = "OnDamage";
        public const string OnDie = "OnDie";
    }

    /// <summary>
    /// プレイヤーを発見したら追いかけ回して弾を放つ。
    /// 平常時は、パスに沿ってパトロールする。
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCMoveControl : MonoBehaviour
    {
        #region Field
        [SerializeField] Transform target;

        [Header("各状態のスピード")]
        [SerializeField] float patrolSpeed = 2f;
        [SerializeField] float pursueSpeed = 5f;

        [Header("パトロール中の各種設定値")]
        [SerializeField] Transform eye;
        [SerializeField] float visitDistance = 10.0f;
        [SerializeField] float viewingAngle = 30.0f;
        [SerializeField] Transform[] patrolPoints;
        /// <summary>目標地点に到達してから次の中継地点へ出発するまでの待ち時間</summary>
        [SerializeField] float breakTime = 2f;
        [SerializeField] float stopOffset = 0.5f;

        [Header("攻撃の各種設定")]
        /// <summary>振り向きの補間スピード </summary>
        [SerializeField, Range(0, 1f)] float turnAroundInterpolationSpeed = 0.1f;
        [SerializeField] float attackRange = 7.0f;
        [SerializeField] float attackCoolTime = 1.0f;

        [Header("ダメージ状態の各種設定")]
        /// <summary>ノックバックするか（ノックバックアニメーションさせるか）判定するフラグ</summary>
        [SerializeField] bool doKnockback = true;

        [Header("Gizoms表示色")]
        [SerializeField] Color visitDistanceColor;
        [SerializeField] Color attackRangeColor;

        //events
        public event Action OnEnterAttackEvent;
        public event Action OnExitAttackEvent;

        private float lastTimeAttacked;
        private int patrolPointsIndex;

        NavMeshAgent agent;
        Animator animator;
        StateMachine<NPCMoveControl> stateMachine;

        #region States
        private readonly IState<NPCMoveControl> IdleState = new Idle();
        private readonly IState<NPCMoveControl> PatrolState = new Patrol();
        private readonly IState<NPCMoveControl> PuersueState = new Pursue();
        private readonly IState<NPCMoveControl> AttackState = new Attack();
        private readonly IState<NPCMoveControl> OnDamageState = new OnDamage();
        #endregion

        #endregion

        #region Method
        private void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            agent.autoBraking = false;

            if (!target)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
            }

            if (!eye)
            {
                eye = this.transform;
            }

            stateMachine = new StateMachine<NPCMoveControl>(this, IdleState);
        }

        private void Update()//Update ⇒async/await
        {
            stateMachine.ExcuteOnUpdate();
        }

        /// <summary>
        /// PathToolのイベントから呼ばれる関数.
        /// パトロールポイントをセットする
        /// </summary>
        /// <param name="points"></param>
        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
        }

        /// <summary>
        /// NPCHealthControlのOnDamageEventから呼ばれる関数
        /// ステートを被ダメージ状態に切り替える
        /// </summary>
        public void ChangeOnDamageState()
        {
            stateMachine.ChangeState(OnDamageState);
        }


#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (stateMachine != null && stateMachine.CurrentState is Attack)
            {
                Handles.color = attackRangeColor;
                Handles.DrawSolidDisc(transform.position, Vector3.up, attackRange);
            }
            else
            {
                Handles.color = visitDistanceColor;
                Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0, -viewingAngle, 0) * transform.forward, viewingAngle * 2, visitDistance);

                Handles.color = attackRangeColor;
                Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0, -viewingAngle, 0) * transform.forward, viewingAngle * 2, attackRange);
            }
        }
#endif
        #endregion

        #region State Classes
        public class Idle : IState<NPCMoveControl>
        {
            public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
            {
                owner.agent.isStopped = true;
                owner.animator.Play(AnimatorName.Idle);
            }

            public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
            {

            }

            public void OnUpdate(NPCMoveControl owner)
            {
                owner.stateMachine.ChangeState(owner.PatrolState);
            }
        }

        public class Patrol : IState<NPCMoveControl>
        {
            float waitTime = 0f;
            bool isStopping = false;
            public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
            {
                GoToNextPoint(owner);
            }

            private void GoToNextPoint(NPCMoveControl owner)
            {
                waitTime = 0;
                isStopping = false;
                if (owner.patrolPoints.Length == 0) return;

                owner.patrolPointsIndex = (owner.patrolPointsIndex + 1) % owner.patrolPoints.Length;
                owner.agent.destination = owner.patrolPoints[owner.patrolPointsIndex].position;
                owner.agent.speed = owner.patrolSpeed;
                owner.agent.isStopped = false;
                owner.animator.Play(AnimatorName.Walk);
            }

            public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
            {

            }

            public void OnUpdate(NPCMoveControl owner)
            {
                if (!owner.agent.pathPending && owner.agent.remainingDistance < owner.stopOffset)
                {
                    isStopping = true;
                }

                if (isStopping)
                {
                    StopPoint(owner);
                }

                if (NPCAIHelper.CanSeePlayer(owner.target, owner.transform, owner.visitDistance, owner.viewingAngle, owner.eye))
                {
                    owner.stateMachine.ChangeState(owner.PuersueState);
                }
            }

            private void StopPoint(NPCMoveControl owner)
            {
                owner.agent.isStopped = true;
                owner.animator.Play(AnimatorName.Idle);
                waitTime += Time.deltaTime;

                if (waitTime > owner.breakTime)
                {
                    GoToNextPoint(owner);
                }
            }
        }

        public class Pursue : IState<NPCMoveControl>
        {
            bool isAngryState = false;//ダメージ受けた後は、プレイヤーを攻撃できる距離まで追い詰める
            public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
            {
                if (prevState is OnDamage)
                {
                    isAngryState = true;
                }
                owner.agent.speed = owner.pursueSpeed;
                owner.agent.isStopped = false;
                owner.animator.Play(AnimatorName.Run);
            }

            public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
            {
                isAngryState = false;
            }

            public void OnUpdate(NPCMoveControl owner)
            {
                NPCAIHelper.LookAtPlayer(owner.target, owner.transform, owner.turnAroundInterpolationSpeed);
                owner.agent.SetDestination(owner.target.position);

                if (NPCAIHelper.CanAttackPlayer(owner.target, owner.transform, owner.attackRange))
                {
                    owner.stateMachine.ChangeState(owner.AttackState);
                }

                if (isAngryState) return;

                if (!NPCAIHelper.CanSeePlayer(owner.target, owner.transform, owner.visitDistance, owner.viewingAngle, owner.eye))
                {
                    owner.stateMachine.ChangeState(owner.IdleState);
                }
            }
        }

        public class Attack : IState<NPCMoveControl>
        {
            public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
            {
                if (owner.OnEnterAttackEvent != null)
                {
                    owner.OnEnterAttackEvent.Invoke();
                }
                owner.agent.isStopped = true;
                owner.animator.Play(AnimatorName.Attack);
            }

            public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
            {
                if (owner.OnExitAttackEvent != null)
                {
                    owner.OnExitAttackEvent.Invoke();
                }
            }

            public void OnUpdate(NPCMoveControl owner)
            {
                NPCAIHelper.LookAtPlayer(owner.target, owner.transform, owner.turnAroundInterpolationSpeed);

                if (!NPCAIHelper.CanAttackPlayer(owner.target, owner.transform, owner.attackRange))
                {
                    owner.stateMachine.ChangeState(owner.PuersueState);
                }

                if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                {
                    //攻撃アニメーションが終了時の処理
                    if (owner.animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                    {
                        owner.animator.Play(AnimatorName.Idle);
                        owner.lastTimeAttacked = Time.time;
                    }
                }

                if (Time.time >= owner.lastTimeAttacked + owner.attackCoolTime)
                {
                    owner.animator.Play(AnimatorName.Attack);
                }
            }
        }

        public class OnDamage : IState<NPCMoveControl>
        {
            IState<NPCMoveControl> prevState;
            public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
            {
                this.prevState = prevState;
                if (owner.doKnockback)
                {
                    owner.agent.isStopped = true;
                    owner.animator.Play(AnimatorName.OnDamage);
                }
            }

            public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
            {
                prevState = null;
            }

            public void OnUpdate(NPCMoveControl owner)
            {
                if (owner.doKnockback)
                {
                    if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
                    {
                        ChangeStateFromDamage(owner);
                    }
                }
                else
                {
                    ChangeStateFromDamage(owner);
                }
            }

            private void ChangeStateFromDamage(NPCMoveControl owner)
            {
                if (prevState is Attack)
                {
                    owner.stateMachine.ChangeState(owner.AttackState);
                }
                else
                {
                    owner.stateMachine.ChangeState(owner.PuersueState);
                }
            }
        }
        #endregion
    }

}
