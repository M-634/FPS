using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Musashi.Level.AdventureMode
{
    public class KeyDoor : Event.CommandHandler
    {
        [SerializeField] Camera switchCamera;
        [SerializeField, Range(1, 10)] int lockKeyCount = 1;
        [SerializeField] float offsetY;
        [SerializeField] float duration;
        [SerializeField] AnimationCurve curve;
        [SerializeField] AudioSource openDoorSe;
        [SerializeField] UnityEventWrapper OnOpened;

        int unlockKeyCount;
        bool unlock = false;
        Tweener currentTweener;

        private void Start()
        {
            reciver.Register(UnLock);
        }

        private void OnDestroy()
        {
            reciver.Remove(UnLock);
        }

        private void UnLock()
        {
            if (unlock) return;

            unlockKeyCount++;

            if (unlockKeyCount == lockKeyCount)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            unlock = true;
            GameManager.Instance.CanProcessPlayerMoveInput = false;
            switchCamera.gameObject.SetActive(true);

            if (openDoorSe)
            {
                openDoorSe.Play();
            }

            currentTweener = transform.DOMoveY(transform.position.y + offsetY, duration).SetEase(curve)
                 .OnComplete(() =>
                 {
                     switchCamera.gameObject.SetActive(false);
                     GameManager.Instance.CanProcessPlayerMoveInput = true;
                     if (openDoorSe)
                     {
                         openDoorSe.Stop();
                     }
                     if (OnOpened != null)
                     {
                         OnOpened.Invoke();
                     }
                 });
        }

        private void OnDisable()
        {
            if (currentTweener != null)
            {
                currentTweener.Kill();
            }
        }

    }
}
