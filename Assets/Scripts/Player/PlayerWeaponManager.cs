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

        public int CurrentEquipmentWeaponIndex = -1;

        public bool IsEquipmentWeapon => CurrentEquipmentWeaponIndex != -1;

        private void Start()
        {
            foreach (var weapon in activeWeapons)
            {
                weapon.gameObject.SetActive(false);
            }    
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
                if(getWeapon.kindOfWeapon == weaponData.KindOfWeapon)
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
                    if (IsEquipmentWeapon)
                    {
                        //現在装備している武器をしまい、拾った武器を装備する
                        return true;
                    }
                    else
                    {
                        //拾った武器をそのまま装備する
                        return true;
                    }
                }
            }

           
            if (IsEquipmentWeapon)
            {
                //装備中の武器と拾った武器を交換する
                return true; 
            }

            //スロットに空きがなく、交換も出来ないので装備出来ない⇒拾えない
            return false;
        }

        /// <summary>
        ///実際に武器を装備する関数 
        /// </summary>
        public void EquipWeapon(WeaponData weaponData,int index)
        {

        }

    }
}
