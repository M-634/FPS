using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 武器はプレイヤーに持たせた状態ででアクティブを切る
    /// </summary>
    public abstract class BaseWeapon : MonoBehaviour
    {
        public KindOfItem KindOfItem;
        public abstract void Attack();

        protected bool canUse = true;

       
        private void OnEnable()
        {
            EventManeger.Instance.Subscribe(EventType.OpenInventory,()=> canUse = false);
            EventManeger.Instance.Subscribe(EventType.CloseInventory,()=> canUse = true);
        }

        private void OnDisable()
        {
            EventManeger.Instance.UnSubscribe(EventType.OpenInventory, () => canUse = false);
            EventManeger.Instance.UnSubscribe(EventType.CloseInventory, () => canUse = true);
        }
    }
}
