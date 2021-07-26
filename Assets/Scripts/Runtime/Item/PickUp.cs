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
    [RequireComponent(typeof(Collider))]
    public class PickUp : MonoBehaviour, IInteractable
    {
        public event Action<Transform> OnPickEvent;

        [SerializeField] float rotateDuration = 1f;
        [SerializeField] bool doRotate = true;

        private Tween currentTween = null;
        public bool HavePicked { get; set; } = false;

        private void Start()
        {
            if (doRotate)
            {
                //rotate based world axix Y 
                currentTween = transform.DORotate(Vector3.up * 360, rotateDuration, RotateMode.WorldAxisAdd)
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
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnDestroy()
        {
            if (DOTween.instance != null)
            {
                currentTween.Kill();
            }
        }
    }
}
