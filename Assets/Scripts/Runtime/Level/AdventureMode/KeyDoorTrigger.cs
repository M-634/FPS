using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi.Level.AdventureMode
{
    public sealed class KeyDoorTrigger :Event.OnTriggerEvent
    {
       [SerializeField] UnityEventWrapper OnUnlockEvent;
       [SerializeField] AudioSource onTriggerSe;

        public bool HasDone { get; set; } = false;
       
        /// <summary>
        /// KeyDoorクラスから呼ばれる。
        /// key解除か、ドアが開いたタイミングで呼ぶこと。
        /// </summary>
        public void InvokeUnLockEvent(bool unlockeDoor = false)
        {
            if(!HasDone && IsTriggered && OnUnlockEvent != null)
            {
                if (!unlockeDoor)
                {
                    onTriggerSe.Play();
                }
                OnUnlockEvent.Invoke();
                HasDone = true;
            }
        }
    }
}
