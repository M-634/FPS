using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace Musashi.Event
{
    /// <summary>
    /// イベントを送るオブジェクトにアタッチする抽象クラス
    /// </summary>
    public abstract class SendCommand : MonoBehaviour
    {
        [SerializeField] CommandReciver reciver;
        [SerializeField] float delaySendTime;

        public  bool IsTriggered { get; set; }

        public async void Send()
        {
            if (reciver && !IsTriggered)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(delaySendTime), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                IsTriggered = true;
                reciver.Receive();
            }
        }
    }
}
