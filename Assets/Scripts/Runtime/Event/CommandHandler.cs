using UnityEngine;

namespace Musashi.Event
{
    [RequireComponent(typeof(CommandReciver))]
    public abstract class CommandHandler : MonoBehaviour
    {
        protected CommandReciver reciver;

        protected virtual void Awake()
        {
            reciver = GetComponent<CommandReciver>();
        }
    }
}
