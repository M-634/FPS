using UnityEngine;

namespace Musashi.Weapon
{
    /// <summary>
    /// オブジェットプールで管理する弾丸の情報を格納するクラス
    /// </summary>
    [System.Serializable]
    public class ObjectPoolingProjectileInfo
    {
        [SerializeField] Transform muzzle;
        [SerializeField] float power;
        [SerializeField] float damage;
        /// <summary>オブジェットのアクティブがTrue状態の時間</summary>
        [SerializeField] int lifeTime;
        public Transform Muzzle => muzzle;
        public float Power => power;
        public float Damage => damage;
        public int LifeTime => lifeTime;
    }
}
