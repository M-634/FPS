using UnityEngine;


namespace Musashi.Event
{
    /// <summary>
    /// プレイヤーがトリガーに接したらイベントを発火させ
    /// コマンドを送るクラス
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class OnTriggerEvent : SendCommand
    {
        [SerializeField] UnityEventWrapper OnEnter;
        [SerializeField] UnityEventWrapper OnExit;
        [SerializeField] LayerMask layers;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
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
