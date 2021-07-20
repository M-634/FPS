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
        [SerializeField] Camera playerCamera;
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
                    obj.Excute(transform);
                }
            }
        }

        private bool CheakInteractiveObj()
        {
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, distance, interactiveLayer))
            {
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            InteractiveMessage.CloseMessage();
            return false;
        }
    }
}
