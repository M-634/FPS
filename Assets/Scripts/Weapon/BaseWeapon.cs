using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    /// <summary>
    /// 武器はプレイヤーに持たせた状態ででアクティブを切る
    /// </summary>
    public abstract class BaseWeapon : MonoBehaviour
    {
        public KindOfItem KindOfItem;
        public abstract void Attack();
    }
}
