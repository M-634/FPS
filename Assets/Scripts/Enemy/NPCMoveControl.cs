using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi.NPC
{
    public static 
        class AnimatorName
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

        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
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
            //Debug.Log(owner.Agent.remainingDistance);
            //いったん0.5f以下になった後remainDistanceが再び0.53fとかになってしまい、想定どうりの動きが出来ないときがある
            //これは、 Idleアニメーションが再生されるときに、位置がずれてしまっていることが原因である
            if (!owner.Agent.pathPending && owner.Agent.remainingDistance < owner.StopOffset)
            {
                isStopping = true;
            }

            if (isStopping)
            {
                StopPoint(owner);
            }

            if (AIHelper.CanSeePlayer(owner.Target,owner.transform,owner.VisitDistance,owner.ViewingAngle,owner.Eye))
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
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {
            owner.Agent.speed = owner.PursueSpeed;
            owner.Agent.isStopped = false;
            owner.Anim.Play(AnimatorName.Run);
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {

        }

        public void OnUpdate(NPCMoveControl owner)
        {
            AIHelper.LookAtPlayer(owner.Target, owner.transform, owner.TurnAroundInterpolationSpeed);
            owner.Agent.SetDestination(owner.Target.position);

            if (AIHelper.CanAttackPlayer(owner.Target, owner.transform, owner.AttackRange))
            {
                owner.StateMacnie.ChangeState(owner.AttackState);
            }

            if (!AIHelper.CanSeePlayer(owner.Target, owner.transform, owner.VisitDistance, owner.ViewingAngle, owner.Eye))
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
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {
            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.Attack);
            Debug.Log("攻撃！");
        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {

        }

        public void OnUpdate(NPCMoveControl owner)
        {

        }
    }


    public class OnDamage : IState<NPCMoveControl>
    {
        public void OnEnter(NPCMoveControl owner, IState<NPCMoveControl> prevState = null)
        {

        }

        public void OnExit(NPCMoveControl owner, IState<NPCMoveControl> nextState = null)
        {

        }

        public void OnUpdate(NPCMoveControl owner)
        {

        }
    }
    #endregion

}
