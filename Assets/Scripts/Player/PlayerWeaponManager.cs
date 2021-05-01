using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器を装備したり、切り替えることを管理するクラス
    /// </summary>
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponDataBase weaponDataBase;//要らない
        /// <summary>playerの子どもに存在する武器。使用時にアクティブを切り替える</summary>
        [SerializeField] WeaponActiveControl[] activeControls;
        [SerializeField] WeaponSlot[] weaponSlots;

        int currentEquipmentActiveWeaponsIndex = -1;//active
        int currentEquipmentWeaponSlotIndex = -1;//slot
        bool canInputAction = true;

        PlayerInputManager playerInput;

        public bool IsEquipmentWeapon => currentEquipmentActiveWeaponsIndex != -1;

        private void Start()
        {
            playerInput = GetComponent<PlayerInputManager>();
    
            foreach (var item in activeControls)
            {
                item.SetActive(false);
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
            if (i == -1 || i == currentEquipmentWeaponSlotIndex) return;
            EquipWeapon(i);
        }

        /// <summary>
        /// 武器のデータベースに存在するか確認する関数。
        /// </summary>
        /// <param name="getWeapon"></param>
        /// <returns></returns>
        public bool CanGetItem(Item getWeapon)
        {
            foreach (var weaponData in weaponDataBase.WeaponDataList)
            {
                if (getWeapon.ItemName == weaponData.weaponName)
                {
                    return CanEquipWeapon(getWeapon);
                }
            }
            Debug.LogWarning("武器データに存在しないため拾えません");
            return false;
        }

        /// <summary>
        /// 拾える武器が装備することができるか確認する関数
        /// </summary>
        /// <param name="weaponData"></param>
        /// <returns></returns>
        public bool CanEquipWeapon(Item getWeapon)
        {
            //スロットに空きがあるか調べる
            for (int i = 0; i < weaponSlots.Length; i++)
            {
                if (weaponSlots[i].IsEmpty)
                {
                   // weaponSlots[i].SetInfo(weaponData);
                    EquipWeapon(i);
                    return true;
                }
            }


            //装備中の武器と拾った武器を交換する
            if (IsEquipmentWeapon)
            {
                int temp = currentEquipmentWeaponSlotIndex;
                weaponSlots[temp].DropObject();
               // weaponSlots[temp].SetInfo(weaponData);
                EquipWeapon(temp);
                return true;
            }

            //スロットに空きがなく、交換も出来ないので装備出来ない⇒拾えない
            return false;
        }

        /// <summary>
        /// スロットのインデックスを指定して、スロット内の武器データとactiveWeaponsの武器データを照合し、武器を装備する
        /// </summary>
        /// <param name="index">スロットのインデックス</param>
        public void EquipWeapon(int index)
        {
            EquipmentWeaponSetActiveFalse();

            if (weaponSlots[index].IsEmpty) return;

            for (int i = 0; i < activeControls.Length; i++)
            {
                //if(weaponSlots[index].CurrentWeaponData.KindOfWeapon == activeControls[i].kindOfWeapon)
                //{
                //    activeControls[i].SetActive(true);
                //    currentEquipmentActiveWeaponsIndex = i;
                //    currentEquipmentWeaponSlotIndex = index;
                //    return;
                //}
            }
            Debug.LogError("activeWeaponsに装備したい武器が存在しません。インスペクターにアタッチしてないと思われる");
        }

        /// <summary>
        /// 装備中の武器のアクティブを切り、各インデックスをリセットする
        /// </summary>
        public void EquipmentWeaponSetActiveFalse()
        {
            if (IsEquipmentWeapon)
            {
                var haveWeapon = activeControls[currentEquipmentWeaponSlotIndex];
                haveWeapon.GetComponent<WeaponGunControl>().CancelAnimation();//memo: WeaponGunControl → 抽象化
                haveWeapon.SetActive(false);
                currentEquipmentActiveWeaponsIndex = -1;
                currentEquipmentWeaponSlotIndex = -1;
            }
        }

        private void ReciveInventoryEvent(bool value)
        {
            canInputAction = value;
            if (IsEquipmentWeapon)
            {
                activeControls[currentEquipmentActiveWeaponsIndex].SetActive(value);
            }
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
