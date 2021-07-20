using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Musashi
{
    public static class GameObjectClassExtention
    {
        /// <summary>
        /// アクティブの切り替えを遅延させる拡張メソッド
        /// </summary>
        /// <param name="value">アクティブ</param>
        /// <param name="duration">秒待つ</param>
        /// <returns></returns>
        public static async void DelaySetActive(this GameObject gameObject, bool value,float duration)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(duration), false, PlayerLoopTiming.Update, gameObject.GetCancellationTokenOnDestroy());
            gameObject.SetActive(value);
        }
    }
}