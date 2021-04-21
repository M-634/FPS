using UnityEngine;

namespace Musashi
{
    public class WeaponGunControl : BaseWeapon
    {
        [System.Serializable]
        class PoolingObject
        {
            public BulletControl bullet;
            public ParticleSystem muzzleFalsh;
            Transform muzzle;


            public PoolingObject(BulletControl bullet, ParticleSystem muzzleFalsh, Transform muzzle = null, bool active = false)
            {
                this.bullet = Instantiate(bullet, muzzle.position, Quaternion.identity, muzzle);
                this.muzzleFalsh = Instantiate(muzzleFalsh, muzzle.position, Quaternion.identity, muzzle);
                SetActive = active;
                this.muzzle = muzzle;
            }

            public bool SetActive
            {
                set
                {
                    bullet.gameObject.SetActive(value);
                    muzzleFalsh.gameObject.SetActive(value);
                    if (value == true)
                    {
                        bullet.transform.position = muzzle.position;
                        muzzleFalsh.transform.position = muzzle.position;
                    }
                }
            }

            public bool CanUse
            {
                get => !bullet.gameObject.activeSelf && !muzzleFalsh.gameObject.activeSelf;
            }
        }

        private enum WeaponShootType
        {
            Manual, Automatic,
        }

        [SerializeField] WeaponShootType weaponShootType;
        // [SerializeField] PoolingObject poolingObject;
        [SerializeField] Transform muzzle;
        public ParticleSystem muzzleFalsh;

        [Header("Ammo")]
        [SerializeField] BulletControl bullet;
        [SerializeField] AmmoCounter ammoCounter;
        [SerializeField] int maxAmmo;
        int currentAmmo;

        [Header("Settings")]
        [SerializeField] float shotPower = 100f;
        [SerializeField] float shotDamage;
        [SerializeField] float shotRateTime;
        float lastTimeShot = Mathf.NegativeInfinity;

        [Header("SFX")]
        [SerializeField] AudioClip shotClip;
        [SerializeField] AudioClip ReloadClip;

        //PoolingObject[] poolingObjects;
        PlayerInputManager playerInput;
        Animator animator;
        AudioSource audioSource;

        private void Awake()
        {
            playerInput = transform.GetComponentInParent<PlayerInputManager>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            if (!muzzle)
                muzzle = this.transform;

            currentAmmo = maxAmmo;
        }

        public void Reload()
        {
            bool canReload = ammoCounter ? ammoCounter.CanReloadAmmo(maxAmmo, currentAmmo) : true;
            if (canReload)
            {
                currentAmmo = maxAmmo;
                if (audioSource)
                    audioSource.Play(ReloadClip, 0.5f);
                //リロード中は、弾が撃てないようにすること!
            }
            else
            {
                //弾切れ！
            }
        }

        bool isAiming = false;
        private void Update()
        {
            if (playerInput.Reload) Reload();

            if (playerInput.Aim)
                isAiming = true;
            else
                isAiming = false;

            SetAim();

            switch (weaponShootType)
            {
                case WeaponShootType.Manual:
                    if (playerInput.Fire) TryShot();
                    break;
                case WeaponShootType.Automatic:
                    if (playerInput.HeldFire) TryShot();
                    break;
            }
        }

        void SetAim()
        {
            animator.SetBool("Aim", isAiming);
        }


        //private void Pooling()
        //{
        //    poolingObjects = new PoolingObject[maxAmmo];
        //    for (int i = 0; i < maxAmmo; i++)
        //    {
        //        var newObj = new PoolingObject(poolingObject.bullet, poolingObject.muzzleFalsh, muzzle, false);
        //        poolingObjects[i] = newObj;
        //    }
        //}

        public void TryShot()
        {
            if (currentAmmo < 1)
            {
                currentAmmo = 0;
                Reload();
                return;
            }

            if (lastTimeShot + shotRateTime < Time.time)
            {
                if (animator)
                    animator.SetTrigger("Fire");//auto時のアニメーションは速くしないと合わないことに注意
            }
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void Shot()
        {
            var b = Instantiate(bullet, muzzle.position, Quaternion.identity);
            b.AddForce(ref shotPower, ref shotDamage, muzzle);

            var mF = Instantiate(muzzleFalsh, muzzle.position, muzzle.rotation);

            if (audioSource)
                audioSource.Play(shotClip, audioSource.volume);

            currentAmmo--;
            if (ammoCounter)
                ammoCounter.Display(ref currentAmmo);
            lastTimeShot = Time.time;
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void CasingRelease()
        {

        }

        public void OnEnable()
        {
            if (ammoCounter)
                ammoCounter.Display(ref currentAmmo);
        }

        public void OnDisable()
        {
            if (ammoCounter)
                ammoCounter.Text.enabled = false;
        }
    }
}
