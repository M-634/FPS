using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] SceneInBuildIndex buildIndex;
    public void OnPointerClick(PointerEventData eventData)
    {
        GameManager.Instance.SceneLoder.LoadScene(buildIndex);
    }
}
