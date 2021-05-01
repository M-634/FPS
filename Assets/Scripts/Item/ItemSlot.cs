using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    public class ItemSlot : SlotBase 
    {
        public void AddItemInSlot(Item getItem)
        {
            if (IsFilled) return;
            int temp = StacSizeInSlot + getItem.StacSize;

            if (temp > maxStacSizeInSlot)
                temp = maxStacSizeInSlot;

            StacSizeInSlot = temp;
            itemsInSlot.Enqueue(getItem);
            getItem.gameObject.SetActive(false);
            GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
        }
    }
}
