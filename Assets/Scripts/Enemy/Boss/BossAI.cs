using System.Collections;
using UnityEngine;

namespace Musashi
{
    public class BossAI : MonoBehaviour
    {
        Animator animator;
        StateMacnie<BossAI> stateMacnie;

        public Animator BossAnim => animator;

        #region State
        public IState<BossAI> Idle { get; set; } = new BossIdle();
        public IState<BossAI> Dead { get; set; } = new BossDead();
        #endregion

        void Start()
        {
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
            if (owner.BossAnim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f)
            {
                //GameManager.Instance.GameClear();
                Debug.Log("a");
                haveDead = true;
            }
        }
    }
}