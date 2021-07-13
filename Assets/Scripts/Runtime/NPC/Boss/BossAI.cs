using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi.NPC
{
    /// <summary>
    /// プレイヤーとの距離、ボスの残り体力に応じて攻撃パターンを変化する
    /// </summary>
    public class BossAI : MonoBehaviour
    {
        public enum BossAttackType
        {
            Melee,//近接
            Long,//遠距離
            Whole//全体
        }

        #region Field
        [SerializeField] float normalSpeed;
        [SerializeField] float pursueSpeed;

        [SerializeField] Transform eye;
        [SerializeField] float visitDistance = 10.0f;
        [SerializeField] float viewingAngle = 30.0f;
     
        [SerializeField] float meleeRange = 5.0f;
        [SerializeField] float longRange = 10.0f;
        [SerializeField] float wholeRange = 12.0f;
        [SerializeField] float restTime = 2.0f;//攻撃してから次の攻撃までの休憩時間

        [SerializeField, Range(0, 1f)] float turnAroundInterpolationSpeed = 0.1f;

        [SerializeField] Color meleeRangeColor;
        [SerializeField] Color longRangeColor;
        [SerializeField] Color wholeRangeColor;

        Transform target;
        NavMeshAgent agent;
        Animator animator;
        StateMachine<BossAI> stateMacnie;
        BossAttackType attackType;

        private float coolAttackTimer = 0;
        #endregion

        #region property
        public float NormalSpeed => normalSpeed;
        public float PursueSpeed => pursueSpeed;
        public float VisitDistance => visitDistance;
        public float ViewingAngle => viewingAngle;
        public float TurnAroundInterpolationSpeed => turnAroundInterpolationSpeed;
        public bool CoolTime { get; set; } = false;
        public Transform Eye => eye;
        public Transform Target => target;
        public NavMeshAgent Agent => agent;
        public Animator BossAnim => animator;
        public BossAttackType AttackType { get => attackType; set => attackType = value; }
        public StateMachine<BossAI> StateMacnie => stateMacnie;
        #region State
        public IState<BossAI> Idle { get; set; } = new BossIdle();
        public IState<BossAI> Pursure { get; set; } = new BossPursuePlayer();
        public IState<BossAI> Attack { get; set; } = new BossAttack();
        public IState<BossAI> Dead { get; set; } = new BossDead();
        #endregion

        #endregion

        void Start()
        {
            if (eye == null)
            {
                eye = this.transform;
            }
            target = GameObject.FindGameObjectWithTag("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            stateMacnie = new StateMachine<BossAI>(this, Idle);
            attackType = BossAttackType.Melee;
        }

        void Update()
        {
            stateMacnie.ExcuteOnUpdate();

            if (CoolTime)
            {
                coolAttackTimer += Time.deltaTime;
                if(coolAttackTimer > restTime)
                {
                    coolAttackTimer = 0f;
                    CoolTime = false;
                }
            }
        }

        public void BossDie()
        {
            stateMacnie.ChangeState(Dead);
        }

        /// <summary>
        /// プレイヤーとの距離とHPの状況に応じて攻撃タイプを変える。
        /// 攻撃条件を満たせばTrueを返す。
        /// </summary>
        /// <returns></returns>
        public bool SelectAttackType()
        {
            //if (NPCAIHelper.CanAttackPlayer(target, this.transform, meleeRange))
            {
                if (CoolTime)
                {
                    agent.isStopped = true;
                    return false;
                }
                attackType = BossAttackType.Melee;
                return true;
            }

            //if (AIHelper.CanAttackPlayer(target, this.transform, longRange))
            //{
            //    attackType = BossAttackType.Long;
            //    return true;
            //}

            //if (AIHelper.CanAttackPlayer(target, this.transform, wholeRange))
            //{
            //    attackType = BossAttackType.Whole;
            //    return true;
            //}
            //return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //if (CurrentState is EnemyAttack)
            //{
            //    Handles.color = attackRangeColor;
            //    Handles.DrawSolidDisc(transform.position, Vector3.up, attackRange);
            //}
            //else
            //{
            //    Handles.color = visitDistanceColor;
            //    Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0, -viewingAngle, 0) * transform.forward, viewingAngle * 2, visitDistance);

            //    Handles.color = attackRangeColor;
            //    Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0, -viewingAngle, 0) * transform.forward, viewingAngle * 2, attackRange);
            //}
            Handles.color = meleeRangeColor;
            Handles.DrawSolidArc(transform.position, Vector3.up, Quaternion.Euler(0, -viewingAngle, 0) * transform.forward, viewingAngle * 2, meleeRange);

            Gizmos.color = Color.red;
        }
#endif
    }

    public class BossIdle : IState<BossAI>
    {
        public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
        {
            //Debug.Log("Idle");
            owner.Agent.isStopped = true;
            owner.BossAnim.Play("Idle");
        }

        public void OnExit(BossAI owner, IState<BossAI> nextState = null)
        {

        }

        public void OnUpdate(BossAI owner)
        {
            owner.StateMacnie.ChangeState(owner.Pursure);
        }
    }

    public class BossPursuePlayer : IState<BossAI>
    {
        public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
        {
            //Debug.Log("pursue");
            owner.Agent.speed = owner.PursueSpeed;
            owner.Agent.isStopped = false;
            owner.BossAnim.Play("Run");
        }

        public void OnExit(BossAI owner, IState<BossAI> nextState = null)
        {

        }

        public void OnUpdate(BossAI owner)
        {
            //NPCAIHelper.LookAtPlayer(owner.Target, owner.transform, owner.TurnAroundInterpolationSpeed);
            owner.Agent.SetDestination(owner.Target.position);
            
            if (owner.SelectAttackType())
            {
                owner.StateMacnie.ChangeState(owner.Attack);
            }
        }
    }

    public class BossAttack : IState<BossAI>
    {
        public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
        {
            owner.Agent.isStopped = true;
            if(owner.AttackType == BossAI.BossAttackType.Melee)
            {
                //近接 
                owner.BossAnim.Play("Melee");
            }

            if(owner.AttackType == BossAI.BossAttackType.Long)
            {
                //遠距離
            }

            if(owner.AttackType == BossAI.BossAttackType.Whole)
            {
                //全体
            }
        }

        public void OnExit(BossAI owner, IState<BossAI> nextState = null)
        {
            owner.CoolTime = true;
        }

        public void OnUpdate(BossAI owner)
        {
            if(owner.BossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                owner.StateMacnie.ChangeState(owner.Idle);
            }
        }
    }

    public class BossDead : IState<BossAI>
    {
        bool haveDead = false;
        public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
        {
            Debug.Log("Die");
            owner.BossAnim.Play("OnDie");
        }

        public void OnExit(BossAI owner, IState<BossAI> nextState = null)
        {

        }

        public void OnUpdate(BossAI owner)
        {

            if (haveDead) return;

            //死亡アニメーションが終わったら
            if (owner.BossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)//error : GetCurrentAnimatorStateInfo(0).normalizedTime  = -1
            {
                //GameManager.Instance.GameClear();
                haveDead = true;
            }
        }
    }
}