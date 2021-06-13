using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Musashi
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField] SceneInBuildIndex buildIndex;
        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }
        public void OnPointerClick(PointerEventData eventData)
        {
            if (GameManager.Instance)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.OnClickButton);
                GameManager.Instance.SceneLoder.LoadScene(buildIndex);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (button.interactable)
            {
                if (GameManager.Instance)
                {
                    GameManager.Instance.SoundManager.PlaySE(SoundName.PointerEnterButton);
                }
            }
        }
    }
}
