using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 落ちている武器にアタッチするクラス
    /// プレイヤーに装備するものと別とする
    /// </summary>
    public class PickUpWeapon : BaseItem
    {
        public override void OnPicked()
        {
            base.OnPicked();
        }

        /// <summary>
        /// プレイヤーに装備する。
        /// </summary>
        /// <returns></returns>
        public override bool CanUseItem()
        {
            var interactive = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteractive>();
            interactive.EquipmentWeapon(kindOfitem);
            Destroy(gameObject, 0.1f);
            return false;
        }
    }
}