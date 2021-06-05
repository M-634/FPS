using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Item
{
    [CreateAssetMenu(fileName = "ItemDataBase", menuName = "CreateItemDataBase")]
    public class ItemDataBase : ScriptableObject
    {
        [SerializeField] List<ItemSettingSOData> itemDataList = new List<ItemSettingSOData>();

        public List<ItemSettingSOData> ItemDataList{ get => itemDataList; }
    }
}
