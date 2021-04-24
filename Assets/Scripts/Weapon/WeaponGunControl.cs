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
        [SerializeField] BulletControl bulletPrefab;
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
            Debug.Log(ammoCounter);
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            if (!muzzle)
                muzzle = this.transform;

            currentAmmo = maxAmmo;
        }

        public void Reload()
        {
            bool canReload = ammoCounter ? ammoCounter.CanReloadAmmo(ref maxAmmo,ref currentAmmo) : true;
            if (canReload)
            {
                animator.Play("Reload");
                if (audioSource)
                    audioSource.Play(ReloadClip, audioSource.volume);
            }
            else
            {
                //弾切れ！
            }
        }

        /// <summary>
        /// リロードアニメーションイベントから呼ばれる関数
        /// </summary>
        public void EndReload()
        {
            //To Do :ここ,実際にリロードできる数を返すこと
            currentAmmo = ammoCounter.ReloadAmmoNumber(ref maxAmmo, ref currentAmmo);
            ammoCounter.Display(ref currentAmmo);
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
            //playerEventでFOVを変える
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
                    animator.Play("Shoot");
            }
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void Shot()
        {
            var mF = Instantiate(muzzleFalsh, muzzle.position, muzzle.rotation);
            mF.Play();

            var b = Instantiate(bulletPrefab, muzzle.position, Quaternion.identity);
            b.AddForce(ref shotPower, ref shotDamage, muzzle);

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
