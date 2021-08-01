using UnityEngine;


namespace Musashi.Event
{
    /// <summary>
    /// トリガーに接したらイベントを発火させ
    /// コマンドを送るクラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class OnTriggerEvent : SendCommand
    {
        [SerializeField] UnityEventWrapper OnEnter;
        [SerializeField] UnityEventWrapper OnExit;
        [SerializeField] LayerMask layers;

        public virtual void AddEnterEvent() { }
        public virtual void AddExitEvent() { }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        }

        protected virtual void Start()
        {
            OnEnter.AddListener(AddEnterEvent);
            OnExit.AddListener(AddExitEvent);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (IsTriggered) return;

            if(0 != (layers.value & 1 << other.gameObject.layer))
            {
                Send();
                OnEnter.Invoke();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            OnExit.Invoke(); 
        }
    }
}
