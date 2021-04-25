using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// プレイヤーの子どもに銃を持たせおく。
    /// 銃を使用時にアクティブをtrueに、しまったらアクティブをfalseにする。
    /// muzzle flash と弾丸はオブジェットプールさせる。
    /// </summary>
    public class WeaponGunControl : BaseWeapon
    {
        /// <summary>
        /// memo : 抽象化させる
        /// </summary>
        class PoolObjcet
        {
            public BulletControl bulletControl;
            public ParticleSystem muzzleFalsh;
            public bool ActiveSelf
            {
                get
                {
                    if (!bulletControl.gameObject.activeSelf && !muzzleFalsh.gameObject.activeSelf)
                        return false;
                    return true;
                }
            }

            public void SetActive(bool value)
            {
                bulletControl.gameObject.SetActive(value);
                muzzleFalsh.gameObject.SetActive(value);
            }

            public void SetPosition(Vector3 pos)
            {
                bulletControl.transform.position = pos;
                muzzleFalsh.transform.position = pos;
            }
        }

        private enum WeaponShootType
        {
            Manual, Automatic,
        }

        [SerializeField] WeaponShootType weaponShootType;
        [SerializeField] Transform muzzle;

        [Header("Pool Setting")]
        [SerializeField] BulletControl bulletPrefab;
        [SerializeField] ParticleSystem muzzleFalsh;
        [SerializeField] int maxPoolNumber;
        List<PoolObjcet> poolObjcetsList;

        [Header("Ammo Setting")]
        [SerializeField] AmmoCounter ammoCounter;
        [SerializeField] int maxAmmo;
        int currentAmmo;

        [Header("Shot Setting")]
        [SerializeField] float shotPower = 100f;
        [SerializeField] float shotDamage;
        [SerializeField] float shotRateTime;
        float lastTimeShot = Mathf.NegativeInfinity;

        [Header("Aim Setting")]
        [SerializeField] float aimCameraFOV;
        [Tooltip("Aimアニメーションが終わるフレームと同じにするといい感じになる")]
        [SerializeField] float aimSpeed;

        [Header("SFX")]
        [SerializeField] AudioClip shotClip;
        [SerializeField] AudioClip ReloadClip;

        [Header("Reticle")]
        [SerializeField] ReticleAnimation reticle;

        PlayerInputManager playerInput;
        PlayerCamaraControl playerCamara;
        PlayerEventManager playerEvent;

        Animator animator;
        AudioSource audioSource;

        bool canAction = true;//「銃を撃つ」、「リロードする」といったアクションができるかどうか判定する変数（例:インベントリを開いた状態では撃てないし、リロードできない）
        bool isAiming = false;

        private void Awake()
        {
            playerInput = transform.GetComponentInParent<PlayerInputManager>();
            playerCamara = transform.GetComponentInParent<PlayerCamaraControl>();

            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            if (!muzzle)
                muzzle = this.transform;

            currentAmmo = maxAmmo;

            InitializePoolObject();
        }


        /// <summary>
        /// bullet と muzzleFlashのオブジェットポールを初期化する
        /// </summary>
        private void InitializePoolObject()
        {
            poolObjcetsList = new List<PoolObjcet>();
            for (int i = 0; i < maxPoolNumber; i++)
            {
                InstantiatePoolObj();
            }
        }

        private PoolObjcet InstantiatePoolObj()
        {
            var poolObj = new PoolObjcet
            {
                bulletControl = Instantiate(bulletPrefab, transform),
                muzzleFalsh = Instantiate(muzzleFalsh, transform)
            };

            poolObj.bulletControl.SetInfo(ref shotPower, ref shotDamage);
            poolObjcetsList.Add(poolObj);
            poolObj.SetActive(false);
            return poolObj;
        }

        private void UsePoolObject()
        {
            foreach (var item in poolObjcetsList)
            {
                if (item.ActiveSelf) continue;
                item.SetPosition(muzzle.position);
                item.SetActive(true);
                return;
            }

            //用意したオブジェットプール内のオブジェットのアクティブが全てtrueなら
            //新しいプールオブジェットを創る
            var poolObj = InstantiatePoolObj();
            poolObj.SetPosition(muzzle.position);
            poolObj.SetActive(true);
        }

        private void BeginningReload()
        {
            bool canReload = ammoCounter ? ammoCounter.CanReloadAmmo(ref maxAmmo, ref currentAmmo) : true;
            if (canReload)
            {
                if (animator)
                    animator.Play("Reload");
                else
                    currentAmmo = maxAmmo;

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
            if (ammoCounter)
            {
                currentAmmo = ammoCounter.ReloadAmmoNumber(ref maxAmmo, ref currentAmmo);
                ammoCounter.Display(ref currentAmmo);
            }
            else
            {
                currentAmmo = maxAmmo;
            }
        }

        private void Update()
        {
            if (!canAction) return;

            if (playerInput.Reload) BeginningReload();

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
            if (isAiming)
            {
                playerCamara.SetFovOfCamera(aimCameraFOV, aimSpeed);
            }
            else
            {
                playerCamara.SetNormalFovOfCamera(aimSpeed);
            }
        }


        private void TryShot()
        {
            if (currentAmmo < 1)
            {
                currentAmmo = 0;
                BeginningReload();
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
            UsePoolObject();

            if (audioSource)
                audioSource.Play(shotClip, audioSource.volume);

            currentAmmo--;
            if (ammoCounter)
                ammoCounter.Display(ref currentAmmo);

            if (reticle)
                reticle.IsShot = true;

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

            if (reticle)
                reticle.IsEquipingGun = true;


            playerEvent = transform.GetComponentInParent<PlayerEventManager>();
            if (playerEvent)
            {
                playerEvent.Subscribe(PlayerEventType.OpenInventory, () =>
                {
                    canAction = false;
                    reticle.gameObject.SetActive(false); 
                });

                playerEvent.Subscribe(PlayerEventType.CloseInventory, () =>
                {
                    canAction = true;
                    reticle.gameObject.SetActive(true);
                });
            }
        }

        public void OnDisable()
        {
            if (ammoCounter)
                ammoCounter.Text.enabled = false;

            if (reticle)
                reticle.IsEquipingGun = false;

            if (playerEvent)
            {
                playerEvent.UnSubscribe(PlayerEventType.OpenInventory, () =>
                {
                    canAction = false;
                    reticle.gameObject.SetActive(false);
                });

                playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () =>
                {
                    canAction = true;
                    reticle.gameObject.SetActive(true);
                });
            }
        }
    }
}
