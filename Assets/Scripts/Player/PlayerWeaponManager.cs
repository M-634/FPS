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
        public bool IsEquipmentWeapon => currentEquipmentActiveWeaponsIndex != -1;
       
        private void Start()
        {
            foreach (var weapon in activeWeapons)
            {
                weapon.gameObject.SetActive(false);
            }

            for (int i = 0; i < weaponSlots.Length; i++)
            {
                weaponSlots[i].SetWeaponSlot(i,this);
            }
        }


        private void Update()
        {
            if (!canInputAction) return;
            var i = PlayerInputManager.SwichWeaponAction();
            if (i == 0 || i == 1) EquipWeapon(i);
            if (i == 2) PutAwayWeapon();
        }

        /// <summary>
        /// 武器のデータベースに存在するか確認する関数。
        /// </summary>
        /// <param name="getWeapon"></param>
        /// <returns></returns>
        public bool CanPickUP(BaseWeapon getWeapon)
        {
            foreach (var weaponData in weaponDataBase.WeaponDataList)
            {
                if (getWeapon.kindOfWeapon == weaponData.KindOfWeapon)
                {
                    return CanEquipWeapon(weaponData);
                }
            }
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
                weaponSlots[currentEquipmentWeaponSlot].ResetInfo();
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
            if (weaponSlots[index].IsEmpty) return;
            PutAwayWeapon();

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

        public void PutAwayWeapon()
        {
            //装備中の武器をしまう
            if (IsEquipmentWeapon)
            {
                activeWeapons[currentEquipmentActiveWeaponsIndex].gameObject.SetActive(false);
                currentEquipmentActiveWeaponsIndex = -1;
                currentEquipmentWeaponSlot = -1;
            }
        }


        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(EventType.OpenInventory, () => canInputAction = false);
            EventManeger.Instance.Subscribe(EventType.CloseInventory, () => canInputAction = true);
        }

        private void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(EventType.OpenInventory, () => canInputAction = false);
            EventManeger.Instance.UnSubscribe(EventType.CloseInventory, () => canInputAction = true);
        }
    }
}
