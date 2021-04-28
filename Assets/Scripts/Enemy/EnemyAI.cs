﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi
{
    public interface IEnemyState
    {
        void OnEnter(EnemyAI owner, IEnemyState prevState = null);
        void OnUpdate(EnemyAI owner);
        void OnExit(EnemyAI owner, IEnemyState nextState = null);
    }

    /// <summary>
    /// ステートパターンによる敵の制御クラス
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyAI : MonoBehaviour
    {
        #region Field
        [SerializeField] Transform target;

        [Header("各状態のスピード")]
        [SerializeField] float patrolSpeed = 2f;
        [SerializeField] float pursueSpeed = 5f;

        [Header("パトロール中の各種設定値")]
        [SerializeField] Transform enemyEye;
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
        IEnemyAttack attack;

        [Header("Gizoms表示色")]
        [SerializeField] Color visitDistanceColor;
        [SerializeField] Color attackRangeColor;
        [SerializeField] Color patrolPathColor;

        NavMeshAgent agent;
        Animator animator;
        #endregion

        #region Field's property
        public Transform Target => target;
        public float PatrolSpeed => patrolSpeed;
        public float PursueSpeed => pursueSpeed;

        public Transform[] PatrolPoints => patrolPoints;
        public int PatrolPointsIndex { get; set; }

        public float BreakTime => breakTime;
        public float StopOffset => stopOffset;
        public NavMeshAgent Agent => agent;
        public Animator Animator => animator;
        public IEnemyAttack Attack => attack;
        #endregion

        #region State's Instance and property



        public IEnemyState CurrentState { get; set; }
        public EnemyIdle EnemyIdle { get; } = new EnemyIdle();
        public EnemyPatrol EnemyPatrol { get; } = new EnemyPatrol();
        public EnemyPursue EnemyPursue { get; } = new EnemyPursue();
        public EnemyAttack EnemyAttack { get; } = new EnemyAttack();
        public EnemyOnDamage EnemyOnDamage { get; } = new EnemyOnDamage();
        #endregion

        #region Method
        private void Start()
        {
            animator = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            attack = GetComponent<IEnemyAttack>();

            agent.autoBraking = false;

            if (!target)
                target = GameObject.FindGameObjectWithTag("Player").transform;

            if (!enemyEye)
                enemyEye = this.transform;

            //Init state
            CurrentState = EnemyIdle;
            CurrentState.OnEnter(this);
        }

        private void Update()
        {
            CurrentState.OnUpdate(this);
            Debug.Log(CurrentState);
        }

        public void ChangeState(IEnemyState nextstate)
        {
            CurrentState.OnExit(this, nextstate);
            nextstate.OnEnter(this, CurrentState);
            CurrentState = nextstate;
        }

        public bool CanSeePlayer()
        {
            Vector3 dir = target.position - transform.position;
            float angle = Vector3.Angle(dir, transform.forward);

            if (dir.magnitude < visitDistance && angle < viewingAngle)
            {
                //Playerと敵の間に障害物があるかどうかRayを飛ばして確かめる
                if (Physics.Linecast(enemyEye.position, target.position,out RaycastHit hit))
                {
                    Debug.DrawLine(enemyEye.position, target.position, Color.white);
                    if(hit.collider.gameObject.CompareTag("Player"))
                    {
                        return true;
                    }
                    return false;
                    //return false;
                }
                //return true;
            }
            return false;
        }

        public bool CanAttackPlayer()
        {
            Vector3 dir = target.position - transform.position;
            if (dir.magnitude < attackRange)
            {
                return true;
            }
            return false;
        }

        public void LookAtPlayer()
        {
            var aim = target.position - transform.position;
            aim.y = 0;
            var look = Quaternion.LookRotation(aim);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnAroundInterpolationSpeed);
        }

        public void SetPatrolPoints(Transform[] points)
        {
            patrolPoints = points;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (CurrentState is EnemyAttack)
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

    public class EnemyIdle : IEnemyState
    {
        void IEnemyState.OnEnter(EnemyAI owner, IEnemyState prevState)
        {
            owner.Animator.Play("Idle");
        }

        void IEnemyState.OnExit(EnemyAI owner, IEnemyState nextState)
        {

        }

        void IEnemyState.OnUpdate(EnemyAI owner)
        {
            if (owner.CanSeePlayer())
            {
                owner.ChangeState(owner.EnemyPursue);
            }
            else if (Random.Range(0, 100) < 10)//適当...
            {
                owner.ChangeState(owner.EnemyPatrol);
            }
        }
    }

    public class EnemyPatrol : IEnemyState
    {
        void IEnemyState.OnEnter(EnemyAI owner, IEnemyState prevState)
        {
            GoToNextPoint(owner);
        }

        void IEnemyState.OnExit(EnemyAI owner, IEnemyState nextState)
        {

        }

        bool isStopping = false;
        void IEnemyState.OnUpdate(EnemyAI owner)
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

            if (owner.CanSeePlayer())
            {
                owner.ChangeState(owner.EnemyPursue);
            }
        }

        float waitTime;
        private void GoToNextPoint(EnemyAI owner)
        {
            waitTime = 0;
            isStopping = false;
            if (owner.PatrolPoints.Length == 0) return;

            owner.PatrolPointsIndex = (owner.PatrolPointsIndex + 1) % owner.PatrolPoints.Length;
            owner.Agent.destination = owner.PatrolPoints[owner.PatrolPointsIndex].position;
            owner.Agent.speed = owner.PatrolSpeed;
            owner.Agent.isStopped = false;
            owner.Animator.Play("Walk");
        }

        private void StopPoint(EnemyAI owner)
        {
            owner.Agent.isStopped = true;
            owner.Animator.Play("Idle");
            waitTime += Time.deltaTime;

            if (waitTime > owner.BreakTime)
            {
                GoToNextPoint(owner);
            }
        }
    }

    public class EnemyPursue : IEnemyState
    {
        void IEnemyState.OnEnter(EnemyAI owner, IEnemyState prevState)
        {
            owner.Agent.speed = owner.PursueSpeed;
            owner.Agent.isStopped = false;
            owner.Animator.Play("Run");
        }

        void IEnemyState.OnExit(EnemyAI owner, IEnemyState nextState)
        {

        }

        void IEnemyState.OnUpdate(EnemyAI owner)
        {
            owner.LookAtPlayer();
           var t = owner.Agent.SetDestination(owner.Target.position);
            Debug.Log(t);

            if (owner.CanAttackPlayer())
            {
                owner.ChangeState(owner.EnemyAttack);
            }

            if (!owner.CanSeePlayer())
            {
                owner.ChangeState(owner.EnemyPatrol);
            }
        }
    }

    public class EnemyAttack : IEnemyState
    {
        void IEnemyState.OnEnter(EnemyAI owner, IEnemyState prevState)
        {
            Debug.Log("攻撃を開始します。");
            owner.Agent.isStopped = true;
            owner.Animator.Play("Attack");
        }

        void IEnemyState.OnExit(EnemyAI owner, IEnemyState nextState)
        {

        }

        void IEnemyState.OnUpdate(EnemyAI owner)
        {
            owner.LookAtPlayer();
            if (!owner.CanAttackPlayer())
            {
                owner.ChangeState(owner.EnemyIdle);
            }

            if (owner.Attack != null)
            {
                owner.Attack.Excute(owner);
            }
        }
    }

    public class EnemyOnDamage : IEnemyState
    {
        public void OnEnter(EnemyAI owner, IEnemyState prevState = null)
        {
            Debug.Log("攻撃を受けた");
        }

        public void OnExit(EnemyAI owner, IEnemyState nextState = null)
        {
        }

        public void OnUpdate(EnemyAI owner)
        {
            owner.ChangeState(owner.EnemyPursue);
        }
    }
}
