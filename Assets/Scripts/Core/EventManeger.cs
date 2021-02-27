using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    /// <summary>
    /// イベントを通知するクラス
    /// </summary>
    public class EventManeger : IDisposable
    {
        public Dictionary<EventType, Action> eventTable;

        private static EventManeger instance;
        public static EventManeger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManeger
                    {
                        eventTable = new Dictionary<EventType, Action>()
                    };
                }
                return instance;
            }
        }

        public void Subscribe(EventType eventType, Action action)
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

            Debug.Log($"{eventType}イベントに{action.Method.Name}が追加された");
        }

        public void UnSubscribe(EventType eventType, Action action)
        {
            Debug.Log($"{eventType}イベントの{action.Method.Name}が解除されたよ");
            eventTable[eventType] -= action;
        }

        public void Excute(EventType eventType)
        {
            eventTable[eventType]?.Invoke();
            Debug.Log($"{eventType}イベントが実行された");
        }

        public void Dispose()
        {
            eventTable.Clear();
            Debug.Log("イベントテーブルを全て削除した");
        }
    }

    /// <summary>
    /// イベントのキー
    /// </summary>
    public enum EventType
    {
        Null,
        EnemyDie,
    }
}




