using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Events;


namespace Musashi.Item
{
    [Serializable]
    [CreateAssetMenu(fileName = "ItemData", menuName = "CreateItemData")]
    public class ItemSettingSOData : ScriptableObject
    {
        public string itemName;
        public string description;
        public ItemType itemType;
        public Sprite icon;
        public bool stackable;
    }
}
