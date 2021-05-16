using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    /// <summary>
    ///現在所持している武器の残段数と減り具合、武器アイコンをUIに表示するクラス
    /// </summary>
    public class CurrentWeaponAmmoCounter : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI ammoCounterText;
        [SerializeField] Image ammoCounterSllider;
        [SerializeField] Image weaponIcon;
        [SerializeField] GameObject ui;
        public TextMeshProUGUI AmmoCounterText => ammoCounterText;
        public Image AmmoCounterSllider => ammoCounterSllider;
        public Image WeaponIcon => weaponIcon;
        public GameObject UIContents => ui;

        private void Awake()
        {
            ui.SetActive(false);
        }

        public void Init()
        {
            AmmoCounterText.text = "";
            AmmoCounterSllider.fillAmount = 1;
            WeaponIcon.sprite = null;
        }
    }
}

