using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// Animatorのアクティブをいじるとアニメーションがバクるので、
    /// アクティブをいじるオブジェットを指定し、管理するクラス。
    /// </summary>
    public class WeaponActiveControl : MonoBehaviour
    {

        public WeaponType kindOfWeapon;

        [SerializeField] GameObject[] switchActiveObjects;
        [SerializeField] WeaponGunControl control;
        [SerializeField] AudioSource audioSource;

        public bool ActiveSelf { get; private set; }

        private void Awake()
        {
            kindOfWeapon = control.weaponType;
        }

        public void SetActive(bool value)
        {
            foreach (var item in switchActiveObjects)
            {
                item.SetActive(value);
            }

            control.enabled = value;
            audioSource.enabled = value;
            ActiveSelf = value;
        }

    }
}