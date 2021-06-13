using UnityEngine;

namespace Musashi
{
    public interface IState<T> where T : MonoBehaviour
    {
        void OnEnter(T owner, IState<T> prevState = null);
        void OnUpdate(T owner);
        void OnExit(T owner, IState<T> nextState = null);
    }
}