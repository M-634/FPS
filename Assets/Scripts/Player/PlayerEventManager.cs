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
        public Dictionary<PlayerEventType, Action> playerEventTable;

        private void Awake()
        {
            playerEventTable = new Dictionary<PlayerEventType, Action>();
        }

        public void Subscribe(PlayerEventType eventType, Action action)
        {
            //すでにイベントキーが存在するなら、actionのみ追加する
            if (playerEventTable.ContainsKey(eventType))
                playerEventTable[eventType] += action;
            else
                playerEventTable.Add(eventType, action);

            Debug.Log($"{eventType}イベントに{action.Method.Name}が追加された");
        }

        public void UnSubscribe(PlayerEventType eventType, Action action)
        {
            Debug.Log($"{eventType}イベントの{action.Method.Name}が解除されたよ");
            playerEventTable[eventType] -= action;
        }

        public void Excute(PlayerEventType eventType)
        {
            playerEventTable[eventType]?.Invoke();
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
