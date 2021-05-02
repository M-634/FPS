﻿using System.Collections;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 武器の攻撃が当たった時の出すエフェクトを関数するクラス
    /// </summary>
    public class HitVFXManager : MonoBehaviour, IPoolUser<HitVFXManager>
    {
        #region field
        [Header("Set VFX")]
        [SerializeField] GameObject decalVFX;
        [SerializeField] GameObject bloodVFX;
        
        [Header("Set pool size")]
        [SerializeField, Range(1, 100)] int poolSize = 1;

        [Header("Set SFX")]
        [SerializeField] AudioClip hitSFX;

        PoolObjectManager poolObjectManager;
        AudioSource audioSource;
        #endregion

        #region Property
        public GameObject DecalVFX => decalVFX;
        public GameObject BloodVFX => bloodVFX;
        public AudioClip HitSFX => hitSFX;
        public AudioSource AudioSource => audioSource;
        public PoolObjectManager PoolObjectManager => poolObjectManager;
        #endregion

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            InitializePoolObject(poolSize);
        }

        public void InitializePoolObject(int poolsize = 1)
        {
            poolObjectManager = new PoolObjectManager();
            for (int i = 0; i < poolsize; i++)
            {
                SetPoolObj();
            }
        }

        public PoolObjectManager.PoolObject SetPoolObj()
        {
            var poolObject = poolObjectManager.InstantiatePoolObj();

            poolObject.AddObj(decalVFX);
            poolObject.AddObj(BloodVFX);

            return poolObject;
        }
    }
}