using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Musashi
{
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerItemInventory : MonoBehaviour
    {
        [SerializeField] ItemDataBase itemDataBase;
        [SerializeField] CanvasGroup inventoryCanvasGroup;
        [SerializeField] ItemSlot[] itemSlots;

        bool isOpenInventory = false;
        public bool IsSlotSelected { get => SelectedSlot != null; }
        public SlotBase SelectedSlot { get; private set; }

        PlayerInputManager playerInput;
        PlayerEventManager playerEvent;

        private void Start()
        {
            inventoryCanvasGroup.HideUIWithCanvasGroup();
            playerInput = GetComponent<PlayerInputManager>();
            playerEvent = GetComponent<PlayerEventManager>();
            if (playerInput)
            {
                foreach (var slot in itemSlots)
                {
                    slot.SetInput(playerInput);
                    slot.SetItemSlot(this);
                }
            }
        }

        private void Update()
        {
            if (playerInput.Inventory)
            {
                OpenAndCloseInventory();
            }
        }

        /// <summary>
        /// 取得したアイテムがデータベースに存在するか確認する。
        /// データベースにあったら、 アイテムを拾うことができるか判定する
        /// </summary>
        /// <param name="getItem"></param>
        /// <returns></returns>
        public bool CanGetItem(Item getItem)
        {
            foreach (var itemData in itemDataBase.ItemDataList)
            {
                if (itemData.itemName == getItem.ItemName)
                {
                    return SearchItemSlot(getItem);
                }
            }
            Debug.LogWarning("データベースに該当するアイテムがありません");
            return false;
        }


        /// <summary>
        /// 既にスロット内にアイテムが存在するか調べる。
        /// ないなら左側から順番に埋めていく。
        /// </summary>
        /// <returns></returns>
        private bool SearchItemSlot(Item getItem)
        {
            for (int i = 0; i < itemSlots.Length; i++)
            {
                //スタック数が満タンの時は次のスロットを調べる
                if (itemSlots[i].IsFilled)
                {
                    continue;
                }

                //同一アイテムがスロット内にあり,かつスタック数が満タンではない時はスタック数を足していく
                if (!itemSlots[i].IsEmpty && itemSlots[i].currentItemInSlot.ItemName == getItem.ItemName && !itemSlots[i].IsFilled)
                {
                    itemSlots[i].AddItemInSlot(getItem);
                    GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                    getItem.gameObject.SetActive(false);
                    return true;
                }

                //スロットが空なら、アイテムデータをセットする
                if (itemSlots[i].IsEmpty)
                {
                    itemSlots[i].SetInfo(getItem);
                    GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                    getItem.gameObject.SetActive(false);
                    return true;
                }
            }
            return false;//全てのスロットが埋まっている
        }

        public void OpenAndCloseInventory()
        {
            if (GameManager.Instance.HaveShowConfigure) return;

            if (isOpenInventory)
            {
                inventoryCanvasGroup.HideUIWithCanvasGroup();
                GameManager.Instance.LockCusor();
                playerEvent.Excute(PlayerEventType.CloseInventory);
            }
            else
            {
                inventoryCanvasGroup.ShowUIWithCanvasGroup();
                GameManager.Instance.UnlockCusor();
                playerEvent.Excute(PlayerEventType.OpenInventory);
            }
            isOpenInventory = !isOpenInventory;
        }
    }
}
