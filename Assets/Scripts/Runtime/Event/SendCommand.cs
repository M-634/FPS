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
        [SerializeField] AudioSource onSendAudio;
        [SerializeField] float delaySendTime;

        public bool IsTriggered { get; set; }

        public async void Send()
        {
            if (reciver && !IsTriggered)
            {
                //効果音を設定している時は、コマンドを送る時間を遅延させることができる
                if (onSendAudio)
                {
                    onSendAudio.Play();
                    await UniTask.Delay(TimeSpan.FromSeconds(delaySendTime), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
                    reciver.Receive();
                    IsTriggered = true;
                }
                else
                {
                    reciver.Receive();
                    IsTriggered = true;
                }
            }
        }
    }
}
