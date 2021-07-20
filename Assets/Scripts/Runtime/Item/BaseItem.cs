using UnityEngine;

namespace Musashi.Item
{
    /// <summary>
    /// 全アイテム共通の設定をまとめたベースクラス。
    /// スクリプタブルオブジェクトで設定した値を引き続き、カプセル化する
    /// </summary>
    public abstract class BaseItem : MonoBehaviour 
    {
        [SerializeField] ItemSettingSOData itemSetting;
        public string GetItemName => itemSetting.itemName;
        public string GetItemDescription => itemSetting.description;
        public Sprite GetItemIcon => itemSetting.icon;
        public ItemType GetItemType => itemSetting.itemType;
        public bool CanStackable => itemSetting.stackable;
        public int GetMaxStacSize => itemSetting.maxStackSize;
        public int GetAddStacSize => itemSetting.stackSize;
        
        public Transform Ower { get; protected set; }

        /// <summary>
        /// アイテム使用時に呼ばれる関数。
        /// デフォルトは、使用できなかった時の処理を返す
        /// </summary>
        /// <returns></returns>
        public virtual bool UseItem() { return false; }
    }
}
