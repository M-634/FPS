using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    public class DetectivePlayer : MonoBehaviour
    {
        [SerializeField] EnemySpawner enemySpawner;

        private void OnTriggerEnter(Collider other)
        {
            if (enemySpawner == null) return;
            if (enemySpawner.IsGenerating || enemySpawner.IsDead) return;

            if (other.transform.CompareTag("Player"))
                enemySpawner.SpwanEnemy();
        }

        private void OnTriggerExit(Collider other)
        {
            if (enemySpawner == null) return;
            if (!enemySpawner.IsGenerating || enemySpawner.IsDead) return;

            if (other.transform.CompareTag("Player"))
                enemySpawner.IsGenerating = false;
        }
    }
}
