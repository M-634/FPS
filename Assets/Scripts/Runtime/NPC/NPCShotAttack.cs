using UnityEngine;
using Musashi.Weapon;

namespace Musashi.NPC
{
    /// <summary>
    /// NPCが銃を撃つタイプのキャラクターにアタッチする。撃つ弾をオブジェットプールで管理し、
    /// 撃つタイミングは、アニメーションイベントから呼ばれる。
    /// </summary>
    [RequireComponent(typeof(NPCMoveControl),typeof(Animator))]
    public class NPCShotAttack : BaseNPCAttackEventControl, IPoolUser<NPCShotAttack>
    {
        [SerializeField] ObjectPoolingProjectileInfo projectileInfo;
        [SerializeField] ProjectileControl projectilePrefab;
        [SerializeField] ParticleSystem muzzleFlashVFXPrefab;
        [SerializeField] Transform poolParent;

        [SerializeField, Range(1, 30)] int poolSize = 5;

        [Header("Setting IK")]
        [SerializeField] bool useIK;
        [SerializeField] Transform targetRightHand;
        [SerializeField] Transform targetLeftHand;
   
        bool isIkActive;
        PoolObjectManager poolObjectManager;
        Animator animator;
        NPCMoveControl control;

        private void Start()
        {
            isIkActive = false;
            control = GetComponent<NPCMoveControl>();
            animator = GetComponent<Animator>();
            if (useIK)
            {
                control.OnEnterAttackEvent += () => isIkActive = true;
                control.OnExitAttackEvent += () => isIkActive = false;
            }
            InitializePoolObject(poolSize);
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる関数
        /// </summary>
        public void Shot()
        {
            poolObjectManager.UsePoolObject(projectileInfo.Muzzle.position, Quaternion.identity, SetPoolObj);
        }

        public void InitializePoolObject(int poolSize = 1)
        {

            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < poolSize; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObj = poolObjectManager.InstantiatePoolObj();

            var projectile = Instantiate(projectilePrefab, poolParent);
            var muzzleFlash = Instantiate(muzzleFlashVFXPrefab, poolParent);

            projectile.SetProjectileInfo(projectileInfo);

            poolObj.AddObj(projectile.gameObject);
            poolObj.AddObj(muzzleFlash.gameObject);

            poolObj.SetActiveAll(false);

            return poolObj;
        }

        /// <summary>
        /// NPCの銃を構えるアニメーションで、標準がプレイヤーに向IKで調整する
        /// </summary>
        /// <param name="layerIndex"></param>
        private void OnAnimatorIK(int layerIndex)
        {
            if (!isIkActive) return;  

            if (targetLeftHand == null || targetRightHand == null) return;

            // 両手の IK Position/Rotation をセットする
            animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            animator.SetIKPosition(AvatarIKGoal.RightHand, targetRightHand.position);
            animator.SetIKRotation(AvatarIKGoal.RightHand, targetRightHand.rotation);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, targetLeftHand.position);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, targetLeftHand.rotation);

            //銃を持っている手のIKターゲット向きをプレイヤーに向ける。(余裕があったら)
            //敵の顔をプレイヤーに向ける
        }
    }
}