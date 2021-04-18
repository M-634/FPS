using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// プレイヤーが武器やアイテム、ステージギミックとのインタラクティブを管理するクラス
    /// </summary>
    [RequireComponent(typeof(PlayerInputManager))]
    public class PlayerInteractive : MonoBehaviour
    {
        [SerializeField] LayerMask interactiveLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;
        PlayerInputManager playerInputManager;
        RaycastHit hit;

        private void Start()
        {
            playerInputManager = GetComponent<PlayerInputManager>();
        }

        private void Update()
        {
            if (CheakInteractiveObj() && playerInputManager.Interactive)
            {
                if (hit.collider.TryGetComponent(out IInteractable obj))
                {
                    obj.Excute(this.gameObject);
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
