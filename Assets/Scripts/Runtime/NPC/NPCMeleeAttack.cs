using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.NPC
{
    /// <summary>
    /// NPC‚ª‹ßÚUŒ‚‚·‚éÛ‚Ì“–‚½‚è”»’è‚ğ§Œä‚·‚éƒNƒ‰ƒX
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
