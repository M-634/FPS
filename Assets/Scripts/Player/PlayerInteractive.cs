using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテムを扱う時に制御するクラス
    /// </summary>
    public class PlayerInteractive : MonoBehaviour
    {
        [SerializeField] Transform equipPosition;
        [SerializeField] BaseWeapon[] equipWeapons;
        [SerializeField] LayerMask pickUpLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;

        public BaseWeapon CurrentHaveWeapon { get; private set; }
        int currentWeaponIndex = -1;
        RaycastHit hit;

        private void Update()
        {
            if (CheakPickUpObj() && PlayerInputManager.InteractiveAction())
            {
                if (hit.collider.TryGetComponent(out IPickUpObjectable pickUpObjectable))
                {
                    pickUpObjectable.OnPicked();
                }
            }

            var key = PlayerInputManager.SwichWeaponAction();
            if(key == -1)
                return;
            if (key == 2)
                RemoveEquipment();
            EquipmentWeaponByShotCutKeyOrInventory(key);
        }

        private bool CheakPickUpObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, pickUpLayer))
            {
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            InteractiveMessage.CloseMessage();
            return false;
        }

        /// <summary>
        ///武器を拾った際に、武器を装備する。装備中の武器があるならアクティブをfalseにする
        /// </summary>
        /// <param name="index"></param>
        /// <param name="getWeapon"></param>
        public void EquipmentWeapon(int index, BaseWeapon getWeapon)
        {
            var go = getWeapon;
            equipWeapons[index] = go;
            go.transform.SetParent(equipPosition);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.Euler(Vector3.zero);

            //装備中の武器があるなら装備中のアクティブをfalseにする
            if (CurrentHaveWeapon)
            {
                equipWeapons[currentWeaponIndex].gameObject.SetActive(false);
            }
            //現在装備中のものを更新
            CurrentHaveWeapon = go;
            currentWeaponIndex = index;
        }

        /// <summary>
        /// 武器を装備するショートカットを押されたか、インベントリを開いてスロットをクリックしたら呼ばれる関数
        /// </summary>
        /// <param name="index"></param>
        public void EquipmentWeaponByShotCutKeyOrInventory(int index)
        {
            //指定したスロットに装備武器がないか、装備中の武器と同じスロットを呼ばれたら何もしない
            if (equipWeapons[index] == null || index == currentWeaponIndex) return;

            //装備中の武器があるなら装備中のアクティブをfalseにする
            if (CurrentHaveWeapon)
            {
                equipWeapons[currentWeaponIndex].gameObject.SetActive(false);
            }

            equipWeapons[index].gameObject.SetActive(true);
            CurrentHaveWeapon = equipWeapons[index];
            currentWeaponIndex = index;
        }

        /// <summary>
        /// 武器を拾う際に、武器スロットがいっぱいで、装備中だったら装備中の武器と拾った武器を交換する
        /// </summary>
        public void ChangeWeapon(BaseWeapon getWeapon)
        {
            //装備中の武器のスロットデータと武器を捨てる
            CurrentHaveWeapon = null;
            equipWeapons[currentWeaponIndex].transform.SetParent(null);
            equipWeapons[currentWeaponIndex].Drop();
            Inventory.Instance.WeaPonSlots[currentWeaponIndex].ResetInfo();
            //拾った武器を装備する
            EquipmentWeapon(currentWeaponIndex, getWeapon);
        }

        public void RemoveEquipment()
        {
            //現在装備していれば装備を外す
            if (currentWeaponIndex != -1)
            {
                equipWeapons[currentWeaponIndex].gameObject.SetActive(false);
                CurrentHaveWeapon = null;
                currentWeaponIndex = -1;
            }
        }
    }
}
