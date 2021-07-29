using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// ボタンにまつわるイベントクラス
    /// </summary>
    [System.Serializable]
    public class ButtonEvents : UnityEvent { }
    /// <summary>
    /// ボタンのベースクラス
    /// </summary>
    [RequireComponent(typeof(Button))]
    public abstract class BaseButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, ISelectHandler
    {
        public ButtonEvents OnEnterButtonEvents = default;
        public ButtonEvents OnClickButtonEvents = default;
        public ButtonEvents OnExitButtonEvents = default;
        public ButtonEvents OnSelectedButtonEvents = default;

        protected Button button;

        protected virtual void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!button.interactable) return;

            if (OnClickButtonEvents != null)
            {
                OnClickButtonEvents.Invoke();
            }

            GameManager.Instance.SoundManager.PlaySE(SoundName.OnClickButton);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;

            if (OnEnterButtonEvents != null)
            {
                OnEnterButtonEvents.Invoke();
            }
            GameManager.Instance.SoundManager.PlaySE(SoundName.PointerEnterButton);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!button.interactable) return;
            if (OnExitButtonEvents != null)
            {
                OnExitButtonEvents.Invoke();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (!button.interactable) return;
            if (OnSelectedButtonEvents != null)
            {
                OnSelectedButtonEvents.Invoke();
            }
            GameManager.Instance.SoundManager.PlaySE(SoundName.PointerEnterButton);
        }
    }
}
