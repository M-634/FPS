using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Musashi
{
    public class AmmoBox : BaseItem
    {
        public int addAmmoNumber = 60;
        public override void OnPicked()
        {
     
            canPickUp = ItemInventory.Instance.CanGetItem(this,1);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                //AmmoCounterに渡す
                EventManeger.Instance.Excute(addAmmoNumber);
                Destroy(gameObject);
            }
        }
    }
}
