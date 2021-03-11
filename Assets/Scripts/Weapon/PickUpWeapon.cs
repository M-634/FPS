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

        public override bool CanUseItem()
        {
            return false;
        }
    }
}