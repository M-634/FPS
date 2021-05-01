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
        [SerializeField] GameObject[] switchActiveObjects;
        [SerializeField] WeaponControl control;
        [SerializeField] AudioSource audioSource;

        public WeaponControl Control => control;
        public bool ActiveSelf { get; private set; }

  
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