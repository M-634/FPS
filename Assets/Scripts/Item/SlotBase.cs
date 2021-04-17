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
    
        public string KeyCode { set => keyCode.text = value; }
        public virtual bool IsEmpty { get; set; } = true;
        public virtual bool IsFilled { get; set; } = false;

        public virtual void SetInfo<T>(T getData) where T:ScriptableObject{}
     
        /// <summary>
        /// 継承先でアイテムスロットなら使用、武器スロットなら装備する処理を実装する。
        /// その後、使用できたか、装備したらインベントリーを閉じる
        /// </summary>
        public virtual void UseObject() {}

        /// <summary>
        /// スロットないのアイテムを捨てる関数
        /// </summary>
        /// <param name="worldPoint"></param>
        public virtual void DropObject(Vector3 worldPoint) { }

        /// <summary>
        /// スロット内のデータを空にする
        /// </summary>
        public virtual void ResetInfo() { }

        bool isSelected = false;

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            Outline.color = highLightColor;
            isSelected = true;
            StartCoroutine(ReciveInteractiveAction());
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            Outline.color = Color.black;
            isSelected = false;
        }
        
        IEnumerator ReciveInteractiveAction()
        {
            while (isSelected)
            {
                //PlayerInputManagerの部分を変更する
                //if (PlayerInputManager.ClickLeftMouse())
                //    UseObject();

                //if (PlayerInputManager.ClickRightMouse())
                //{
                //    var mousePosition = Input.mousePosition;
                //    mousePosition.z += 2f;

                //    var worldPoint = Camera.main.ScreenToWorldPoint(mousePosition);
                //    DropObject(worldPoint);
                //}
                yield return null;
            }
        }
    }
}
