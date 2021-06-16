using UnityEngine;
using UnityEngine.EventSystems;

namespace Musashi
{
    public sealed class OptionsButton : BaseButton 
    {
        [SerializeField] bool isInitSelected;

        private void OnEnable()
        {
            if (isInitSelected)
            {
                button.Select();
            }
        }
    }
}
