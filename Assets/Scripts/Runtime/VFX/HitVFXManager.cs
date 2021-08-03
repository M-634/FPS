using System.Collections;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// ゲームオ中、武器の攻撃が当たった時に出すエフェクトをまとめて制御する関数するクラス
    /// </summary>
    public class HitVFXManager : SingletonMonoBehaviour<HitVFXManager>, IPoolUser<HitVFXManager>
    {
        #region field
        [Header("set player shot vfx")]
        [SerializeField] GameObject decalVFX;//デフォルトのエフェクト


        [Header("set pool parent object")]
        [SerializeField] Transform poolObjcetParent;
        [Header("set pool size")]
        [SerializeField, Range(1, 100)] int poolSize = 1;

        [Header("set player shot hit sfX")]
        [SerializeField] AudioClip hitSFX;

        PoolObjectManager poolObjectManager;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            if (!poolObjcetParent)
                poolObjcetParent = this.transform;

            InitializePoolObject(poolSize);
        }

        public void InitializePoolObject(int poolsize = 1)
        {
            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < poolsize; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObject = poolObjectManager.InstantiatePoolObj();
            var decalInstance = Instantiate(decalVFX, poolObjcetParent);
            poolObject.AddObj(decalInstance);
            poolObject.SetActiveAll(false);
            return poolObject;
        }

        /// <summary>
        /// ヒットしたオブジェクトのlayerに応じて,エフェクトを変える
        /// (今のところは、decalVFXで統一している)
        /// </summary>
        public void ProductEffectByPlayer(int layer, Vector3 hitPosition, Vector3 normal)
        {
            Quaternion quaternion = Quaternion.LookRotation(normal * -1f);
            poolObjectManager.UsePoolObject(decalVFX, hitPosition, quaternion,SetPoolObj);
        }

        public void ProductEffectByNPC(GameObject effect,Vector3 hitPosition,Vector3 normal)
        {
            if(effect != null)
            {
                Quaternion quaternion = Quaternion.LookRotation(normal * -1f);
                poolObjectManager.UsePoolObject(effect, hitPosition, quaternion,SetPoolObj);
            }
        }
    }
}