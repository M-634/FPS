using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// ゲーム全体に関わるイベントを通知するクラス
    /// </summary>
    public class GameEventManager : IDisposable
    {
        public Dictionary<GameEventType, Action> eventTable;

        private static GameEventManager instance;
        public static GameEventManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameEventManager
                    {
                        eventTable = new Dictionary<GameEventType, Action>(),
                    };
                }
                return instance;
            }
        }

        public void Subscribe(GameEventType eventType, Action action)
        {
            //すでにイベントキーが存在するなら、actionのみ追加する
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType] += action;
            }
            else
            {
                eventTable.Add(eventType, action);
            }
            Debug.Log($"{eventType}:に{action.Method.Name}が追加されました");
        }

        public void UnSubscribe(GameEventType eventType, Action action)
        {
            eventTable[eventType] -= action;
        }

        public void Excute(GameEventType eventType)
        {
            if (eventTable.ContainsKey(eventType))
            {
                eventTable[eventType]?.Invoke();
                Debug.Log($"{eventType}イベントが発火しました");
            }
        }

        public void Dispose()
        {
            eventTable.Clear();
        }
    }

    /// <summary>
    /// イベントのキー
    /// </summary>
    public enum GameEventType
    {
        EnemyDie,//敵の撃破数をカウントする
        EnemySpwan,//敵、出現時に敵の総数をカウントする
        StartGame,//タイマー表示、タイマースタート、敵を生成
        Goal//タイマーストップ、残った敵を削除、タイマーがベストタイムなら記録,プレイヤーの体力を満タンにする
    }

    [System.Serializable]
    public class UnityEventWrapper : UnityEvent { }
}




