﻿using UnityEngine;

namespace Musashi
{
    public class StateMachine<T> where T : MonoBehaviour
    {
        public IState<T> CurrentState { get; set; }
        private readonly T owner;

        public StateMachine(T owner, IState<T> state)
        {
            this.owner = owner;
            CurrentState = state;
            CurrentState.OnEnter(owner);
        }

        public void ExcuteOnUpdate()
        {
            CurrentState.OnUpdate(owner);
        }

        public void ChangeState(IState<T> nextstate)
        {
            CurrentState.OnExit(owner, nextstate);
            nextstate.OnEnter(owner, CurrentState);
            CurrentState = nextstate;
            
            if(owner is NPC.NPCMoveControl)
            {
                Debug.Log(CurrentState);
            }
        }
    }
}