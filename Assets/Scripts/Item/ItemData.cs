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
        [SerializeField] int maxStackNumber;

        public KindOfItem KindOfItem => kindOfItem;
        public Sprite Icon => icon;
        public BaseItem ItemPrefab => itemPrefab;
        public int MaxStackNumber => maxStackNumber;
    }
}
