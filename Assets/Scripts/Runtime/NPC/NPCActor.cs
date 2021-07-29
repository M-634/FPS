using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.NPC
{
    /// <summary>
    /// 敵のループ処理を制御するクラス.NPCの親オブジェットにアタッチすること
    /// </summary>
    public class NPCActor : MonoBehaviour
    {
        public UnityEventWrapper OnInitializeNPCEvent = default;
        public UnityEventWrapper OnSpwanNPCEvent = default;

        private void Start()
        {
            OnInitializeNPCEvent.Invoke();
            OnSpwanNPCEvent.AddListener(()=>GameEventManager.Instance.Excute(GameEventType.EnemySpwan));

            GameEventManager.Instance.Subscribe(GameEventType.StartGame, OnSpwanNPCEvent.Invoke);
            GameEventManager.Instance.Subscribe(GameEventType.Goal, OnInitializeNPCEvent.Invoke);
        }

        /// <summary>
        /// シーンロード時のみイベントを解除する
        /// </summary>
        private void OnDestroy()
        {
            GameEventManager.Instance.UnSubscribe(GameEventType.StartGame, OnSpwanNPCEvent.Invoke);
            GameEventManager.Instance.UnSubscribe(GameEventType.Goal, OnInitializeNPCEvent.Invoke);
        }


    }
}