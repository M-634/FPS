using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    /// <summary>
    /// インベントリ内の弾薬を管理するクラス
    /// </summary>
    public class AmmoControlInInventory : MonoBehaviour
    {
        //[SerializeField] TextMeshProUGUI text;
        //public TextMeshProUGUI Text => text;
        public int sumNumberOfAmmoInInventory = 0;

        //private void Awake()
        //{
        //    text.enabled = false;
        //}

        //public void Display(int ammoNum)
        //{
        //    text.enabled = true;
        //    text.text = ammoNum.ToString() + " | " + sumNumberOfAmmoInInventory.ToString();
        //}

        /// <summary>
        /// 弾薬ボックスを拾ったか、銃を拾ったら弾薬所持数を更新する。
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
        public  bool CanReloadAmmo(int maxAmmo, int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;
            if (diff <= 0) return false;

            if(sumNumberOfAmmoInInventory - diff >= 0)
            {
                return true;
            }

            if(sumNumberOfAmmoInInventory > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 実際にリロード出来る弾数
        /// </summary>
        /// <returns></returns>
       public int ReloadAmmoNumber(int maxAmmo,  int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;

            if (sumNumberOfAmmoInInventory - diff >= 0)
            {
                sumNumberOfAmmoInInventory -= diff;
                return maxAmmo;
            }

            int temp = currentAmmo + sumNumberOfAmmoInInventory;
            sumNumberOfAmmoInInventory = 0;
            return temp;
        }

        /// <summary>
        /// ショットガンのリロード。一発ずつ行う
        /// </summary>
        /// <returns></returns>
        public int ReloadAmmNumber()
        {
            sumNumberOfAmmoInInventory--;
            if (sumNumberOfAmmoInInventory < 0)
            {
                sumNumberOfAmmoInInventory = 0;
            }
            return 1;
        }
    }
}

