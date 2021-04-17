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
        [SerializeField] LayerMask pickUpLayer;
        [SerializeField] GameObject interactiveMessage;
        [SerializeField] float distance = 10f;

        RaycastHit hit;

        private void Update()
        {
            // PlayerInputManager.InteractiveAction()を変更する
            //if (CheakPickUpObj() && PlayerInputManager.InteractiveAction())
            //{
            //    if (hit.collider.TryGetComponent(out IPickUpObjectable pickUpObjectable))
            //    {
            //        pickUpObjectable.OnPicked();
            //    }
            //}
        }

        private bool CheakPickUpObj()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distance, pickUpLayer))
            {
                InteractiveMessage.ShowInteractiveMessage(InteractiveMessage.InteractiveText);
                return true;
            }
            InteractiveMessage.CloseMessage();
            return false;
        }
    }
}
