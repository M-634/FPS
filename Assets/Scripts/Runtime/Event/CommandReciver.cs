using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

namespace Musashi.Event
{
    public enum CommandType
    {
        None,
        OpenDoor,
        SpwanPlayer,
        UpdateSavePoint,
    }

    public class CommandReciver : MonoBehaviour
    {
        readonly Dictionary<CommandType, Action> commandTable = new Dictionary<CommandType, Action>();

        public void Register(CommandType type, Action action)
        {

            if (commandTable.ContainsKey(type))
            {
                commandTable[type] += action;
            }
            else
            {
                commandTable.Add(type, action);
            }
         
        }

        public void Remove(CommandType type)
        {
            commandTable.Remove(type);  
        }

        public void Receive(CommandType type)
        {
            if (commandTable.ContainsKey(type))
            {
                commandTable[type].Invoke();
            }
        }
    }
}
