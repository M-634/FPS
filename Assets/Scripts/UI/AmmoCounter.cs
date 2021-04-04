using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Musashi
{
    /// <summary>
    /// 銃を持った時に弾薬の数を表示する。
    /// 各銃に装填されている弾数 | インベントリに所持している弾の総数
    /// </summary>
    public class AmmoCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;
        public TextMeshProUGUI Text => text;
        int numberWithHavingGun;
        int sumNumberInInventory;

        private void Awake()
        {
            Display();
            text.enabled = false;
        }

        public void Display()
        {
            text.text = numberWithHavingGun.ToString() + " | " + sumNumberInInventory.ToString(); 
        }

        /// <summary>
        /// 弾薬ボックスを拾ったか、銃を拾ったら弾薬所持数を更新する。
        /// イベントから呼ばれる
        /// </summary>
        public void AddSumOfAmmo(int getAmmoNumber)
        {
            sumNumberInInventory += getAmmoNumber;
            Display();
        }

        /// <summary>
        /// 現在持っている銃の装填数を設定する関数
        /// </summary>
        public void SetCurrentAmmo(int currentAmmo)
        {
            numberWithHavingGun = currentAmmo;
            Display();
        }

        /// <summary>
        /// 銃を撃った時に呼ばれる関数
        /// </summary>
        public void UseAmmo()
        {
            numberWithHavingGun--;
            if (numberWithHavingGun < 0)
                numberWithHavingGun = 0;
            Display();
        }

        /// <summary>
        /// リロード時に呼ばれる関数。
        /// リロードできた弾の数を返す。
        /// </summary>
        /// <param name="maxAmmo">各銃の最大装填数</param>
        /// <param name="currentAmmo">現在の弾の装填数</param>
        public int ReloadAmmo(int maxAmmo,int currentAmmo)
        {
            int diff = maxAmmo - currentAmmo;
            if(sumNumberInInventory - diff >= 0)
            {
                numberWithHavingGun = maxAmmo;
                sumNumberInInventory -= diff;
            }
            else
            {
                numberWithHavingGun += sumNumberInInventory;
                sumNumberInInventory = 0;
            }
            Display();
            return numberWithHavingGun;
        }
        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(AddSumOfAmmo);
        }

        public void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(AddSumOfAmmo);

        }
    }
}

