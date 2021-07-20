using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi.Item
{
    /// <summary>
    /// フィールド上に落ちているアイテムの動きを制御するクラス
    /// </summary>
    public class PickUp : MonoBehaviour, IInteractable
    {    
        public event Action<Transform> OnPickEvent;

        public void Excute(Transform player)
        {
            OnPickEvent.Invoke(player);
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
