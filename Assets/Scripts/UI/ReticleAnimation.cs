﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 銃を持った時に表示するレティクルのアニメーションを制御するクラス
/// エイム時と銃を持っていない時は、デフォルト(真ん中に白い点)にする。
/// </summary>
public class ReticleAnimation : MonoBehaviour
{
    [SerializeField] Image defult;
    [SerializeField] Image top, bottom, left, right;
    [SerializeField] float startPosValue = 40f;
    [SerializeField] float shiftAmount;
    [SerializeField] float coolSpeed;

    public bool IsShot { get; set; }

    bool isEquipingGun = false;
    public bool IsEquipingGun
    {
        get { return isEquipingGun; }
        set
        {
            defult.enabled = !value;
            top.enabled = value;
            bottom.enabled = value;
            left.enabled = value;
            right.enabled = value;
            isEquipingGun = value;
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!IsEquipingGun) return;

        top.rectTransform.anchoredPosition = new Vector2(top.rectTransform.anchoredPosition.x, Mathf.Lerp(top.rectTransform.anchoredPosition.y, startPosValue, Time.deltaTime * coolSpeed));
        bottom.rectTransform.anchoredPosition = new Vector2(bottom.rectTransform.anchoredPosition.x, Mathf.Lerp(bottom.rectTransform.anchoredPosition.y, -startPosValue, Time.deltaTime * coolSpeed));
        left.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(left.rectTransform.anchoredPosition.x, -startPosValue, Time.deltaTime * coolSpeed), left.rectTransform.anchoredPosition.y);
        right.rectTransform.anchoredPosition = new Vector2(Mathf.Lerp(right.rectTransform.anchoredPosition.x, startPosValue, Time.deltaTime * coolSpeed), right.rectTransform.anchoredPosition.y);

        if (IsShot)
        {
            top.rectTransform.anchoredPosition = new Vector2(0, shiftAmount);
            bottom.rectTransform.anchoredPosition = new Vector2(0, -shiftAmount);
            left.rectTransform.anchoredPosition = new Vector2(-shiftAmount, 0);
            right.rectTransform.anchoredPosition = new Vector2(shiftAmount, 0);

            IsShot = false;
        }
    }
}