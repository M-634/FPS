using UnityEngine;
using Musashi.Player;

namespace Musashi.Item
{
    [RequireComponent(typeof(PickUp))]
    public class AmmoItem : BaseItem 
    {
        PickUp pickUp;

        private void Start()
        {
            pickUp = GetComponent<PickUp>();
            pickUp.OnPickEvent += PickUp_OnPickEvent;
        }

        private void PickUp_OnPickEvent(Transform player)
        {
            Ower = player;
            if (player.TryGetComponent(out PlayerItemInventory inventory))
            {
                if (inventory.AddItem(this))
                {
                    Destroy(gameObject);
                }
            }
        }
    }

}
