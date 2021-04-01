﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 落ちている武器にアタッチするクラス
    /// プレイヤーに装備するものと別とする
    /// </summary>
    public class PickUpWeapon : BaseWeapon,IPickUpObjectable
    {
        Rigidbody rb;
        PlayerWeaponManager weaponManager;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            weaponManager = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerWeaponManager>();
        }
        public void OnPicked()
        {
            var canPicup = weaponManager.CanPickUP(this);
            if (canPicup)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// インベントリからアイテムを捨てる時に呼ばれる関数
        /// </summary>
        public void Drop()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Ground"))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }

    }
}