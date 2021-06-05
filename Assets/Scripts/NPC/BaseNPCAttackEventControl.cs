using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace Musashi.NPC
{
    /// <summary>
    /// 敵の攻撃アニメーションイベントをラッパーしたクラス
    /// </summary>
    [Serializable]
    public class NPCAttackAnimationEvent : UnityEvent { }

    /// <summary>
    /// 敵の攻撃イベントを制御するベースクラス
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class BaseNPCAttackEventControl : MonoBehaviour
    {
        [SerializeField] NPCAttackAnimationEvent OnAnimationEvent;

        protected void AddEvent(UnityAction call)
        {
            OnAnimationEvent.AddListener(call);
        }

        /// <summary>
        /// アニメーションイベントから直接呼ばれる関数
        /// </summary>
        public void Excute()
        {
            if(OnAnimationEvent != null)
            {
                OnAnimationEvent.Invoke();
            }
        }
    }
}