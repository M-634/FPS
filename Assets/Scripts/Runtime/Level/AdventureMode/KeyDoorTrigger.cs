using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi.Level.AdventureMode
{
    public class KeyDoorTrigger : MonoBehaviour,IInteractable
    {
        [SerializeField] UnityEventWrapper OnUnlockEvent;

        public void Excute(Transform player)
        {
            if(OnUnlockEvent != null)
            {
                OnUnlockEvent.Invoke();
            }
        }
    }
}
