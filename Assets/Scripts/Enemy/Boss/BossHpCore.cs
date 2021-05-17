using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public class BossHpCore : BaseHealthControl
    {
        [SerializeField] Canvas bossInfo;
        [SerializeField] UnityEvent bossDeadEvent;
        protected override void OnDie()
        {
            IsDead = true;
            bossInfo.gameObject.SetActive(false);
            
            if(bossDeadEvent != null)
            {
                bossDeadEvent.Invoke();
            }
        }
    }
}

