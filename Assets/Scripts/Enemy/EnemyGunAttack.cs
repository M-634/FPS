using UnityEngine;

namespace Musashi
{
    public class EnemyGunAttack : MonoBehaviour, IEnemyAttack
    {
        [SerializeField] BaseWeapon currentHaveWeapon;

        public void Excute(EnemyAI owner)
        {
            if (!currentHaveWeapon)
            {
                Debug.Log("攻撃！");
                return;
            }

            //if (currentHaveWeapon.IsCoolTime)
            //{
            //    currentHaveWeapon.UpdateAmmo();
            //}
            //else
            //{
            //    currentHaveWeapon.TryShot();
            //}
        }
    }
}
