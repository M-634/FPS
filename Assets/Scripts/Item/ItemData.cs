using System.Collections;
using UnityEngine;
using System;

namespace Musashi
{

    [Serializable]
    [CreateAssetMenu(fileName = "ItemData", menuName = "CreateItemData")]
    public class ItemData : ScriptableObject
    {
        [SerializeField] KindOfItem kindOfItem;
        [SerializeField] Sprite icon;
        [SerializeField] BaseItem itemPrefab;
        [SerializeField] bool isWeapon;
        [SerializeField] int maxStackNumber;

        public KindOfItem KindOfItem { get => kindOfItem; }
        public Sprite Icon { get => icon; }
        public BaseItem ItemPrefab { get => itemPrefab; }
        public bool IsWeapon { get => isWeapon; }
        public int MaxStackNumber { get => maxStackNumber; }
    }
}
