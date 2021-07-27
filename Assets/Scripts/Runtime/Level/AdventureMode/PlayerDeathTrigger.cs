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
        [SerializeField] SavePointManager pointManager;
        private void Reset()
        {
            GetComponent<BoxCollider>().isTrigger = true;
            GetComponent<Rigidbody>().isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                //charactor controller を直接テレポートさせるとバグるので、インスタンス化させる。
                Destroy(other.transform.parent.gameObject);
                pointManager.SpwanPlayer();
            }
        }
    }
}
