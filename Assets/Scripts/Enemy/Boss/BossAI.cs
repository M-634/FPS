using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Musashi
{

    public enum BossAttackType
    {
        Melee,//近接
        Range,//遠距離
        Whole//全体
    }

    /// <summary>
    /// プレイヤーとの距離、ボスの残り体力に応じて攻撃パターンを変化す
    /// </summary>
    public class BossAI : MonoBehaviour
    {
        #region field
        [SerializeField] float normalSpeed;
        [SerializeField] float pursueSpeed;
        Transform target;
        NavMeshAgent agent;
        Animator animator;
        StateMacnie<BossAI> stateMacnie;
        BossAttackType attackType;
        #endregion

        #region property
        public float PursueSpeed => pursueSpeed;
        public Transform Target => target;
        public NavMeshAgent Agent => agent;
        public Animator BossAnim => animator;
        public StateMacnie<BossAI> StateMacnie => stateMacnie;
        #region State
        public IState<BossAI> Idle { get; set; } = new BossIdle();
        public IState<BossAI> Pursure { get; set; } = new BossPursuePlayer();
        public IState<BossAI> Dead { get; set; } = new BossDead();
        #endregion

        #endregion

        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
            agent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            stateMacnie = new StateMacnie<BossAI>(this, Idle);
        }

        void Update()
        {
            stateMacnie.ExcuteOnUpdate();
        }

        public void BossDie()
        {
            stateMacnie.ChangeState(Dead);
        }
    }

    public class BossIdle : IState<BossAI>
    {
        public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
        {
            Debug.Log("Idle");
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
            Debug.Log("pursue");
            owner.Agent.speed = owner.PursueSpeed;
            owner.Agent.isStopped = false;
            owner.BossAnim.Play("Run");
        }

        public void OnExit(BossAI owner, IState<BossAI> nextState = null)
        {
    
        }

        public void OnUpdate(BossAI owner)
        {
            owner.Agent.SetDestination(owner.Target.position);
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