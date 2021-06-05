﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.VFX
{
    /// <summary>
    /// VFXの中でオブジェクトプールを適用するオブジェクトにアタッチする
    /// </summary>
    public class VFXPoolControl : MonoBehaviour
    {
        [SerializeField] int lifeTime = 1;
        bool init = true;//オブジェットプールで初期化される時は何もしない

        private void OnEnable()
        {
            if(init == false)
            {
                gameObject.SetActive(false,lifeTime);
            }
            init = false;
        }
    }
}