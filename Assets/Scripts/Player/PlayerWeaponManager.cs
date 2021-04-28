using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を装備したり、切り替えることを管理するクラス
    /// </summary>
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponDataBase weaponDataBase;
        /// <summary>playerの子どもに存在する武器。使用時にアクティブを切り替える</summary>
        [SerializeField] BaseWeapon[] activeWeapons;
        [SerializeField] WeaponSlot[] weaponSlots;

        int currentEquipmentActiveWeaponsIndex = -1;
        int currentEquipmentWeaponSlot = -1;
        bool canInputAction = true;

        PlayerInputManager playerInput;

        public bool IsEquipmentWeapon => currentEquipmentActiveWeaponsIndex != -1;

        private void Start()
        {
            playerInput = GetComponent<PlayerInputManager>();
            foreach (var weapon in activeWeapons)
            {
                weapon.gameObject.SetActive(false);
            }

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i].SetInput(playerInput);
                weaponSlots[i].SetWeaponSlot(i, this);
            }
        }


        private void Update()
        {
            if (!canInputAction) return;

            var i = playerInput.SwichWeaponID;
            if (i == -1) return;
            EquipWeapon(i);
        }

        /// <summary>
        /// 武器のデータベースに存在するか確認する関数。
        /// </summary>
        /// <param name="getWeapon"></param>
        /// <returns></returns>
        public bool CanPickUP(PickUpWeapon getWeapon)
        {
            foreach (var weaponData in weaponDataBase.WeaponDataList)
            {
                if (getWeapon.kindOfWeapon == weaponData.KindOfWeapon)
                {
                    return CanEquipWeapon(weaponData);
                }
            }
            Debug.LogWarning("武器データに存在しないため拾えません");
            return false;
        }

        /// <summary>
        /// 武器を装備することができるか確認する関数
        /// </summary>
        /// <param name="weaponData"></param>
        /// <returns></returns>
        public bool CanEquipWeapon(WeaponData weaponData)
        {
            //スロットに空きがあるか調べる
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i].IsEmpty)
                {
                    weaponSlots[i].SetInfo(weaponData);
                    EquipWeapon(i);
                    return true;
                }
            }


            if (IsEquipmentWeapon)
            {
                //装備中の武器と拾った武器を交換する
                PutAwayWeapon();
                weaponSlots[currentEquipmentWeaponSlot].ResetInfo();//error 
                weaponSlots[currentEquipmentWeaponSlot].SetInfo(weaponData);
                EquipWeapon(currentEquipmentWeaponSlot);
                return true;
            }

            //スロットに空きがなく、交換も出来ないので装備出来ない⇒拾えない
            return false;
        }

        /// <summary>
        /// スロットのインデックスを指定して、スロット内の武器データとactiveWeaponsの武器データを照合して、武器を装備する
        /// </summary>
        /// <param name="index">スロットのインデックス</param>
        public void EquipWeapon(int index)
        {
            PutAwayWeapon();
            if (weaponSlots[index].IsEmpty) return;

            for (int i = 0; i < activeWeapons.Length; i++)
            {
                if (weaponSlots[index].CurrentWeaponData.KindOfWeapon == activeWeapons[i].kindOfWeapon)
                {
                    activeWeapons[i].gameObject.SetActive(true);
                    currentEquipmentActiveWeaponsIndex = i;
                    currentEquipmentWeaponSlot = index;
                    return;
                }
            }
            Debug.LogError("activeWeaponsに装備したい武器が存在しません。インスペクターにアタッチしてないと思われる");
        }

        /// <summary>
        /// 装備中の武器を捨てる関数
        /// </summary>
        public void PutAwayWeapon()
        {
            if (IsEquipmentWeapon)
            {
                activeWeapons[currentEquipmentActiveWeaponsIndex].gameObject.SetActive(false);
                currentEquipmentActiveWeaponsIndex = -1;
                currentEquipmentWeaponSlot = -1;
            }
        }

        private void ReciveInventoryEvent(bool value)
        {
            canInputAction = value;
            if (IsEquipmentWeapon)
                activeWeapons[currentEquipmentActiveWeaponsIndex].gameObject.SetActive(value);
        }


        PlayerEventManager playerEvent;
        private void OnEnable()
        {
            playerEvent = GetComponent<PlayerEventManager>();
            if (playerEvent)
            {
                playerEvent.Subscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
                playerEvent.Subscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            }
        }

        private void OnDisable()
        {
            if (playerEvent)
            {
                playerEvent.UnSubscribe(PlayerEventType.OpenInventory, () => ReciveInventoryEvent(false));
                playerEvent.UnSubscribe(PlayerEventType.CloseInventory, () => ReciveInventoryEvent(true));
            }
        }
    }
}
