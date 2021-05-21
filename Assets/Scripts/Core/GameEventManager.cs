using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
                eventTable[eventType] += action;
            else
                eventTable.Add(eventType, action);
        }

        public void UnSubscribe(GameEventType eventType, Action action)
        {
            eventTable[eventType] -= action;
        }

        public void Excute(GameEventType eventType)
        {
            if(eventTable.ContainsKey(eventType))
                eventTable[eventType]?.Invoke();
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
        EnemyDie,
        SpawnDie,
        StartGame,
        EndGame
    }
}




