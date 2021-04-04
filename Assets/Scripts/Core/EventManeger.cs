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
        public Action<float,float> healAction;
        public Action<int> pickUpAmmoAction;
   
        private static EventManeger instance;
        public static EventManeger Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EventManeger
                    {
                        eventTable = new Dictionary<EventType, Action>(),
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

        public void Subscribe(Action<float, float> action)
        {
            healAction += action;
        }

        public void Subscribe(Action<int> action)
        {
            pickUpAmmoAction += action;
        }

   
        public void UnSubscribe(EventType eventType, Action action)
        {
            Debug.Log($"{eventType}イベントの{action.Method.Name}が解除されたよ");
            eventTable[eventType] -= action;
        }

        public void UnSubscribe(Action<float, float> action)
        {
            healAction -= action;
        }

        public void UnSubscribe(Action<int> action)
        {
            pickUpAmmoAction -= action;
        }


        public void Excute(EventType eventType)
        {
            eventTable[eventType]?.Invoke();
            Debug.Log($"{eventType}イベントが実行された");
        }

        public void Excute(float healPoint,float healTime)
        {
            healAction?.Invoke(healPoint,healTime);
        }

        public void Excute(int ammoNum)
        {
            pickUpAmmoAction?.Invoke(ammoNum);
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
        ChangePostProcess,
        OpenInventory,
        CloseInventory,
    }
}




