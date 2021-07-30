using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Musashi.Event
{
    public class CommandReciver : MonoBehaviour
    {
        [SerializeField] UnityEventWrapper OnCommandRecive;

        public void Register(UnityAction callback)
        {
            OnCommandRecive.AddListener(callback);
        }

        public void Remove(UnityAction callback)
        {
            OnCommandRecive.RemoveListener(callback);
        }

        public void Receive()
        {
            if(OnCommandRecive != null)
            {
                OnCommandRecive.Invoke();
            }     
        }

        private void OnDestroy()
        {
            OnCommandRecive.RemoveAllListeners();
        }
    }
}
