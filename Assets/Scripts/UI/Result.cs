using UnityEngine;
using TMPro;

namespace Musashi
{
    public class Result : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI resultText;

        private void Start()
        {
            if (GameManager.Instance.IsGameClear)
                resultText.text = "YOU WIN !";
            else
                resultText.text = "YOU LOSE";
        }
    }
}
