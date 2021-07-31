using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace Musashi.Event
{

    public enum SendCommandType
    {
        OneShot,//一回だけ
        Loop,//繰り返し
    }

    /// <summary>
    /// イベントを送るオブジェクトにアタッチする抽象クラス
    /// </summary>
    public abstract class SendCommand : MonoBehaviour
    {
        [SerializeField] CommandType commandType;
        [SerializeField] SendCommandType sendType = SendCommandType.OneShot;
        [SerializeField] CommandReciver reciver;
        [SerializeField] float delaySendTime;

        public bool IsTriggered { get; set; }

        public async void Send()
        {
            if (sendType == SendCommandType.OneShot)
            {
                if (reciver && !IsTriggered)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(delaySendTime), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                    IsTriggered = true;
                    reciver.Receive(commandType);
                }
            }
            else
            {
                if (reciver)
                {
                    await UniTask.Delay(TimeSpan.FromSeconds(delaySendTime), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                    reciver.Receive(commandType);
                }
            }
        }
    }
}
