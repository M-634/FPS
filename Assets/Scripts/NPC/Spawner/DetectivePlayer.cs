using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public class DetectivePlayer : MonoBehaviour
    {
        [SerializeField] EnemySpawner enemySpawner;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.transform.CompareTag("Player")) return;
  
            if (enemySpawner == null || enemySpawner.IsGenerating || enemySpawner.IsDead) return;

            enemySpawner.SpwanEnemy();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.transform.CompareTag("Player")) return;
     
            if (enemySpawner == null || !enemySpawner.IsGenerating || enemySpawner.IsDead) return;

            enemySpawner.IsGenerating = false;
        }
    }
}
