using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    [CreateAssetMenu(fileName = "ItemDataBase", menuName = "CreateItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        [SerializeField] List<ItemData> itemDataList = new List<ItemData>();

        public List<ItemData> ItemDataList{ get => itemDataList; }
    }
}
