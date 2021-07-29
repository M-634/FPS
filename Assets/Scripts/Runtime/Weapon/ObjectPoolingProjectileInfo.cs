using UnityEngine;

namespace Musashi.Weapon
{
    /// <summary>
    /// オブジェットプールで管理する弾丸の情報を格納するクラス
    /// </summary>
    [System.Serializable]
    public class ObjectPoolingProjectileInfo
    {
        public Transform muzzle;
        public float power;
        public float damage;
        public float lifeTime;
    }
}
