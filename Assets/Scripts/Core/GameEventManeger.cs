using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    /// <summary>
    /// ゲーム全体に関わるイベントを通知するクラス
    /// </summary>
    public class GameEventManeger : IDisposable
    {
        public Dictionary<GameEventType, Action> eventTable;

        private static GameEventManeger instance;
        public static GameEventManeger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameEventManeger
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
                eventTable[eventType] += action;
            else
                eventTable.Add(eventType, action);
         
            Debug.Log($"{eventType}イベントに{action.Method.Name}が追加された");
        }

        public void UnSubscribe(GameEventType eventType, Action action)
        {
            Debug.Log($"{eventType}イベントの{action.Method.Name}が解除されたよ");
            eventTable[eventType] -= action;
        }

        public void Excute(GameEventType eventType)
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
    public enum GameEventType
    {
        EnemyDie,
    }
}




