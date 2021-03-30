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
        [SerializeField] Transform muzzle;
       // [SerializeField] PoolingObject poolingObject;
        public BulletControl bullet;
        public ParticleSystem muzzleFalsh;

        [SerializeField] int maxAmmo;

        [SerializeField] float shotPower = 100f;
        [SerializeField] float shotDamage;
        [SerializeField] float shotRateTime;
        float lastTimeShot = Mathf.NegativeInfinity;

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

            //Pooling();
            ReLoad();
            gameObject.SetActive(false);
        }

        public void ReLoad()
        {
            CurrentAmmo = maxAmmo;

            if (audioSource)
                audioSource.Play(ReloadClip);
        }

        private void Update()
        {
            if (canUse == false) return;

            if (Input.GetKeyDown(KeyCode.R)) ReLoad();

            switch (weaponShootType)
            {
                case WeaponShootType.Manual:
                    if (Input.GetMouseButtonDown(0)) TryShot();
                    break;
                case WeaponShootType.Automatic:
                    if (Input.GetMouseButton(0)) TryShot();
                    break;
            }
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
            if (CurrentAmmo < 1)
            {
                CurrentAmmo = 0;
                ReLoad();
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
            var b = Instantiate(bullet,muzzle.position,Quaternion.identity);
            b.AddForce(ref shotPower, muzzle);

            var mF = Instantiate(muzzleFalsh, muzzle.position, muzzle.rotation);

            if (audioSource)
                audioSource.Play(shotClip);

            CurrentAmmo--;
            lastTimeShot = Time.time;

            //for (int i = 0; i < poolingObjects.Length; i++)
            //{
            //    if (poolingObjects[i].CanUse)
            //    {
            //        //fire
            //        Debug.Log("fire");
            //        poolingObjects[i].SetActive = true;
            //        poolingObjects[i].muzzleFalsh.Play();
            //        poolingObjects[i].bullet.AddForce(ref shotPower, muzzle);

            //        if (audioSource)
            //            audioSource.Play(shotClip);

            //        CurrentAmmo--;
            //        lastTimeShot = Time.time;
            //        return;
            //    }
            //}
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
