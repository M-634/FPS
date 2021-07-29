using UnityEngine;
using Musashi.Weapon;

namespace Musashi.Item
{
    [RequireComponent(typeof(PickUp))]
    public class WeaponPickup : MonoBehaviour 
    {
        [SerializeField] WeaponControl weaponPrefab;

        PickUp pickUp;

        private void Start()
        {
            pickUp = GetComponent<PickUp>();

            pickUp.OnPickEvent += PickUp_OnPickEvent;
        }

        private void PickUp_OnPickEvent(Transform player)
        {
            if(player.TryGetComponent(out Player.PlayerWeaponManager weaponManager))
            {
                if (weaponManager.AddWeapon(weaponPrefab))
                {
                    pickUp.HavePicked = true;
                    Destroy(gameObject);
                }
            }
        }
    }
}
