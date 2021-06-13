using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敵の攻撃アニメーション中に設定されたイベントから当たり判定のOn OFFを制御するクラス
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyAttackAnimationEvent : MonoBehaviour
{
    [SerializeField] Collider hitCheakCollider;
    private void Start()
    {
        hitCheakCollider.enabled = false;
    }

    public void StartAttack()
    {
        hitCheakCollider.enabled = true;
    }

    public void EndAttack()
    {
        hitCheakCollider.enabled = false;
    }
}
