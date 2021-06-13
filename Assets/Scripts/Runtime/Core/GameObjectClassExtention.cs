using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

namespace Musashi
{
    public static class GameObjectClassExtention
    {
        /// <summary>
        /// GameObjectClassのSetActiveを非同期処理にした拡張クラス。
        /// lifeTime秒後にアクティブが切り替わる
        /// </summary>
        public static async void SetActive(this GameObject gameObject, bool value, int lifeTime = 0)
        {
            if (!gameObject.activeSelf) return;

            await UniTask.Delay(TimeSpan.FromSeconds(lifeTime),ignoreTimeScale: false);//lifeTime秒待つ

            if (Application.isPlaying == false) return;

            if (gameObject)
            {
                gameObject.SetActive(value);
            }
        }
    }
}