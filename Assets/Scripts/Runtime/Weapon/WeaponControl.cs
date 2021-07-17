using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Item;
using Musashi.Player;
using UnityEngine.UI;
using System;

namespace Musashi.Weapon
{
    public enum WeaponType
    {
        ShotGun, HandGun, AssaultRifle,
    }

    public enum WeaponShootType
    {
        Manual, Automatic,
    }

    /// <summary>
    /// プレイヤーの入力処理に応じて武器を制御するクラス
    /// </summary>
    public class WeaponControl : MonoBehaviour, IPoolUser<WeaponControl>
    {
        #region Field

        #region Gun settings from WeaponSettingSOData 
        //general settings
        [HideInInspector]
        public string weaponName;
        [HideInInspector]
        public WeaponType weaponType;
        //Main settins
        private float shotDamage;
        private float shotPower;
        private float fireRate;
        // private Vector2 recoill;
        private int ammmoAndMuzzleFlashPoolsize;
        //effects
        private ParticleSystem muzzleFalsh;
        private AudioClip shotSFX, reloadSFX, emptySFX;
        private AudioClip shotgunLoadingSFX;
        //ammo settings
        private BulletControl bullet;
        private int maxAmmo;
        //Properties
        public float AimCameraFOV { get; private set; }
        public float AimSpeed { get; private set; }
        #endregion

        [Header("Reference pick up weapon from Id")]
        [SerializeField] WeaponSettingSOData weaponSetting;
        [Header("Manual is single shot. Automatic is rapid fire")]
        [SerializeField] WeaponShootType weaponShootType;
        [Header("Display weapon icon on UI")]
        [SerializeField] Image weaponIcon;
        [Header("Set each Transform")]
        [SerializeField] Transform muzzle;
        [SerializeField] Transform poolObjectParent;
        [Header("Require Component")]
        [SerializeField] ReticleAnimation reticle;
        [SerializeField] HitVFXManager hitVFXManager;

        /// <summary>If Ammo number changed, Invoke this events</summary>
        public event Action OnChangedAmmo;

        int currentAmmo = 0;
        float lastTimeShot = Mathf.NegativeInfinity;
        bool canAction = true;//「銃を撃つ」、「リロードする」といったアクションができるかどうか判定する変数（例:インベントリを開いた状態では撃てないし、リロードできない）

        PoolObjectManager poolObjectManager;
        Animator animator;
        AudioSource audioSource;
        #endregion

        #region Property
        public Image GetIcon => weaponIcon;
        public WeaponShootType GetWeaponShootType => weaponShootType;
        public int MaxAmmo => maxAmmo;
        public int CurrentAmmo
        {
            get => currentAmmo;
            set
            {
                currentAmmo = value;
                if (currentAmmo > maxAmmo)
                {
                    currentAmmo = maxAmmo;
                }

                if (OnChangedAmmo != null)
                {
                    OnChangedAmmo.Invoke();
                }
            }
        }
        #endregion

        #region Method
        private void Awake()
        {
            SetDataFromWeaponSettingSOData();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();

            if (!muzzle)
            {
                muzzle = this.transform;
            }

            poolObjectParent = GameObject.FindGameObjectWithTag("ObjectPoolParent").transform;

            currentAmmo = maxAmmo;

            InitializePoolObject(ammmoAndMuzzleFlashPoolsize);
        }

        /// <summary>
        /// スクリプタブルオブジェクトからデータをセットする関数
        /// </summary>
        private void SetDataFromWeaponSettingSOData()
        {
            if (weaponSetting == null)
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
            if (weaponType == WeaponType.ShotGun)
            {
                shotgunLoadingSFX = weaponSetting.shotgunLoadingSFX;
            }
            //set weapon statas
            shotDamage = weaponSetting.shotDamage;
            shotPower = weaponSetting.shotPower;
            fireRate = weaponSetting.fireRate;
            //recoill = weaponSetting.recoil;
            //set aim settings
            AimCameraFOV = weaponSetting.aimCameraFOV;
            AimSpeed = weaponSetting.aimSpeed;
            //set ammo settings;
            bullet = weaponSetting.bullet;
            maxAmmo = weaponSetting.maxAmmo;
            //set pool size
            ammmoAndMuzzleFlashPoolsize = weaponSetting.ammoAndMuzzleFlashPoolSize;
        }

        public void InitializePoolObject(int poolSize = 1)
        {
            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < poolSize; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObj = poolObjectManager.InstantiatePoolObj();

            var b = Instantiate(bullet, poolObjectParent);
            var mF = Instantiate(muzzleFalsh, poolObjectParent);

            poolObj.AddObj(b.gameObject);
            poolObj.AddObj(mF.gameObject);

            b.SetInfo(shotPower, shotDamage, hitVFXManager);
            poolObj.SetActiveAll(false);
            return poolObj;
        }

        /// <summary>
        /// リロードできるかどうか判定する関数
        /// </summary>
        private void CanReload()
        {
            bool canReload = ItemInventory.Instance ? ItemInventory.Instance.CanReloadAmmo(maxAmmo, currentAmmo) : true;
            if (canReload)
            {
                if (animator)
                    animator.Play("Reload");
                else
                    CurrentAmmo = maxAmmo;


                if (audioSource && weaponType != WeaponType.ShotGun)
                    audioSource.Play(reloadSFX, audioSource.volume);
            }
            else
            {
                //弾切れ！
                audioSource.Play(emptySFX);
            }
        }

        /// <summary>
        /// リロードアニメーションイベントから呼ばれる関数
        /// </summary>
        public void EndReload()
        {
            if (ItemInventory.Instance)
            {
                CurrentAmmo = ItemInventory.Instance.ReloadAmmoNumber(maxAmmo, currentAmmo);
            }
            else
            {
                CurrentAmmo = maxAmmo;
            }
        }

        /// <summary>
        /// ショットガンのリロードアニメーションイベントから呼ばれる関数
        /// </summary>
        public void ShutGunCycleReload()
        {
            bool canReload = ItemInventory.Instance ? ItemInventory.Instance.CanReloadAmmo(maxAmmo, currentAmmo) : true;

            if (canReload)
            {
                if (ItemInventory.Instance)
                {
                    CurrentAmmo += ItemInventory.Instance.ReloadAmmNumber();
                }
                else
                {
                    CurrentAmmo++;
                }

                if (audioSource)
                {
                    audioSource.Play(reloadSFX, audioSource.volume);
                }

                if (CurrentAmmo == maxAmmo)
                {
                    animator.SetBool("ReloadCycleEnd", true);
                }
            }
            else
            {
                animator.SetBool("ReloadCycleEnd", true);
            }
        }


        /// <summary>
        /// ショットガンのPullアニメーションイベントから呼ばれる関数
        /// </summary>
        public void ShutGunPullStart()
        {
            canAction = false;
        }


        /// <summary>
        /// ショットガンのPullアニメーションイベントから呼ばれる関数
        /// </summary>
        public void ShutGunPullEnd()
        {
            canAction = true;
            if (weaponType == WeaponType.ShotGun)
            {
                audioSource.Play(shotgunLoadingSFX);
                animator.SetBool("ReloadCycleEnd", false);
            }
        }

        /// <summary>
        /// 再生中のアニメーションを中断させる関数
        /// </summary>
        public void CancelAnimation()
        {
            animator.Play("Idle");
        }

       
        public void TryShot()
        {
            if (CurrentAmmo < 1)
            {
                CurrentAmmo = 0;
                CanReload();
                return;
            }

            if (Time.time > lastTimeShot + fireRate)
            {
                if (animator)
                {
                    animator.Play("Shot");
                }
                else
                {
                    Shot();
                }
            }
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void Shot()
        {
            poolObjectManager.UsePoolObject(muzzle.position, muzzle.rotation, SetPoolObj);

            if (audioSource)
            {
                audioSource.Play(shotSFX, audioSource.volume);
            }

            CurrentAmmo--;

            if (reticle)
            {
                reticle.IsShot = true;
            }

            lastTimeShot = Time.time;
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる
        /// </summary>
        public void CasingRelease()
        {

        }

        //public void OnEnable()
        //{
        //    if (ItemInventory.Instance)
        //    {
        //        ItemInventory.Instance.DisplayEquipmentWeaponInfo(currentAmmo, maxAmmo);
        //    }

        //    if (reticle)
        //    {
        //        reticle.IsDefult = false;
        //    }
        //}

        //public void OnDisable()
        //{

        //    if (reticle)
        //    {
        //        reticle.IsDefult = true;
        //    }
        //}
        #endregion
    }
}