using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテム、ステージギミックとのインタラクティブを管理するクラス
    /// </summary>
    public class PlayerInteractive : MonoBehaviour
    {
        [SerializeField] LayerMask interactiveLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;
        PlayerInputProvider inputProvider;
        RaycastHit hit;

        private void Start()
        {
            inputProvider = GetComponentInParent<PlayerInputProvider>();
        }

        private void Update()//コルーチンに変える
        {
            if (CheakInteractiveObj() && inputProvider.Interactive)
            {
                if (hit.collider.TryGetComponent(out IInteractable obj))
                {
                    obj.Excute(transform.parent);
                }
            }
        }

        private bool CheakInteractiveObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, interactiveLayer))
            {
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            InteractiveMessage.CloseMessage();
            return false;
        }
    }
}
