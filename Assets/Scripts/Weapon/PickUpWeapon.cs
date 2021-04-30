using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 落ちている武器にアタッチするクラス
    /// プレイヤーに装備するものと別とする
    /// </summary>
    public class PickUpWeapon : CanPickUpObject
    {
        public WeaponType kindOfWeapon;
     
        public override void OnPicked(GameObject player)
        {
            var canPickUp = player.GetComponent<PlayerWeaponManager>().CanPickUP(this);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                Destroy(gameObject);
            }
        }
    }
}