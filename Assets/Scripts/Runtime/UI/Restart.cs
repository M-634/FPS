using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace Musashi
{
    /// <summary>
    /// ゲームシーンのリスタート機能。シーンの再読み込み
    /// </summary>
    public class Restart : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
    {
        Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.interactable = false;
        }

        private void OnEnable()
        {
            if (GameManager.Instance.SceneLoder.GetActiveSceneBuildIndex == (int)SceneInBuildIndex.MainGame)
            {
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!button.interactable) return;
            GameManager.Instance.SoundManager.PlaySE(SoundName.OnClickButton);
            GameManager.Instance.SceneLoder.LoadScene(SceneInBuildIndex.MainGame, GameManager.Instance.Configure.Close);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!button.interactable) return;
            GameManager.Instance.SoundManager.PlaySE(SoundName.PointerEnterButton);
        }
    }
}
