using UnityEngine;
using System.Threading.Tasks;//async/awaitを使うため 

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
            await Task.Delay(1000 * lifeTime);// lifeTime 秒待つ
            if(gameObject)
                gameObject.SetActive(value);
        }
    }
}