using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi.NPC
{
    /// <summary>
    /// 死亡時にエフェクトを出す
    /// </summary>
    public class NPCHealthControl : BaseHealthControl
    {
        [SerializeField] GameObject deathVFX;
        [SerializeField] Transform spwanVFXPoint;

        protected override void AddOnDieEvent()
        {
            if (spwanVFXPoint == null)
            {
                spwanVFXPoint = this.transform;
            }

            if (deathVFX != null)
            {
                deathVFX.transform.SetPositionAndRotation(spwanVFXPoint.position, spwanVFXPoint.rotation);
                deathVFX.SetActive(true);
            }
            else
            {
                Debug.LogWarning($"死亡時エフェクトをアタッチしていません。");
            }
            GameEventManager.Instance.Excute(GameEventType.EnemyDie);
        }
    }
}