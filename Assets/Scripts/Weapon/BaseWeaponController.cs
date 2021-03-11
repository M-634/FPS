using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    ///武器報情報をまとめたクラス
    /// </summary>
    [System.Serializable]
    public class WeaponGeneralData
    {
        public KindOfItem KindOfItem;
        public Transform[] muzzles;
        public BulletControl bullet;

        [HideInInspector]
        public int muzzlesIndex;

        public float shotPower;
        public float shotDamage;
        public float shotRateTime;

        public int maxAmmo;
        public float coolTime;
        public float ammoLifeTime;

        public AudioClip shotClip;
    }

    /// <summary>
    /// 武器はプレイヤーに持たせた状態ででアクティブを切る
    /// </summary>
    public class BaseWeaponController : MonoBehaviour
    {
        public WeaponGeneralData data;
        BulletControl[] arraysOfAmmo;
     
        AudioSource audioSource;
        float rateTimer;
        float coolTimeCount;

        public virtual int CurrentAmmo { get; set; }

        public bool IsCoolTime { get; set; }

        protected virtual void Start()
        {
            audioSource = GetComponent<AudioSource>();
            //PoolingAmmo();
            ReLoad();
            gameObject.SetActive(false);
        }

        public void ReLoad()
        {
            CurrentAmmo = data.maxAmmo;
            IsCoolTime = false;
            coolTimeCount = 0f;
        }

        void PoolingAmmo()
        {
            arraysOfAmmo = new BulletControl[data.maxAmmo];
            for (int i = 0; i < data.maxAmmo; i++)
            {
                var ammo = Instantiate(data.bullet, data.muzzles[data.muzzlesIndex].position, Quaternion.identity);
                data.muzzlesIndex = (data.muzzlesIndex + 1) % data.muzzles.Length;
                ammo.transform.parent = transform;
                ammo.gameObject.SetActive(false);
                arraysOfAmmo[i] = ammo;
            }
        }

        void InstantiateAmmoFromPool()
        {
            foreach (var bullet in arraysOfAmmo)
            {
                if (bullet.gameObject.activeSelf == false)
                {
                    data.muzzlesIndex = (data.muzzlesIndex + 1) % data.muzzles.Length;

                    //Fire!!
                    bullet.gameObject.SetActive(true);
                    bullet.Fire(data);

                    //play shot
                    if (audioSource)
                        audioSource.PlayOneShot(data.shotClip);
                    CurrentAmmo--;
                    return;
                }
            }
        }

        public void TryShot()
        {
            if (CurrentAmmo <= 0)
            {
                IsCoolTime = true;
                CurrentAmmo = 0;
                return;
            }
            Shot();
        }

        public void Shot()
        {
            rateTimer += Time.deltaTime;
            if (rateTimer > data.shotRateTime)
            {
                rateTimer = 0f;
                InstantiateAmmoFromPool();
            }
        }


        public void UpdateAmmo()
        {
            if (!IsCoolTime) return;

            coolTimeCount += Time.deltaTime;
            if (coolTimeCount > data.coolTime)
            {
                CurrentAmmo++;
                if (CurrentAmmo > data.maxAmmo)
                {
                    ReLoad();
                }
            }
        }
    }
}
