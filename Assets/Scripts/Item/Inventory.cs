using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class Inventory : SingletonMonoBehaviour<Inventory>
    {
        [SerializeField] ItemDataBase itemDataBase;
        [SerializeField] Slot[] slots = new Slot[5];
        public Slot[] Slots { get => slots; }

        int slotNumber = -1;
        private void Update()
        {
            slotNumber = PlayerInputManager.SlotAction();

            if (slotNumber != -1)
            {
                slots[slotNumber].UseItemInSlot();
                slotNumber = -1;
            } 
        }

        /// <summary>
        /// 取得したアイテムがデータベースに存在するか確認する。
        /// データベースにあったら、 アイテムを拾うことができるか判定する
        /// </summary>
        /// <param name="getItem"></param>
        /// <returns></returns>
        public bool CanGetItem(BaseItem getItem)
        {
            foreach (var itemData in itemDataBase.ItemDataList)
            {
                if (itemData.KindOfItem == getItem.kindOfitem)
                {
                    return SearchSlot(itemData);
                }
            }
            Debug.LogWarning("データベースに該当するアイテムがありません");
            return false;
        }

        /// <summary>
        /// 既にスロット内にあるアイテムかどうか調べる。
        /// ないなら左側から順番に埋めていく
        /// </summary>
        /// <returns></returns>
        private bool SearchSlot(ItemData getItemData)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                //スタック数が満タンの時は次のスロットを調べる
                if (slots[i].IsFilled)
                {
                    continue;
                }

                //同一アイテムがスロット内にあり,かつスタック数が満タンではない時はスタック数を足していく
                if (!slots[i].IsEmpty && slots[i].CurrentItemData.KindOfItem == getItemData.KindOfItem && !slots[i].IsFilled)
                {
                    slots[i].AddItemInSlot();
                    return true;
                }

                //スロットが空なら、アイテムデータをセットする
                if (slots[i].IsEmpty)
                {
                    slots[i].SetInfo(getItemData);
                    return true;
                }
            }
            return false;//全てのスロットが埋まっている
        }
    }
}
