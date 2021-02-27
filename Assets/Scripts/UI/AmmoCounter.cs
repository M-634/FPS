using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Musashi
{
    public class AmmoCounter : MonoBehaviour
    {
        [SerializeField] Image BackgroundImage;
        [SerializeField] Color defultBackgroundColor;
        [SerializeField] Color flashBackgroundColor;

        [SerializeField] Image FillImageAmmo;

        int maxAmmo;

        public void Init(int maxAmmo) 
        {
            this.maxAmmo = maxAmmo;
            FillImageAmmo.fillAmount = 1f;
        }

        public void UpdateCounter(int value)
        {
            FillImageAmmo.fillAmount = (float)value / maxAmmo;

            if(FillImageAmmo.fillAmount < 0.2f)
            {
                BackgroundImage.color = flashBackgroundColor;
            }
            else 
            {
                BackgroundImage.color = defultBackgroundColor;
            }
        }
    }
}

