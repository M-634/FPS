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
    }
}
