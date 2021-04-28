using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// Animatorのアクティブをいじるとアニメーションがバクるので、
    /// アクティブをいじるオブジェットを指定し、管理するクラス。
    /// </summary>
    [RequireComponent(typeof(WeaponGunControl))]
    public class WeaponActiveControl : MonoBehaviour
    {

        public KindOfWeapon kindOfWeapon;

        [SerializeField] GameObject[] gameObjects;
        [SerializeField] WeaponGunControl control;
        [SerializeField] AudioSource audioSource;

        public bool ActiveSelf { get; private set; }

        public void SetActive(bool value)
        {
            foreach (var item in gameObjects)
            {
                item.SetActive(value);
            }

            control.enabled = value;
            audioSource.enabled = value;
            ActiveSelf = value;
        }

    }
}