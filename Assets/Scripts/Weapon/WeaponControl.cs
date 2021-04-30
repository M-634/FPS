using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーの入力処理に応じて武器を制御するクラス
    /// </summary>
    public class WeaponControl : MonoBehaviour,IPoolUser<WeaponControl>
    {
        private enum WeaponShootType
        {
            Manual, Automatic,
        }

        [SerializeField] WeaponSettingSOData weaponSetting;
        [SerializeField] WeaponShootType weaponShootType;

        #region Gun settings from WeaponSettingSOData 
        //general settings
        [SerializeField] string weaponName;
        private WeaponType weaponType;
        //Main settins
        private float shotDamage;
        private float shotPower;
        private float fireRate;
        private Vector2 recoill;
        private float aimCameraFOV;
        private float aimSpeed;
        private int ammmoAndMuzzleFlashPoolsize;
        //effects
        private ParticleSystem muzzleFalsh;
        private AudioClip shotSFX, reloadSFX, emptySFX;
        private AudioClip shotgunLoadingSFX;
        //ammo settings
        private BulletControl bullet;
        private int maxAmmo;
        #endregion

        [Header("Reference pick up weapon from Id")]
        public int ammoID;//拾った武器のIDを参照して、弾を管理する(publicはdebug用)
        public int currentAmmo;//拾った武器のスタック数をセットする

        [Header("Set muzzle")]
        [SerializeField] Transform muzzle;
        [Header("Require Component")]
        [SerializeField] AmmoCounter ammoCounter;
        [SerializeField] ReticleAnimation reticle;

        float lastTimeShot = Mathf.NegativeInfinity;
        bool canAction = true;//「銃を撃つ」、「リロードする」といったアクションができるかどうか判定する変数（例:インベントリを開いた状態では撃てないし、リロードできない）
        bool isAiming = false;


        PoolObjectManager poolObjectManager;
        PlayerInputManager playerInput;
        PlayerCamaraControl playerCamara;
        PlayerEventManager playerEvent;
        Animator animator;
        AudioSource audioSource;

        private void Awake()
        {
            SetDataFromWeaponSettingSOData();
            playerInput = transform.GetComponentInParent<PlayerInputManager>();
            playerCamara = transform.GetComponentInParent<PlayerCamaraControl>();

            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            if (!muzzle)
                muzzle = this.transform;

            currentAmmo = maxAmmo;

            InitializePoolObject(ammmoAndMuzzleFlashPoolsize);
        }

        /// <summary>
        /// スクリプタブルオブジェクトからデータをセットする関数
        /// </summary>
        private void SetDataFromWeaponSettingSOData()
        {
            if(weaponSetting == null)
            {
                Debug.LogError("武器データがアサインされていません！");
                return;
            }
            //set general settings
            weaponName = weaponSetting.weaponName;
            weaponType = weaponSetting.weaponType;
            //set effects settings
            muzzleFalsh = weaponSetting.muzzleFlash;
            shotSFX = weaponSetting.shotSFX;
            reloadSFX = weaponSetting.reloadSFX;
            emptySFX = weaponSetting.emptySFX;
            if(weaponType == WeaponType.ShotGun)
            {
                shotgunLoadingSFX = weaponSetting.shotgunLoadingSFX;
            }
            //set weapon statas
            shotDamage = weaponSetting.shotDamage;
            shotPower = weaponSetting.shotPower;
            fireRate = weaponSetting.fireRate;
            recoill = weaponSetting.recoil;
            //set aim settings
            aimCameraFOV = weaponSetting.aimCameraFOV;
            aimSpeed = weaponSetting.aimSpeed;
            //set ammo settings;
            bullet = weaponSetting.bullet;
            maxAmmo = weaponSetting.maxAmmo;
            //set pool size
            ammmoAndMuzzleFlashPoolsize = weaponSetting.ammoAndMuzzleFlashPoolSize;
        }

        public void InitializePoolObject(int num = 1)
        {
            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < num; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObj = poolObjectManager.InstantiatePoolObj();

            var b = Instantiate(bullet, transform);
            var mF = Instantiate(muzzleFalsh, transform);

            poolObj.AddObj(b.gameObject);
            poolObj.AddObj(mF.gameObject);

            b.SetInfo(ref shotPower, ref shotDamage);
            poolObj.SetActiveAll(false);
            return poolObj;
        }

        /// <summary>
        /// リロードできるかどうか判定する関数
        /// </summary>
        private void CanReload()
        {
            bool canReload = ammoCounter ? ammoCounter.CanReloadAmmo(ref maxAmmo, ref currentAmmo) : true;
            if (canReload)
            {
                if (animator)
                    animator.Play("Reload");
                else
                    currentAmmo = maxAmmo;

                if (audioSource)
                    audioSource.Play(reloadSFX, audioSource.volume);
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

        /// <summary>
        ///アニメーションイベントから呼ばれる関数
        ///単発銃のリロード
        /// </summary>
        public void AutoBeginningRelod()
        {

        }

        /// <summary>
        /// アニメーションイベントから呼ばれる関数。
        /// 単発銃のリロード終了
        /// </summary>
        public void AutoEndRelod()
        {

        }

        /// <summary>
        /// 再生中のアニメーションを中断させる関数
        /// </summary>
        public void CancelAnimation()
        {
            animator.Play("Idle");
        }


        void Update()
        {
            if (!canAction) return;

            if (playerInput.Reload) CanReload();

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
                CanReload();
                return;
            }

            if (lastTimeShot + fireRate < Time.time)
            {
                if (animator)
                    animator.Play("Shot");
            }
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void Shot()
        {
            poolObjectManager.UsePoolObject(muzzle.position, muzzle.rotation, () => SetPoolObj());

            if (audioSource)
                audioSource.Play(shotSFX, audioSource.volume);

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