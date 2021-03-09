using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Musashi
{
    public class MenuButton : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] SceneInBuildIndex buildIndex;
        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.SceneLoder.LoadScene(buildIndex);
        }
    }
}
