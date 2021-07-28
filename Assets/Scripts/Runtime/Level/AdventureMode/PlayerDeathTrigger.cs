using UnityEngine;

namespace Musashi.Level.AdventureMode
{
    /// <summary>
    /// アドベンチャーモードの落下地点に置くオブジェクト
    /// プレイヤーが接触したら死亡する
    /// </summary>
    [RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]
    public class PlayerDeathTrigger : MonoBehaviour
    {
        [SerializeField] AdventureModeGameFlowManager pointManager;

        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.TryGetComponent(out Player.PlayerTranslate player))
            {
                //player.Translate(pointManager.CurrentSavePoint);
                GameEventManager.Instance.Excute(GameEventType.SpwanPlayer);
            }
        }
    }
}
