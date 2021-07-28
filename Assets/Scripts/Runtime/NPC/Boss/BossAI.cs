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

        [SerializeField] GameObject meleeHitObject;

        [SerializeField, Range(0, 1f)] float turnAroundInterpolationSpeed = 0.1f;

        [SerializeField] Color meleeRangeColor;
        [SerializeField] Color longRangeColor;
        [SerializeField] Color wholeRangeColor;

        GameObject currentHitobject;
        Transform target;
        NavMeshAgent agent;
        Animator animator;
        StateMachine<BossAI> stateMacnie;
        BossAttackType attackType;

        private bool coolTime = false;
        private float coolAttackTimer = 0;
        #endregion

        #region State
        readonly IState<BossAI> Idle = new BossIdle();
        readonly IState<BossAI> Pursure = new BossPursuePlayer();
        readonly IState<BossAI> Attack = new BossAttack();
        readonly IState<BossAI> Dead = new BossDead();
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
            meleeHitObject.SetActive(false);
        }

        void Update()
        {
            stateMacnie.ExcuteOnUpdate();

            if (coolTime)
            {
                coolAttackTimer += Time.deltaTime;
                if (coolAttackTimer > restTime)
                {
                    coolAttackTimer = 0f;
                    coolTime = false;
                }
            }
        }
        private void LookAtTarget()
        {
            var aim = target.position - transform.position;
            aim.y = 0;
            var look = Quaternion.LookRotation(aim);
            transform.rotation = Quaternion.Slerp(transform.rotation, look, turnAroundInterpolationSpeed);
        }

        private bool CanAttackTarget(float attackRange)
        {
            Vector3 dir = target.position - transform.position;
            if (dir.magnitude < attackRange)
            {
                return true;
            }
            return false;
        }

        public void BossDie()
        {
            stateMacnie.ChangeState(Dead);
        }

        /// <summary>
        /// プレイヤーとの距離とHPの状況に応じて攻撃タイプを変える。
        /// 攻撃条件を満たせばTrueを返す。（今のところ近接のみ：他の攻撃は、effectがまだ出来てない）
        /// </summary>
        /// <returns></returns>
        public bool SelectAttackType()
        {
            if (CanAttackTarget(meleeRange))
            {
                attackType = BossAttackType.Melee;
                currentHitobject = meleeHitObject;
                return true;
            }
            return false;
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる関数。
        /// 攻撃の当たり判定オブジェクトのアクティブを切り替える
        /// </summary>
        public void ActiveHitPoint()
        {
            currentHitobject.SetActive(true);
        }

        public void EndAttack()
        {
            currentHitobject.SetActive(false);
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

        #region state classes
        public class BossIdle : IState<BossAI>
        {
            public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
            {
                owner.agent.isStopped = true;
                owner.animator.Play("Idle");
            }

            public void OnExit(BossAI owner, IState<BossAI> nextState = null)
            {

            }

            public void OnUpdate(BossAI owner)
            {
                if (!owner.coolTime)
                {
                    owner.stateMacnie.ChangeState(owner.Pursure);
                }
            }
        }

        public class BossPursuePlayer : IState<BossAI>
        {
            public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
            {
                owner.agent.speed = owner.pursueSpeed;
                owner.agent.isStopped = false;
                owner.animator.Play("Run");
            }

            public void OnExit(BossAI owner, IState<BossAI> nextState = null)
            {

            }

            public void OnUpdate(BossAI owner)
            {
                owner.LookAtTarget();
                owner.agent.SetDestination(owner.target.position);

                if (!owner.coolTime && owner.SelectAttackType())
                {
                    owner.stateMacnie.ChangeState(owner.Attack);
                }
            }
        }

        public class BossAttack : IState<BossAI>
        {
            public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
            {

                owner.agent.isStopped = true;
                if (owner.attackType == BossAttackType.Melee)
                {
                    //近接 
                    owner.animator.Play("Melee");
                }

                if (owner.attackType == BossAttackType.Long)
                {
                    //遠距離
                }

                if (owner.attackType == BossAttackType.Whole)
                {
                    //全体
                }
            }

            public void OnExit(BossAI owner, IState<BossAI> nextState = null)
            {
                owner.coolTime = true;
            }

            public void OnUpdate(BossAI owner)
            {

                if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    owner.stateMacnie.ChangeState(owner.Idle);
                }
            }
        }

        public class BossDead : IState<BossAI>
        {
            bool haveDead = false;
            public void OnEnter(BossAI owner, IState<BossAI> prevState = null)
            {
                Debug.Log("Die");
                owner.animator.Play("OnDie");
            }

            public void OnExit(BossAI owner, IState<BossAI> nextState = null)
            {

            }

            public void OnUpdate(BossAI owner)
            {

                if (haveDead) return;

                //死亡アニメーションが終わったら
                if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)//error : GetCurrentAnimatorStateInfo(0).normalizedTime  = -1
                {
                    GameManager.Instance.GameClear();
                    haveDead = true;
                }
            }
        }
        #endregion
    }

}