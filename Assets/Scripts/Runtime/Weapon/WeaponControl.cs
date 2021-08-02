using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Item;
using Musashi.Player;
using UnityEngine.UI;
using System;
using UnityEngine.Events;

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
    /// プレイヤーが扱う武器を制御するクラス
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

        private float fireRate;
        // private Vector2 recoill;
        private int ammmoAndMuzzleFlashPoolsize;
        //effects
        private ParticleSystem muzzleFalsh;
        private AudioClip shotSFX, reloadSFX, emptySFX;
        private AudioClip shotgunLoadingSFX;
        //Projectile settings
        [SerializeField, HideInInspector]
        ObjectPoolingProjectileInfo projectileInfo;
        private ProjectileControl projectilePrefab;
        private int maxAmmo;
        //Properties
        public float AimCameraFOV { get; private set; }
        public float AimSpeed { get; private set; }
        #endregion

        [Header("Reference pick up weapon from Id")]
        [SerializeField] WeaponSettingSOData weaponSetting;
        [Header("Manual is single shot. Automatic is rapid fire")]
        [SerializeField] WeaponShootType weaponShootType;
        [Header("The root object for the weapon, this is what will change acitive")]
        [SerializeField] GameObject weaponRoot;
        [Header("Display weapon icon on UI")]
        [SerializeField] Sprite weaponIcon;
        [Header("Set each Transform")]
        [SerializeField] Transform muzzle;

        [SerializeField] Vector3 defultLoaclPostionOffset;
        [SerializeField] Vector3 aimLocalPostionOffset;

        /// <summary>If Ammo number changed, Invoke this events</summary>
        public event Action OnChangedAmmo;
        public event Func<bool> CanReloadAmmo;
        public event Func<int> HaveEndedReloadingAmmo;

        int currentAmmo = 0;
        float lastTimeShot = Mathf.NegativeInfinity;

        Transform poolObjectParent;

        PoolObjectManager poolObjectManager;
        HitVFXManager hitVFXManager;
        Animator animator;
        AudioSource audioSource;
        #endregion

        #region Property
        public bool IsReloading { get; private set; }
        public bool IsPlayingAnimationStateIdle => animator.GetCurrentAnimatorStateInfo(0).IsName("Idle");
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
        public bool IsWeaponActive { get; private set; }
        public Sprite GetIcon => weaponIcon;
        public WeaponShootType GetWeaponShootType => weaponShootType;
        public GameObject SourcePrefab { get; set; }//instanceする前のGameObject
        public Vector3 RootPosition
        {
            get
            {
                return weaponRoot ? weaponRoot.transform.localPosition : transform.localPosition;
            }
            set
            {
                if (weaponRoot)
                {
                    weaponRoot.transform.localPosition = value;
                }
                else
                {
                    transform.localPosition = value;
                }
            }
        }
        public Vector3 GetDefultLocalPositionOffset => defultLoaclPostionOffset;
        public Vector3 GetAimLocalPositionOffset => aimLocalPostionOffset;

        #endregion

        private void Start()
        {
            SetDataFromWeaponSettingSOData();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            hitVFXManager = GetComponentInParent<HitVFXManager>();

            if (!weaponRoot)
            {
                weaponRoot = gameObject;
            }
            if (!muzzle)
            {
                muzzle = this.transform;
            }
            poolObjectParent = GameObject.FindGameObjectWithTag("ObjectPoolParent").transform;//リファクタリングメモ ；Nullチェック！

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
            //set projectile statas
            projectileInfo.damage = weaponSetting.shotDamage;
            projectileInfo.power = weaponSetting.shotPower;
            projectileInfo.lifeTime = weaponSetting.projectileLifeTime;
            projectileInfo.muzzle = this.muzzle;
            //weapon statas
            fireRate = weaponSetting.fireRate;
            //recoill = weaponSetting.recoil;
            //set aim settings
            AimCameraFOV = weaponSetting.aimCameraFOV;
            AimSpeed = weaponSetting.aimSpeed;
            //set ammo settings;
            projectilePrefab = weaponSetting.projectilePrefab;
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

            var p = Instantiate(projectilePrefab, poolObjectParent);
            var mF = Instantiate(muzzleFalsh, poolObjectParent);

            p.SetProjectileInfo(projectileInfo);

            poolObj.AddObj(p.gameObject);
            poolObj.AddObj(mF.gameObject);

            poolObj.SetActiveAll(false);
            return poolObj;
        }

        #region public Methods
        /// <summary>
        /// リロードを始める関数
        /// </summary>
        public void StartReload()
        {
            if (IsReloading || CurrentAmmo == MaxAmmo) return;

            var canReload = CanReloadAmmo.Invoke();
            if (canReload)
            {
                if (animator)
                {
                    IsReloading = true;
                    animator.Play("Reload");
                }
                else
                {
                    CurrentAmmo = HaveEndedReloadingAmmo.Invoke();
                }


                if (audioSource && weaponType != WeaponType.ShotGun)
                {
                    audioSource.Play(reloadSFX, audioSource.volume);
                }
            }
            else
            {
                //弾切れ！
                audioSource.Play(emptySFX);
            }
        }

        /// <summary>
        /// 弾を発射できるか試す関数    
        /// </summary>
        public void TryShot()
        {
            if (CurrentAmmo < 1)
            {
                CurrentAmmo = 0;
                StartReload();
                return;
            }

            if (Time.time > lastTimeShot + fireRate)
            {
                if (animator)
                {
                    CancelAnimation();
                    animator.Play("Shot");
                }
                else
                {
                    Shot();
                }
            }
        }

        public void CancelAnimation()
        {
            if (!animator) return;

            IsReloading = false;
            audioSource.Stop();
            animator.Play("Idle");
        }

        /// <summary>
        /// 武器の表示を制御する関数
        /// </summary>
        /// <param name="value"></param>
        public void ShowWeapon(bool value, UnityAction callBack = null)
        {
            weaponRoot.SetActive(value);
            IsWeaponActive = value;
            if (callBack != null)
            {
                callBack.Invoke();
            }
        }
        #endregion

        #region Animation Events: アニメーションイベントから呼ばれる関数
        /// <summary>
        /// 弾の装填が完了したタイミングで呼ばれるアニメーションイベント関数
        /// </summary>
        public void EndReloadCharge()
        {
            if (weaponType == WeaponType.ShotGun)
            {
                var addAmmo = HaveEndedReloadingAmmo.Invoke();//1 or 0;
                CurrentAmmo += addAmmo;

                if (audioSource)
                {
                    audioSource.Play(reloadSFX, audioSource.volume);
                }

                if (addAmmo == 0 || CurrentAmmo == maxAmmo)
                {
                    animator.SetBool("ReloadCycleEnd", true);
                    return;
                }
            }
            else
            {
                CurrentAmmo = HaveEndedReloadingAmmo.Invoke();
            }
        }

        /// <summary>
        /// リロードアニメーション中にどこから入力を受け付けないかを決めるアニメーションイベント関数
        /// </summary>
        public void ReloadActionStartTrigger()
        {
            IsReloading = true;
        }

        /// <summary>
        /// リロードアニメーション終了時に設定するアニメーションイベント関数。
        /// </summary>
        public void ReloadActionEndTrigger()
        {
            IsReloading = false;
            if (weaponType == WeaponType.ShotGun)
            {
                audioSource.Play(shotgunLoadingSFX);
                animator.SetBool("ReloadCycleEnd", false);
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

            //if (reticle)
            //{
            //    reticle.IsShot = true;
            //}

            lastTimeShot = Time.time;
        }

        /// <summary>
        /// アニメーションイベントから呼ばれる(未実装)
        /// </summary>
        public void CasingRelease()
        {

        }
        #endregion
    }
}