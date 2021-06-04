using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Musashi.NPC
{
    /// <summary>
    /// 敵AIのヘルパー関数
    /// </summary>
    public static class NPCAIHelper
    {
        public static bool CanSeePlayer(Transform player,Transform self,float visitDistance,float viewingAngle,Transform eye)
        {
            Vector3 dir = player.position - self.position;
            float angle = Vector3.Angle(dir, self.forward);

            if (dir.magnitude < visitDistance && angle < viewingAngle)
            {
                //Playerと敵の間に障害物があるかどうかRayを飛ばして確かめる
                if (Physics.Linecast(eye.position, player.position, out RaycastHit hit))
                {
                    Debug.DrawLine(player.position, player.position, Color.white);
                    if (hit.collider.gameObject.CompareTag("Player"))
                    {
                        return true;
                    }
                    return false;
                }
            }
            return false;
        }

        public static bool CanAttackPlayer(Transform player, Transform self,float attackRange)
        {
            Vector3 dir = player.position - self.position;
            if (dir.magnitude < attackRange)
            {
                return true;
            }
            return false;
        }

        public static void LookAtPlayer(Transform player,Transform self,float duration)
        {
            var aim = player.position - self.position;
            aim.y = 0;
            var look = Quaternion.LookRotation(aim);
            self.rotation = Quaternion.Slerp(self.rotation, look, duration);
        }
    }
}
