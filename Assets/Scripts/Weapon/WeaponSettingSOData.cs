using UnityEngine;
using System;

namespace Musashi
{
    /// <summary>
    /// 武器データのスクリプタブルオブジェクト
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "WeaponData",menuName = "WeaponSettingData")]
    public class WeaponSettingSOData : ScriptableObject 
    {
        #region Field
        public string weaponName;
        public WeaponType weaponType;
        public Sprite weaponIcon;

        [Header("Settings of objects")]
        public ParticleSystem muzzleFlash;
        public AudioClip shotSFX, reloadSFX, emptySFX;

        //[SerializeField] GameObject shell;
        //[SerializeField] int shellsPoolSize;
        //[SerializeField] float shellEjectintForce;

        [Header("Weapon statas")]
        public float shotDamage;
        public float shotPower;
        public float fireRate;
        public Vector2 recoil;

        [Header("Aim setttings")]
        public float aimCameraFOV;
        public float aimSpeed;

        [Header("Ammo Settings")]
        public BulletControl bullet;
        public int maxAmmo;


        [Header("Setting Pool Size")]
        [Range(1,100)]
        public int ammoAndMuzzleFlashPoolSize = 1;

        public AudioClip shotgunLoadingSFX;
        #endregion
    }
}
