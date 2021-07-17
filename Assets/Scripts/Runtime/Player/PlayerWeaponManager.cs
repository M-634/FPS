using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;
using Musashi.Item;
using UnityEngine.UI;
using TMPro;

namespace Musashi.Weapon
{
    /// <summary>
    /// �������̕������UI�ɕ\������̂ɕK�v�ȕϐ���p�ӂ��A
    /// �����ɂ܂�鏈���𐧌䂷��N���X
    /// </summary>
    [System.Serializable]
    public class EquipmentWeaponInfo
    {
        [SerializeField] GameObject equipmentWeaponInfoUI;
        [SerializeField] Image equipmentWeaponIcon = default;
        [SerializeField] Image ammoCounterSllider = default;
        [SerializeField] TextMeshProUGUI ammoCounterText = default;

        WeaponControl weapon;
        public GameObject GetInfoUI => equipmentWeaponInfoUI;

        public void SetCurrentEquipmentWeaponInfo(WeaponControl currentEquipmentWeapon)
        {
            weapon = currentEquipmentWeapon;
            UpdateAmmmoCounter();
            weapon.OnChangedAmmo += UpdateAmmmoCounter;

            if (weapon.GetIcon)
            {
                equipmentWeaponIcon.sprite = weapon.GetIcon.sprite;
            }
            GetInfoUI.SetActive(true);
        }

        private void UpdateAmmmoCounter()
        {
            ammoCounterText.text = weapon.CurrentAmmo.ToString() + " | " + ItemInventory.Instance.SumNumberOfAmmoInInventory.ToString();
            ammoCounterSllider.fillAmount = (float)weapon.CurrentAmmo / weapon.MaxAmmo;
        }

        public void ResetInfo()
        {
            weapon.OnChangedAmmo -= UpdateAmmmoCounter;

            equipmentWeaponIcon.sprite = null;
            ammoCounterSllider.fillAmount = 0f;
            ammoCounterText.text = "";

            weapon = null;
            GetInfoUI.SetActive(false);
        }
    }
}

namespace Musashi.Player
{
    /// memo:�i���Ǘ��A�G�C�����̃J����FOV��ς���̂����̃N���X���ōs���ˑ��֌W������������
    /// <summary>
    /// �v���C���[����������𐧍삷��N���X�B
    /// </summary>
    [RequireComponent(typeof(InputProvider), typeof(PlayerCharacterStateMchine))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        [SerializeField] WeaponControl startDefultWeapon;
        [SerializeField] Camera weaponCamera;

        [Header("set each pos")]
        [SerializeField] Transform defultWeaponPos;
        [SerializeField] Transform aimmingWeaponPos;
        [SerializeField] Transform downWeaponPos;
        [SerializeField] Transform weaponParentSocket;

        [SerializeField] AnimationCurve weaponChangeCorrectiveCurvel;
        [SerializeField] float weaponUpDuration = 1f;
        [SerializeField] float weaponDownDuration = 1f;

        [SerializeField] EquipmentWeaponInfo equipmentWeaponInfo = default;

        InputProvider inputProvider;
        PlayerCharacterStateMchine playerCharacter;

        WeaponControl currentEquipmentWeapon;

        float targetAimFov;
        Vector3 targetAimPos;

        private void Start()
        {
            inputProvider = GetComponent<InputProvider>();
            playerCharacter = GetComponent<PlayerCharacterStateMchine>();
            InitializeWeapon();
        }
        private void InitializeWeapon()
        {
            weaponParentSocket.localPosition = downWeaponPos.localPosition;
            var startWeapon = Instantiate(startDefultWeapon, weaponParentSocket);
            startWeapon.transform.localPosition = Vector3.zero;
            startWeapon.transform.rotation = weaponParentSocket.rotation;
            ChangeWeapon(nextWeapon: startWeapon);
        }
        private void Update()
        {
            if (!currentEquipmentWeapon) return;

            //Aim
            AimingWeapon(inputProvider.Aim);

            //Shoot
            switch (currentEquipmentWeapon.GetWeaponShootType)
            {
                case WeaponShootType.Manual:
                    if (inputProvider.Fire)
                    {
                        currentEquipmentWeapon.TryShot();
                    }
                    break;
                case WeaponShootType.Automatic:
                    if (inputProvider.HeldFire)
                    {
                        currentEquipmentWeapon.TryShot();
                    }
                    break;
            }
        }

        /// <summary>
        /// �G�C�����̋����𐧌䂷��֐�
        /// </summary>
        /// <param name="isAiming"></param>
        private void AimingWeapon(bool isAiming)
        {
            if (isAiming)
            {
                targetAimFov = currentEquipmentWeapon.AimCameraFOV;
                targetAimPos = aimmingWeaponPos.localPosition;
            }
            else
            {
                targetAimFov = playerCharacter.DefultFieldOfView;
                targetAimPos = defultWeaponPos.localPosition;
            }
            float aimSpeed = currentEquipmentWeapon.AimSpeed;

            //set camera
            playerCharacter.SetFovOfCamera(isAiming, targetAimFov, aimSpeed);
            weaponCamera.fieldOfView = targetAimFov;

            weaponParentSocket.localPosition = Vector3.Lerp(weaponParentSocket.localPosition, targetAimPos, aimSpeed * Time.deltaTime);
        }

        /// <summary>
        /// ����؂�ւ����̏o������𐧌䂷��֐�
        /// </summary>
        private void ChangeWeapon(WeaponControl nextWeapon, UnityAction callBack = null)
        {
            if (currentEquipmentWeapon)
            {
                weaponParentSocket.DOLocalMove(downWeaponPos.localPosition, weaponDownDuration)
                    .SetEase(weaponChangeCorrectiveCurvel)
                    .OnComplete(() =>
                    {
                        currentEquipmentWeapon.gameObject.SetActive(false);
                        equipmentWeaponInfo.ResetInfo();

                        nextWeapon.gameObject.SetActive(true);
                    });
            }

            weaponParentSocket.DOLocalMove(defultWeaponPos.localPosition, weaponUpDuration)
                .SetEase(weaponChangeCorrectiveCurvel)
                .OnComplete(() =>
                {
                    if (callBack != null)
                    {
                        callBack.Invoke();
                    }
                    currentEquipmentWeapon = nextWeapon;
                    equipmentWeaponInfo.SetCurrentEquipmentWeaponInfo(currentEquipmentWeapon);
                });
        }
    }
}
