using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;


namespace Musashi.Player
{
    /// memo:�i���Ǘ��A�G�C�����̃J����FOV��ς���̂����̃N���X���ōs���ˑ��֌W������������
    /// <summary>
    /// �v���C���[����������𐧍삷��N���X�B
    /// </summary>
    [RequireComponent(typeof(InputProvider))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponControl startDefultWeapon;

        [Header("set each pos")]
        [SerializeField] Transform defultWeaponPos;
        [SerializeField] Transform aimmingWeaponPos;
        [SerializeField] Transform downWeaponPos;
        [SerializeField] Transform weaponParentSocket;

        [SerializeField] AnimationCurve weaponChangeCorrectiveCurvel;
        [SerializeField] float weaponUpDuration = 1f;
        [SerializeField] float weaponDownDuration = 1f;

        InputProvider inputProvider;

        private void Start()
        {
            inputProvider = GetComponent<InputProvider>();
            InitializeWeapon();
        }

        private void InitializeWeapon()
        {
            weaponParentSocket.localPosition = downWeaponPos.localPosition;
            var startWeapon = Instantiate(startDefultWeapon, weaponParentSocket);
            startWeapon.transform.SetPositionAndRotation(weaponParentSocket.position, defultWeaponPos.transform.rotation);
            ChangeWeapon(up: true,nextWeapon:startWeapon);
        }

        /// <summary>
        /// ����؂�ւ����̏o������𐧌䂷��֐�
        /// </summary>
        private void ChangeWeapon(bool up,WeaponControl nextWeapon,WeaponControl beforeWeapon = null,  UnityAction callBack = null)
        {
            Vector3 targetPos;
            float duration;
            if (up)
            {
                duration = weaponUpDuration;
                targetPos = defultWeaponPos.localPosition;
            }
            else
            {
                duration = weaponDownDuration;
                targetPos = downWeaponPos.localPosition;
            }
            weaponParentSocket.transform.DOLocalMove(targetPos,duration).SetEase(weaponChangeCorrectiveCurvel)
                .OnComplete(() => 
                {
                  if(callBack != null)
                    {
                        callBack.Invoke();
                    }
                });
        }
    }
}
