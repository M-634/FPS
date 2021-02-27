//using UnityEngine;

//namespace Musashi
//{
//    public class WeaponController : MonoBehaviour
//    {
//        [Tooltip("PC :0 = right, 1 = left")]
//        [SerializeField] Transform[] muzzles;
//        int muzzlesIndex = 0;

//        [SerializeField] BulletControl bullet;
//        [SerializeField] WeaponGeneralData bulletData;
//        [SerializeField] float shotRateTime = 0.5f;
//        [SerializeField] int maxAmmo = 20;
//        BulletControl[] arraysOfAmmo;
//        int currentAmmo;
//        public int CurrentAmmo { get => currentAmmo; set => currentAmmo = ammoCounter.Count(value); }

//        [SerializeField] AmmoCounter ammoCounter;//eventにしたい

//        [SerializeField] float coolTime = 2f;
//        float coolTimeCount;
//        bool isCoolTime;

//        private void Start()
//        {
//            ammoCounter.Init(maxAmmo);
//            PoolingAmmo();
//            ReLoad();
//        }

//        void ReLoad()
//        {
//            CurrentAmmo = maxAmmo;
//            coolTimeCount = 0f;
//            isCoolTime = false;
//        }

//        void PoolingAmmo()
//        {
//            arraysOfAmmo = new BulletControl[maxAmmo];
//            for (int i = 0; i < maxAmmo; i++)
//            {
//                var ammo = Instantiate(bullet, muzzles[muzzlesIndex].position, Quaternion.identity);
//                muzzlesIndex = (muzzlesIndex + 1) % muzzles.Length;
//                ammo.transform.parent = transform;
//                ammo.gameObject.SetActive(false);
//                arraysOfAmmo[i] = ammo;
//            }
//        }

//        private void Update()
//        {
//            if (PlayerInputManager.Shot())
//            {
//                Shot();
//                isCoolTime = false;
//                coolTimeCount = 0f;
//            }

//            if (PlayerInputManager.CoolDownWeapon())
//            {
//                isCoolTime = true;
//            }

//            UpdateAmmo();
//        }

//        private void UpdateAmmo()
//        {
//            if (!isCoolTime) return;

//            coolTimeCount += Time.deltaTime;
//            if (coolTimeCount > coolTime)
//            {
//                CurrentAmmo++;
//                if (CurrentAmmo > maxAmmo)
//                {
//                    ReLoad();
//                }
//            }
//        }

//        float rateTimer;


//        void Shot()
//        {
//            if (CurrentAmmo <= 0)
//            {
//                CurrentAmmo = 0;
//                return;
//            }

//            rateTimer += Time.deltaTime;
//            if (rateTimer > shotRateTime)
//            {
//                rateTimer = 0f;
//                InstantiateAmmoFromPool();
//            }
//        }

//        void InstantiateAmmoFromPool()
//        {
//            foreach (var bullet in arraysOfAmmo)
//            {
//                if (bullet.gameObject.activeSelf == false)
//                {
//                   // bulletData.muzzle = muzzles[muzzlesIndex];
//                    bullet.SetData(bulletData);
//                    muzzlesIndex = (muzzlesIndex + 1) % muzzles.Length;

//                    //Fire!!
//                    bullet.gameObject.SetActive(true);
//                    CurrentAmmo--;
//                    return;
//                }
//            }
//        }
//    }
//}
