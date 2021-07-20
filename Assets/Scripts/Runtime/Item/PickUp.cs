using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

namespace Musashi.Item
{
    /// <summary>
    /// フィールド上に落ちているアイテムの動きを制御するクラス。
    /// </summary>
    public class PickUp : MonoBehaviour, IInteractable
    {
        public event Action<Transform> OnPickEvent;

        [SerializeField] float rotateDuration = 1f;
        [SerializeField] bool doRotate = true;

        public bool HavePicked { get; set; }

        private void Start()
        {
            if (doRotate)
            {
                //rotate based world axix Y 
                transform.DORotate(Vector3.up * 360, rotateDuration, RotateMode.WorldAxisAdd)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Incremental);
            }
        }

        public void Excute(Transform player)
        {
            if (OnPickEvent != null)
            {
                OnPickEvent.Invoke(player);
            }

            if (HavePicked)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
            }
        }


        /// <summary>
        /// コンポーネントをアタッチした時に呼ばれる。
        /// </summary>
        private void Reset()
        {
            this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }
    }
}
