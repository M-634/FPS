using UnityEngine;

namespace Musashi
{
    public class SlotsManager : MonoBehaviour
    {
    //    [SerializeField] Slot[] slots;
    //    public Slot[] Slots { get => slots; }

    

    //    /// <summary>
    //    /// アイテムが拾うことができるかどうか判定し、
    //    /// 拾えるなら、スロットにデータをセットする
    //    /// </summary>
    //    /// <param name="getItemData"></param>
    //    /// <returns></returns>
    //    public bool RegisterItemInSlot(ItemData getItemData)
    //    {
    //        ////既にスロット内にアイテムが存在するかどうか調べる
    //        //var slot = SearchSlot(getItemData);

    //        ////既にあるならスタック数を増やす
    //        //if (isExisted)
    //        //{
    //        //    return slot.AddItem();
    //        //}

    //        ////スロットに存在しないアイテムなら、空いてる所へセットする
    //        //if(slot == null)
    //        //{
    //        //    return false;
    //        //}
    //        //slot.SetInfo(getItemData);
    //        //return true;
    //    }

    //    /// <summary>
    //    /// 既にスロット内にあるアイテムかどうか調べる。
    //    /// ないなら左側から順番に埋めていく
    //    /// </summary>
    //    /// <returns></returns>
    //    public bool SearchSlot(ItemData getItemData)
    //    {
    //        for (int i = 0; i < slots.Length; i++)
    //        {
    //            //スタック数が満タンの時は次のスロットを調べる
    //            if (slots[i].IsFilled)
    //            {
    //                continue;
    //            }

    //            //既に同一アイテムがスロット内にあるが,スタック数が満タンではない時はスタック数を足していく
    //            if(!slots[i].IsEmpty && slots[i].CurrentItemData == getItemData && !slots[i].IsFilled)
    //            {
    //                slots[i].AddItem();
    //                return true;
    //            }

    //            //スロットが空なら、アイテムデータをセットする
    //            if (slots[i].IsEmpty)
    //            {
    //                slots[i].SetInfo(getItemData);
    //                return true;
    //            }
    //        }
    //        return false;//全てのスロットが埋まっている
    //    }

    }
}
