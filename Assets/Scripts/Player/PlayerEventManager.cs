using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    /// <summary>
    /// このクラスをアタッチしているプレイヤーに関わるイベントを管理するクラス
    /// </summary>
    public class PlayerEventManager : MonoBehaviour
    {
        private Dictionary<PlayerEventType, Action> playerEventTable;
        public Dictionary<PlayerEventType, Action> PlayerEventTable 
        {
            get
            {
                // null check property
                if (playerEventTable == null)
                    playerEventTable = new Dictionary<PlayerEventType, Action>();
                return playerEventTable;
            }
        }

        public void Subscribe(PlayerEventType eventType, Action action)
        {
            //すでにイベントキーが存在するなら、actionのみ追加する
            if (PlayerEventTable.ContainsKey(eventType))
                PlayerEventTable[eventType] += action;
            else
                PlayerEventTable.Add(eventType, action);

            Debug.Log($"{eventType}イベントに{action.Method.Name}が追加された");
        }

        public void UnSubscribe(PlayerEventType eventType, Action action)
        {
            Debug.Log($"{eventType}イベントの{action.Method.Name}が解除されたよ");
            PlayerEventTable[eventType] -= action;
        }

        public void Excute(PlayerEventType eventType)
        {
            PlayerEventTable[eventType]?.Invoke();
            Debug.Log($"{eventType}イベントが実行された");
        }

    }

    /// <summary>
    /// イベントキー
    /// </summary>
    public enum PlayerEventType
    {
        ChangePostProcess,
        OpenInventory,
        CloseInventory,
    }
}
