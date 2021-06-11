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

        [Header("Gizoms表示色")]
        [SerializeField] Color visitDistanceColor;
        [SerializeField] Color attackRangeColor;

        NavMeshAgent agent;
        Animator animator;
        StateMachine<NPCMoveControl> stateMachine;
        #endregion

        //events
        public Action OnEnterAttackEvent;
        public Action OnExitAttackEvent;

        #region Field's property
        public float PatrolSpeed => patrolSpeed;
        public float PursueSpeed => pursueSpeed;
        public Transform[] PatrolPoints => patrolPoints;
        public int PatrolPointsIndex { get; set; }
        public float BreakTime => breakTime;
        public float StopOffset => stopOffset;
        public float VisitDistance => visitDistance;
        public float ViewingAngle => viewingAngle;
        public float AttackRange => attackRange;
        public float AttackCoolTime => attackCoolTime;
        public float TurnAroundInterpolationSpeed => turnAroundInterpolationSpeed;
        public float LastTimeAttacked { get; set; }

        public Transform Eye => eye;
        public Transform Target => target;
        public NavMeshAgent Agent => agent;
        public Animator Anim => animator;
        public StateMachine<NPCMoveControl> StateMacnie => stateMachine;
        #endregion

        #region States
        public IState<NPCMoveControl> IdleState { get; private set; } = new Idle();
        public IState<NPCMoveControl> PatrolState { get; private set; } = new Patrol();
        public IState<NPCMoveControl> PuersueState { get; private set; } = new Pursue();
        public IState<NPCMoveControl> AttackState { get; private set; } = new Attack();
        public IState<NPCMoveControl> OnDamageState { get; private set; } = new OnDamage();
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
        public void OnDamage()
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
    }

    #region State Classes
    public class Idle : IState<NPCMoveControl>
    {
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {
            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.Idle);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {

        }

        public void OnUpdate(NPCMoveControl owner)
        {
            owner.StateMacnie.ChangeState(owner.PatrolState);
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
            if (owner.PatrolPoints.Length == 0) return;

            owner.PatrolPointsIndex = (owner.PatrolPointsIndex + 1) % owner.PatrolPoints.Length;
            owner.Agent.destination = owner.PatrolPoints[owner.PatrolPointsIndex].position;
            owner.Agent.speed = owner.PatrolSpeed;
            owner.Agent.isStopped = false;
            owner.Anim.Play(AnimatorName.Walk);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {

        }

        public void OnUpdate(NPCMoveControl owner)
        {
            if (!owner.Agent.pathPending && owner.Agent.remainingDistance < owner.StopOffset)
            {
                isStopping = true;
            }

            if (isStopping)
            {
                StopPoint(owner);
            }

            if (NPCAIHelper.CanSeePlayer(owner.Target, owner.transform, owner.VisitDistance, owner.ViewingAngle, owner.Eye))
            {
                owner.StateMacnie.ChangeState(owner.PuersueState);
            }
        }

        private void StopPoint(NPCMoveControl owner)
        {
            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.Idle);
            waitTime += Time.deltaTime;

            if (waitTime > owner.BreakTime)
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
            owner.Agent.speed = owner.PursueSpeed;
            owner.Agent.isStopped = false;
            owner.Anim.Play(AnimatorName.Run);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {
            isAngryState = false;
        }

        public void OnUpdate(NPCMoveControl owner)
        {
            NPCAIHelper.LookAtPlayer(owner.Target, owner.transform, owner.TurnAroundInterpolationSpeed);
            owner.Agent.SetDestination(owner.Target.position);

            if (NPCAIHelper.CanAttackPlayer(owner.Target, owner.transform, owner.AttackRange))
            {
                owner.StateMacnie.ChangeState(owner.AttackState);
            }

            if (isAngryState) return;

            if (!NPCAIHelper.CanSeePlayer(owner.Target, owner.transform, owner.VisitDistance, owner.ViewingAngle, owner.Eye))
            {
                owner.StateMacnie.ChangeState(owner.IdleState);
            }
        }
    }

    /// <summary>
    /// 攻撃はアニメーションイベントからメソッドを呼ぶようにすること
    /// </summary>
    public class Attack : IState<NPCMoveControl>
    {
        bool canAttack = true;
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {
            if (owner.OnEnterAttackEvent != null)
            {
                owner.OnEnterAttackEvent.Invoke();
            }

            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.Attack);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {
            if(owner.OnExitAttackEvent != null)
            {
                owner.OnExitAttackEvent.Invoke();
            }
        }

        public void OnUpdate(NPCMoveControl owner)
        {
            NPCAIHelper.LookAtPlayer(owner.Target, owner.transform, owner.TurnAroundInterpolationSpeed);

            //if (!NPCAIHelper.CanSeePlayer(owner.Target, owner.transform, owner.VisitDistance, owner.ViewingAngle, owner.Eye))
            //{
            //    owner.StateMacnie.ChangeState(owner.IdleState);
            //}

            if (!NPCAIHelper.CanAttackPlayer(owner.Target, owner.transform, owner.AttackRange))
            {
                owner.StateMacnie.ChangeState(owner.PuersueState);
            }

            //if (!canAttack) return;

            if (owner.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                //攻撃アニメーションが終了時の処理
                if (owner.Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                {
                    //canAttack = false;
                    owner.Anim.Play(AnimatorName.Idle);
                    //WaitAttackCoolTime(owner);
                    owner.LastTimeAttacked = Time.time;
                }
            }

            if (Time.time >= owner.LastTimeAttacked + owner.AttackCoolTime)
            {
                owner.Anim.Play(AnimatorName.Attack);
            }
        }

        private async void WaitAttackCoolTime(NPCMoveControl owner)
        {
            owner.Anim.Play(AnimatorName.Idle);
            await UniTask.Delay(TimeSpan.FromSeconds(owner.AttackCoolTime), ignoreTimeScale: false);
            owner.Anim.Play(AnimatorName.Attack);
            canAttack = true;
        }
    }

    public class OnDamage : IState<NPCMoveControl>
    {
        IState<NPCMoveControl> prevState;
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {
            this.prevState = prevState;
            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.OnDamage);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {
            prevState = null;
        }

        public void OnUpdate(NPCMoveControl owner)
        {
            if (owner.Anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                if (prevState is Attack)
                {
                    owner.StateMacnie.ChangeState(owner.AttackState);
                }
                else
                {
                    owner.StateMacnie.ChangeState(owner.PuersueState);
                }
            }
        }
    }
    #endregion
}
