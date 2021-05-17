using UnityEngine;

namespace Musashi
{
    public class StateMacnie<T> where T : MonoBehaviour
    {
        public IState<T> CurrentState { get; set; }
        public T owner;

        public StateMacnie(T owner, IState<T> state)
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
        }
    }
}