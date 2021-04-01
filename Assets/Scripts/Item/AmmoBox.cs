using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Musashi
{
    public class AmmoBox : BaseItem
    {
        public override void OnPicked()
        {
            canPickUp = ItemInventory.Instance.CanGetItem(this);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                //AmmoCounterに渡す
                Destroy(gameObject);
            }
        }
    }
}
