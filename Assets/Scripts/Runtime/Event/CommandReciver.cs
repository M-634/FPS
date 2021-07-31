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

        public void Register(UnityAction action)
        {
            OnCommandRecive.AddListener(action);
        }

        public void Remove(UnityAction action)
        {
            OnCommandRecive.RemoveListener(action);
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
