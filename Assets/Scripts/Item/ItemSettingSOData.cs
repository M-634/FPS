using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;


namespace Musashi
{

    [Serializable]
    [CreateAssetMenu(fileName = "ItemData", menuName = "CreateItemData")]
    public class ItemSettingSOData : ScriptableObject
    {
        public string itemName;
        public string description;
        public ItemType itemType;
        public Sprite icon;
        public BaseItem itemPrefab;
        public bool stackable;
        [Range(1,100)]
        public int maxStackSize = 1;
        [Range(1, 100)]
        public int stackSize = 1;

        public ItemType KindOfItem => itemType;
        public Sprite Icon => icon;
        public BaseItem ItemPrefab => itemPrefab;
        public int MaxStackNumber => maxStackSize;
    }
}
