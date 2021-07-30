using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;


namespace Musashi.Event
{
    /// <summary>
    /// �C�x���g�𑗂�I�u�W�F�N�g�ɃA�^�b�`���钊�ۃN���X
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
                //���ʉ���ݒ肵�Ă��鎞�́A�R�}���h�𑗂鎞�Ԃ�x�������邱�Ƃ��ł���
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
