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

        /// <summary>
        /// アニメーションイベントから直接呼ばれる関数
        /// </summary>
        public void ExcuteAnimationEvent()
        {
            if(OnAnimationEvent != null)
            {
                OnAnimationEvent.Invoke();
            }
        }
    }
}