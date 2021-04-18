using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Musashi
{
    public class AmmoBox : BaseItem
    {
        public int addAmmoNumber = 60;
        public override void OnPicked(GameObject player)
        {
            canPickUp = player.GetComponent<PlayerItemInventory>().CanGetItem(this, 1);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                //AmmoCounterに渡す
                player.GetComponentInChildren<AmmoCounter>().AddSumOfAmmo(addAmmoNumber);
                Destroy(gameObject);
            }
        }
    }
}
