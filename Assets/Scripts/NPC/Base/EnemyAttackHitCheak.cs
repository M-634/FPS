using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    ///Playerに当たり判定のコライダーが接触したかどうか調べ、
    ///当たったら攻撃力分のダメージをPlayerに与える
    /// </summary>
    public class EnemyAttackHitCheak : MonoBehaviour
    {
        [SerializeField] float attackDamage = 0f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if(other.TryGetComponent(out IDamageable target))
                {
                    target.OnDamage(attackDamage);
                    Debug.Log($"{target}に{attackDamage}のダメージを与えた");
                }
            }
        }
    } 
}
