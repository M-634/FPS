using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Musashi
{
    public class Result : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI resultText;
        [SerializeField] Button initButton;


        private void Start()
        {
            if (GameManager.Instance.IsGameClear)
                resultText.text = "YOU WIN !";
            else
                resultText.text = "YOU LOSE";

            if(initButton)
            {
                initButton.Select();
            }
        }
    }
}
