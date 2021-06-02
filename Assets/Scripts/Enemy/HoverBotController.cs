using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi.Enemy
{
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
    public class HoverBotController : MonoBehaviour
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
        [SerializeField] float attackRange = 7.0f;
        [SerializeField] Transform[] patrolPoints;
        /// <summary>目標地点に到達してから次の中継地点へ出発するまでの待ち時間</summary>
        [SerializeField] float breakTime = 2f;
        [SerializeField] float stopOffset = 0.5f;

        [Header("攻撃の各種設定")]
        /// <summary>振り向きの補間スピード </summary>
        [SerializeField, Range(0, 1f)] float turnAroundInterpolationSpeed = 0.1f;

        [Header("Gizoms表示色")]
        [SerializeField] Color visitDistanceColor;
        [SerializeField] Color attackRangeColor;

        NavMeshAgent agent;
        Animator animator;
        StateMachine<HoverBotController> stateMachine;
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
        public float TurnAroundInterpolationSpeed => turnAroundInterpolationSpeed;

        public Transform Eye => eye;
        public Transform Target => target;
        public NavMeshAgent Agent => agent;
        public Animator Anim => animator;
        public StateMachine<HoverBotController> StateMacnie => stateMachine;
        #endregion


        #region States
        public IState<HoverBotController> Idle { get; private set; } = new Idle();
        public IState<HoverBotController> Patrol { get; private set; } = new Patrol();
        public IState<HoverBotController> Puersue { get; private set; } = new Pursue();
        public IState<HoverBotController> Attack { get; private set; } = new Attack();
        public IState<HoverBotController> OnDamage { get; private set; } = new OnDamage();
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

            stateMachine = new StateMachine<HoverBotController>(this, Idle);
        }

        private void Update()
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
    public class Idle : IState<HoverBotController>
    {
        public void OnEnter(HoverBotController owner, IState<HoverBotController> prevState = null)
        {
            owner.Agent.isStopped = true;
            owner.Anim.Play(AnimatorName.Idle);
        }

        public void OnExit(HoverBotController owner, IState<HoverBotController> nextState = null)
        {

        }

        public void OnUpdate(HoverBotController owner)
        {
            owner.StateMacnie.ChangeState(owner.Patrol);
        }
    }

    public class Patrol : IState<HoverBotController>
    {
        float waitTime = 0f;
        bool isStopping = false;
        public void OnEnter(HoverBotController owner, IState<HoverBotController> prevState = null)
        {
            GoToNextPoint(owner);
        }

        private void GoToNextPoint(HoverBotController owner)
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

        public void OnExit(HoverBotController owner, IState<HoverBotController> nextState = null)
        {

        }

        public void OnUpdate(HoverBotController owner)
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
                owner.StateMacnie.ChangeState(owner.Puersue);
            }
        }

        private void StopPoint(HoverBotController owner)
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

    public class Pursue : IState<HoverBotController>
    {
        public void OnEnter(HoverBotController owner, IState<HoverBotController> prevState = null)
        {

        }

        public void OnExit(HoverBotController owner, IState<HoverBotController> nextState = null)
        {

        }

        public void OnUpdate(HoverBotController owner)
        {

        }
    }

    public class Attack : IState<HoverBotController>
    {
        public void OnEnter(HoverBotController owner, IState<HoverBotController> prevState = null)
        {

        }

        public void OnExit(HoverBotController owner, IState<HoverBotController> nextState = null)
        {

        }

        public void OnUpdate(HoverBotController owner)
        {

        }
    }


    public class OnDamage : IState<HoverBotController>
    {
        public void OnEnter(HoverBotController owner, IState<HoverBotController> prevState = null)
        {

        }

        public void OnExit(HoverBotController owner, IState<HoverBotController> nextState = null)
        {

        }

        public void OnUpdate(HoverBotController owner)
        {

        }
    }
    #endregion

}
