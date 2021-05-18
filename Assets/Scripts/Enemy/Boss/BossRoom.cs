using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// ボス部屋のエントランスをプレイヤーが通過したら、
    /// カットシーンを再生する
    /// </summary>
    public class BossRoom : MonoBehaviour
    {
        bool hasEntered;
        private void OnTriggerEnter(Collider other)
        {
            if (hasEntered) return;

            if (other.CompareTag("Player"))
            {
                hasEntered = true;
                Debug.Log("start cut movie");
            }
        }
    }
}
