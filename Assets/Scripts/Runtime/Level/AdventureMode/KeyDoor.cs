using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Musashi.Level.AdventureMode
{
    [RequireComponent(typeof(AudioSource))]
    public class KeyDoor : Event.CommandHandler
    {
        [SerializeField] Camera switchCamera;
        [SerializeField] Transform doorObject;

        [SerializeField, Range(1, 10)] int lockKeyCount = 1;
        [SerializeField] KeyDoorTrigger[] keyDoorTriggers;

        [SerializeField] float offsetY;
        [SerializeField] float duration;

        [SerializeField] AnimationCurve curve;
        [SerializeField] AudioClip openSE;

        int unlockKeyCount;
        bool unlockedDoor = false;

        Tweener currentTweener;
        AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.spatialBlend = 0f;//—§‘Ì‰¹‹¿Œø‰Ê‚ð‰Á‚¦‚é‚ÆA‰¹‚ª•·‚±‚¦‚È‚­‚È‚é‚½‚ß

            if (switchCamera)
            {
                switchCamera.gameObject.SetActive(false);
            }
            if (!doorObject)
            {
                doorObject = this.transform;
            }
            reciver.Register(UnLock);
        }

        private void OnDestroy()
        {
            reciver.Remove(UnLock);
        }

        private void UnLock()
        {
            if (unlockedDoor) return;

            unlockKeyCount++;

            if (unlockKeyCount == lockKeyCount)
            {
                OpenDoor();
            }
            else
            {
                OnUnlockKey();
            }
        }

        private void OpenDoor()
        {
            unlockedDoor = true;
            GameManager.Instance.CanProcessPlayerMoveInput = false;
            if (switchCamera)
            {
                switchCamera.gameObject.SetActive(true);
            }

            audioSource.Play(openSE); 

            currentTweener = doorObject.DOMoveY(doorObject.position.y + offsetY, duration).SetEase(curve)
                 .OnComplete(() =>
                 {
                     if (switchCamera)
                     {
                         switchCamera.gameObject.SetActive(false);
                     }
                     GameManager.Instance.CanProcessPlayerMoveInput = true;
                     audioSource.Stop();
                     OnUnlockKey();
                 });
        }

        private void OnUnlockKey()
        {
            foreach (var trigger in keyDoorTriggers)
            {
                if (trigger.HasDone) continue;
                trigger.InvokeUnLockEvent(unlockedDoor);
            }
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
