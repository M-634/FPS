using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    /// <summary>
    /// 銃を持った時に弾薬の数を表示する。
    /// 所持している銃の装填数 | インベントリに持っている銃弾の数
    /// </summary>
    public class AmmoCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        public TextMeshProUGUI Text => text;
        public int sumNumberOfAmmoInInventory = 0;//publicはテスト用

        private void Awake()
        {
            text.enabled = false;
        }

        public void Display(ref int ammoNum)
        {
            text.enabled = true;
            text.text = ammoNum.ToString() + " | " + sumNumberOfAmmoInInventory.ToString();
        }

        /// <summary>
        /// 弾薬ボックスを拾ったか、銃を拾ったら弾薬所持数を更新する。
        /// イベントから呼ばれる
        /// </summary>
        public void AddSumOfAmmo(int getAmmoNumber)
        {
            sumNumberOfAmmoInInventory += getAmmoNumber;
        }

   
        /// <summary>
        /// リロードできるかどうか判定する関数
        /// </summary>
        /// <param name="maxAmmo">各銃の最大装填数</param>
        /// <param name="currentAmmo">現在の弾の装填数</param>
        public  bool CanReloadAmmo(int maxAmmo,int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;
            if(sumNumberOfAmmoInInventory - diff >= 0)
            {
                sumNumberOfAmmoInInventory -= diff;
                Display(ref maxAmmo);
                return true;
            }
            return false;
        }
        //private void OnEnable()
        //{
        //    EventManeger.Instance.Subscribe(AddSumOfAmmo);
        //}

        //public void OnDisable()
        //{
        //    EventManeger.Instance.UnSubscribe(AddSumOfAmmo);

        //}
    }
}

