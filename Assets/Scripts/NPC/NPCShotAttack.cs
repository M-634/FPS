using UnityEngine;

namespace Musashi.NPC
{
    /// <summary>
    /// NPCが銃を撃つタイプのキャラクターの場合、撃つ弾をオブジェットプールで管理し、
    /// 撃つタイミングは、アニメーションイベントから呼ばれる。
    /// </summary>
    public class NPCShotAttack :BaseNPCAttackEventControl,IPoolUser<NPCShotAttack>
    {
        [SerializeField] Transform muzzle;
        [SerializeField] GameObject projectile;
        [SerializeField] GameObject muzzleFlashVFX;
        [SerializeField] Transform poolParent;

        [SerializeField,Range(1,30)] int poolSize = 5;
        PoolObjectManager poolObjectManager;

        private void Start()
        {
            AddEvent(Shot);
            InitializePoolObject(poolSize);
        }

        private void Shot()
        {
            poolObjectManager.UsePoolObject(muzzle.position, Quaternion.identity, SetPoolObj);
            Debug.Log("shot!");
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

            var b = Instantiate(projectile,poolParent);
            var mF = Instantiate(muzzleFlashVFX,poolParent);

            poolObj.AddObj(b);
            poolObj.AddObj(mF);

            poolObj.SetActiveAll(false);

            return poolObj;
        }
    }
}