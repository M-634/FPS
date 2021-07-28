using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.NPC
{
    /// <summary>
    /// NPCが近接攻撃する際の当たり判定を制御するクラス
    /// </summary>
    public class NPCMeleeAttack : MonoBehaviour
    {
        [SerializeField] float attackDamage = 50f;
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                if(other.TryGetComponent(out Player.PlayerHealthControl healthControl))
                {
                    healthControl.OnDamage(attackDamage);
                }
            }
        }

        private void Reset()
        {
            GetComponent<Collider>().isTrigger = true; 
        }
    }
}
