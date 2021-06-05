using UnityEngine;
using Musashi.Weapon;

namespace Musashi.NPC
{
    /// <summary>
    /// NPCが銃を撃つタイプのキャラクターにアタッチする。撃つ弾をオブジェットプールで管理し、
    /// 撃つタイミングは、アニメーションイベントから呼ばれる。
    /// </summary>
    public class NPCShotAttack :BaseNPCAttackEventControl,IPoolUser<NPCShotAttack>
    {
        [SerializeField] ObjectPoolingProjectileInfo projectileInfo;
        [SerializeField] ProjectileControl projectilePrefab;
        [SerializeField] ParticleSystem muzzleFlashVFXPrefab;
        [SerializeField] Transform poolParent;

        [SerializeField,Range(1,30)] int poolSize = 5;
        PoolObjectManager poolObjectManager;

        private void Start()
        {
            InitializePoolObject(poolSize);
        }

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
    }
}