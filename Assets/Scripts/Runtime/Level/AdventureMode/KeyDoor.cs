using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Musashi.Level.AdventureMode
{
    public class KeyDoor : MonoBehaviour
    {
        [SerializeField,Range(1,10)] int lockKeyCount = 1;
        [SerializeField] float offsetY;
        [SerializeField] float duration;
  
        int unlockKeyCount;
        bool unlock = false;

        public void UnLock()
        {
            if (unlock) return;

            unlockKeyCount++;

            if(unlockKeyCount == lockKeyCount)
            {
                OpenDoor();
            }
        }

        private void OpenDoor()
        {
            unlock = true;
            transform.DOMoveY(transform.position.y + offsetY, duration).SetEase(Ease.Linear);
        }
    }
}
