using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤープレハブの子供にある銃オブジェクトにアタッチする。
    /// 初期化時は、弾とエフェクトをプールさせたらアクティブを切る
    /// </summary>
    public class WeaponGunControl : BaseWeapon
    {
        [System.Serializable]
        class PoolingObject
        {
            public BulletControl bullet;
            public ParticleSystem muzzleFalsh;
            readonly Rigidbody bulletRb;

            public PoolingObject(BulletControl bullet, ParticleSystem muzzleFalsh, Transform muzzle = null, bool active = false)
            {
                this.bullet = Instantiate(bullet, muzzle.position, Quaternion.identity, muzzle);
                bulletRb = bullet.GetComponent<Rigidbody>();
                this.muzzleFalsh = Instantiate(muzzleFalsh, muzzle.position, Quaternion.identity, muzzle);
                SetActive = active;
            }

            public bool SetActive
            {
                set
                {
                    bullet.gameObject.SetActive(value);
                    muzzleFalsh.gameObject.SetActive(value);
                }
            }

            public bool CanUse
            {
                get => !bullet.gameObject.activeSelf && !muzzleFalsh.gameObject.activeSelf;
            }
        }

        [SerializeField] Transform muzzle;
        [SerializeField] PoolingObject poolingObject;

        [SerializeField] int maxAmmo;

        [SerializeField] float shotPower = 100f;
        [SerializeField] float shotDamage;
        [SerializeField] float shotRateTime;

        [SerializeField] AudioClip shotClip;
        [SerializeField] AudioClip ReloadClip;

        int currentAmmo;
        public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }

        PoolingObject[] poolingObjects;
        Animator animator;
        AudioSource audioSource;

        private void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            if (!muzzle)
                muzzle = this.transform;

            Pooling();
            ReLoad();
            gameObject.SetActive(false);
        }

        public void ReLoad()
        {
            CurrentAmmo = maxAmmo;

            if (audioSource)
                audioSource.Play(ReloadClip);
        }


        private void Pooling()
        {
            poolingObjects = new PoolingObject[maxAmmo];
            for (int i = 0; i < maxAmmo; i++)
            {
                var newObj = new PoolingObject(poolingObject.bullet, poolingObject.muzzleFalsh, muzzle, false);
                poolingObjects[i] = newObj;
            }
        }

        public void TryShot()
        {
            if (CurrentAmmo < 1)
            {
                CurrentAmmo = 0;
                return;
            }
            if (animator)
                animator.SetTrigger("Fire");
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void Shot()
        {
            for (int i = 0; i < poolingObjects.Length; i++)
            {
                if (poolingObjects[i].CanUse)
                {
                    //fire

                    if (audioSource)
                        audioSource.Play(shotClip);

                    CurrentAmmo--;
                    return;
                }
            }
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void CasingRelease()
        {

        }

        public override void Attack()
        {
            TryShot();
        }
    }
}
