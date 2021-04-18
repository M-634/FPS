using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

namespace Musashi
{
    /// <summary>
    /// インベントリースロットの抽象クラス
    /// ＜キーマウスの場合＞
    /// カーソルを合わせるとハイライトとアイテムの説明が出てくる。
    /// スロットを左クリックしたらインベントリーを閉じてアイテムを使用する。
    /// 右クリックしたらアイテムを捨てる。
    /// </summary>
    public abstract class SlotBase : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField] protected Image icon;
        [SerializeField] protected Image Outline;
        [SerializeField] protected TextMeshProUGUI keyCode;
        [SerializeField] protected Color highLightColor;
        [SerializeField] protected TextMeshProUGUI stack;

        bool isSelected = false;

        protected PlayerInputManager playerInput;
        public string KeyCode { set => keyCode.text = value; }
        public virtual bool IsEmpty { get; set; } = true;
        public virtual bool IsFilled { get; set; } = false;

        /// <summary>
        /// 初期化時に PlayerItemInventoryクラスから呼ばれる。
        /// 必要なコンポーネントを設定する。
        /// </summary>
        /// <param name="_playerInput"></param>
        /// <param name="_itemInventory"></param>
        public void SetInput(PlayerInputManager _playerInput)
        {
            playerInput = _playerInput;
        }

        public virtual void SetInfo<T>(T getData) where T:ScriptableObject{}
     
        /// <summary>
        /// 継承先でアイテムスロットなら使用、武器スロットなら装備する処理を実装する。
        /// その後、使用できたか、装備したらインベントリーを閉じる
        /// </summary>
        public virtual void UseObject(GameObject player) {}

        /// <summary>
        /// スロットないのアイテムを捨てる関数
        /// </summary>
        /// <param name="worldPoint"></param>
        public virtual void DropObject(Vector3 worldPoint) { }

        /// <summary>
        /// スロット内のデータを空にする
        /// </summary>
        public virtual void ResetInfo() { }

        /// <summary>
        /// スロットの上にポインターが来たら、選択されたと判定する関数。
        /// スロットにハイライトをつけ、入力を受け付ける非同期処理を走らせる
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Outline.color = highLightColor;
            isSelected = true;
            StartCoroutine(ReciveInteractiveAction());
        }

        /// <summary>
        ///スロット上からポインターがなくなったら、選択された判定すを解除し
        ///ハイライトを消す
        /// </summary>
        /// <param name="eventData"></param>
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Outline.color = Color.black;
            isSelected = false;
        }
        
        /// <summary>
        /// スロットを選択を選択されている間、入力を受け付ける関数。
        /// </summary>
        /// <returns></returns>
        IEnumerator ReciveInteractiveAction()
        {
            while (isSelected)
            {
                if (playerInput.UseItem)
                    UseObject(playerInput.transform.gameObject);

                if (playerInput.DropItem)
                {
                    var dir = playerInput.transform.GetComponentInChildren<Camera>().transform.forward;
                    DropObject(playerInput.transform.position + transform.up + dir * 2f);
                }
                yield return null;
            }
        }
    }
}
