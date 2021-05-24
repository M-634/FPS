using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    public class Item : MonoBehaviour, IInteractable
    {
        public Action<Transform> OnPickUpEvent;
        public Func<bool> OnUseEvent;
        
        [SerializeField] bool canUseItem;
        [SerializeField] protected ItemSettingSOData itemSetting;
        [SerializeField, Range(1, 999)] int maxStackSize = 1;
        [SerializeField, Range(1, 100)] int stackSize = 1;

        public string ItemName { get; private set; }
        public ItemType ItemType { get; private set; }
        public Sprite Icon { get; private set; }
        public bool Stackable { get; private set; }
        public int MaxStacSize { get => maxStackSize; }
        public int StacSize { get => stackSize; set => stackSize = value; }

        protected virtual void Start()
        {
            SetItemDataFromScriptableObject();

            OnPickUpEvent += (Transform player) =>
            {
                var canGet = player.GetComponentInChildren<ItemInventory>().CanGetItem(this);

                if (canGet)
                {
                    GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                    if(canUseItem)
                    {
                        gameObject.SetActive(false);
                    }
                    else
                    {
                        Destroy(gameObject);
                    }
                }
            };
        }

        private void SetItemDataFromScriptableObject()
        {
            if (itemSetting == null)
            {
                Debug.LogError("アイテム情報のスクリプタブルオブジェクトがアサインされていません");
                return;
            }
            ItemName = itemSetting.itemName;
            ItemType = itemSetting.itemType;
            Icon = itemSetting.icon;
            Stackable = itemSetting.stackable;
        }

        public void Excute(Transform player)
        {
            OnPickUpEvent?.Invoke(player);
        }


        /// <summary>
        /// コンポーネントをアタッチした時に呼ばれる。
        /// </summary>
        private void Reset()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
    }
}
