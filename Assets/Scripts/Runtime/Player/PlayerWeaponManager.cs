using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Musashi.Player
{
    //memo:���̃Q�[���ł́A������̂Ă�@�\�͎������܂���B�v���C���[�̓Q�[�����ɑ��݂��镐��̎�ނ̐�������������Ă܂��B
    /// <summary>
    /// �v���C���[����������𐧍삷��N���X�B
    /// </summary>
    [RequireComponent(typeof(PlayerInputProvider), typeof(PlayerCharacterStateMchine))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        #region Field
        [Header("General settings")]
        [SerializeField] WeaponControl startDefultWeapon;
        [SerializeField] Camera weaponCamera;

        [Header("Set each position")]
        [SerializeField] Transform defultWeaponPos;
        [SerializeField] Transform aimmingWeaponPos;
        [SerializeField] Transform downWeaponPos;
        [SerializeField] Transform weaponParentSocket;
        [SerializeField] float adjustPosTime = 0.01f;

        [Header("Set aiming propeties")]
        [SerializeField] AnimationCurve weaponChangeCorrectiveCurvel;
        [SerializeField] float weaponUpDuration = 1f;
        [SerializeField] float weaponDownDuration = 1f;

        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        [SerializeField] float bobFrequency = 10f;
        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        [SerializeField] float bobSharpness = 10f;
        [Tooltip("Distance the weapon bobs when not aiming")]
        [SerializeField] float defaultBobAmount = 0.05f;
        [Tooltip("Distance the weapon bobs when aiming")]
        [SerializeField] float aimingBobAmount = 0.02f;

        [Header("Current equipment weapon information")]
        [SerializeField] GameObject equipmentWeaponInfoUI;
        [SerializeField] Image equipmentWeaponIcon = default;
        [SerializeField] Image ammoCounterSllider = default;
        [SerializeField] TextMeshProUGUI ammoCounterText = default;

        [Header("Misc")]
        [Tooltip("Layer to set FPS weapon gameObjects to")]
        [SerializeField] LayerMask FPSWeaponLayer;

        bool isChangingWeapon;
        bool isAiming;
        float targetAimFov;
        Vector3 targetAimPos;

        private float weaponBobFactor;
        private Vector3 lastPlayerCharacterPosition;
        private Vector3 weaponBobLocalPosition;

        private readonly List<WeaponControl> weaponSlots = new List<WeaponControl>(); //����X���b�g�B�v���n�u����C���X�^���X�������Q�[���I�u�W�F�N�g���i�[����B��������͎��ĂȂ�
        private WeaponControl currentEquipmentWeapon;

        PlayerInputProvider inputProvider;
        PlayerCharacterStateMchine playerCharacter;
        PlayerItemInventory inventory;
        #endregion

        #region Properties
        public WeaponControl CurrentEquipmentWeapon
        {
            get => currentEquipmentWeapon;
            private set
            {
                currentEquipmentWeapon = value;
                if (currentEquipmentWeapon == null)
                {
                    //remove each events
                    currentEquipmentWeapon.CanReloadAmmo -= CurrentEquipmentWeapon_CanReloadAmmo;
                    currentEquipmentWeapon.HaveEndedReloadingAmmo -= CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                    currentEquipmentWeapon.OnChangedAmmo -= CurrentEquipmentWeapon_OnChangedAmmo;
                    return;
                }
                //set each events;
                currentEquipmentWeapon.CanReloadAmmo += CurrentEquipmentWeapon_CanReloadAmmo;
                currentEquipmentWeapon.HaveEndedReloadingAmmo += CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                currentEquipmentWeapon.OnChangedAmmo += CurrentEquipmentWeapon_OnChangedAmmo;
            }
        }

        /// <summary>
        /// ����𑕔����Ă��鎞�ɁA�v���C���[�̓��͂��󂯕t���邩�ǂ������肷��v���p�e�B
        /// </summary>
        public bool CanProcessWeapon => CurrentEquipmentWeapon && !isChangingWeapon;
        #endregion

        #region Private Methods
        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            playerCharacter = GetComponent<PlayerCharacterStateMchine>();
            inventory = GetComponent<PlayerItemInventory>();

            inventory.ChangedAmmoInInventoryEvent += CurrentEquipmentWeapon_OnChangedAmmo;
            InitializeWeapon();
        }

        private void Update()
        {
            SwitchWeapon();
            InteractiveShooterTypeWeapon();
        }

        private void LateUpdate()
        {
            //UpdateWeaponBob();
            UpdateAimingWeapon();
            UpdateWeaponPosOffet();
        }

        /// <summary>
        /// ����̈ʒu����������֐�
        /// </summary>
        private void UpdateWeaponPosOffet()
        {
            if (CanProcessWeapon && CurrentEquipmentWeapon.IsPlayingAnimationStateIdle)
            {
                var offsetLoacalPosition = isAiming ? CurrentEquipmentWeapon.GetAimLocalPositionOffset : CurrentEquipmentWeapon.GetDefultLocalPositionOffset;
                //�e�̕���A�j���[�V�����̓��[�g���[�V�����ōs���Ă���B���̂��߁A�����[�h��V���b�g��ł��тɍ��W���኱����čs���܂��B
                //�����h�����߂ɁA�␳��������weaponParentSocket�̃��[�J�����W�ɍ��킹�Ă����h���B
                CurrentEquipmentWeapon.RootPosition = offsetLoacalPosition;
                CurrentEquipmentWeapon.transform.localRotation = weaponParentSocket.localRotation;
            }
        }

        /// <summary>
        /// �����[�h������������C���x���g�����̒i�����l�����A ���ۂɃ����[�h�ł���i����Ԃ��֐��B
        /// </summary>
        private int CurrentEquipmentWeapon_HaveEndedReloadingAmmo()
        {
            if (CurrentEquipmentWeapon.weaponType == WeaponType.ShotGun)
            {
                //�ꔭ�������[�h����
                inventory.SumAmmoInInventory -= 1;
                return 1;
            }

            int diff = currentEquipmentWeapon.MaxAmmo - currentEquipmentWeapon.CurrentAmmo;

            if (inventory.SumAmmoInInventory - diff >= 0)
            {
                inventory.SumAmmoInInventory -= diff;
                return currentEquipmentWeapon.MaxAmmo;
            }

            int temp = currentEquipmentWeapon.CurrentAmmo + inventory.SumAmmoInInventory;
            inventory.SumAmmoInInventory = 0;
            return temp;
        }

        /// <summary>
        /// �����[�h�ł��邩�ǂ������肷��֐�
        /// </summary>
        private bool CurrentEquipmentWeapon_CanReloadAmmo()
        {
            if (inventory.SumAmmoInInventory > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// �������̕���̏e�e���ω����邽�тɌĂ΂��֐�
        /// </summary>
        private void CurrentEquipmentWeapon_OnChangedAmmo()
        {
            if (CurrentEquipmentWeapon)
            {
                ammoCounterText.text = CurrentEquipmentWeapon.CurrentAmmo.ToString() + " | " + inventory.SumAmmoInInventory.ToString();
                ammoCounterSllider.fillAmount = (float)CurrentEquipmentWeapon.CurrentAmmo / CurrentEquipmentWeapon.MaxAmmo;
            }
        }

        private void InteractiveShooterTypeWeapon()
        {
            if (!CanProcessWeapon) return;

            //Aim
            isAiming = inputProvider.Aim;

            if (!CurrentEquipmentWeapon.CanInputAction) return;

            //Shoot
            switch (CurrentEquipmentWeapon.GetWeaponShootType)
            {
                case WeaponShootType.Manual:
                    if (inputProvider.Fire)
                    {
                        CurrentEquipmentWeapon.TryShot();
                    }
                    break;
                case WeaponShootType.Automatic:
                    if (inputProvider.HeldFire)
                    {
                        CurrentEquipmentWeapon.TryShot();
                    }
                    break;
            }

            //Reload
            if (inputProvider.Reload)
            {
                CurrentEquipmentWeapon.StartReload();
            }
        }

        /// <summary>
        /// �v���C���[�̓��͂ɉ����ĕ����؂�ւ���֐�
        /// </summary>
        private void SwitchWeapon()
        {
            if (isChangingWeapon) return;

            //test 
            int i = inputProvider.SwichWeaponID;
            if (i >= weaponSlots.Count) return;

            var nextWeapon = weaponSlots[i];
            if (nextWeapon != CurrentEquipmentWeapon)
            {
                ChangeWeapon(nextWeapon);
            }
        }

        /// <summary>
        /// �v���C���[���Q�[���X�^�[�g���Ɏ�����𑕔�������i���������j�֐�
        /// </summary>
        private void InitializeWeapon()
        {
            weaponParentSocket.localPosition = downWeaponPos.localPosition;
            if (startDefultWeapon != null && AddWeapon(startDefultWeapon))
            {
                ChangeWeapon(weaponSlots.First());
            }
        }

        /// <summary>
        /// �G�C�����̋����𐧌䂷��֐�
        /// </summary>
        /// <param name="isAiming"></param>
        private void UpdateAimingWeapon()
        {
            if (!CanProcessWeapon) return;

            if (isAiming)
            {
                targetAimFov = CurrentEquipmentWeapon.AimCameraFOV;
                targetAimPos = aimmingWeaponPos.localPosition;
            }
            else
            {
                targetAimFov = playerCharacter.DefultFieldOfView;
                targetAimPos = defultWeaponPos.localPosition;
            }
            float aimSpeed = CurrentEquipmentWeapon.AimSpeed;

            //set camera
            playerCharacter.SetFovOfCamera(isAiming, targetAimFov, aimSpeed);
            weaponCamera.fieldOfView = targetAimFov;

            weaponParentSocket.localPosition = Vector3.Lerp(weaponParentSocket.localPosition, targetAimPos, aimSpeed * Time.deltaTime);
        }

        /// <summary>
        /// �v���C���[���n�ʂɂ��鎞�A�ړ����ɕ����h�炷�����𐧌䂷��֐�
        /// (�����ۗ�)
        /// </summary>
        private void UpdateWeaponBob()
        {
            if (Time.deltaTime > 0f)
            {
                var velo = (playerCharacter.transform.position - lastPlayerCharacterPosition) / Time.deltaTime;
                float characterMovementFactor = 0f;
                if (playerCharacter.IsGround)
                {
                    // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                    characterMovementFactor = Mathf.Clamp01(velo.magnitude / playerCharacter.LimitGroundedMovemet);

                }
                weaponBobFactor = Mathf.Lerp(weaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);

                // Calculate vertical and horizontal weapon bob values based on a sine function
                float bobAmount = isAiming ? aimingBobAmount : defaultBobAmount;
                float frequency = bobFrequency;
                float hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * weaponBobFactor;
                float vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * weaponBobFactor;

                // Apply weapon bob
                weaponBobLocalPosition.x = hBobValue;
                weaponBobLocalPosition.y = Mathf.Abs(vBobValue);

                lastPlayerCharacterPosition = playerCharacter.transform.position;
            }

        }

        /// <summary>
        /// ����؂�ւ����̏o������𐧌䂷��֐�
        /// </summary>
        private void ChangeWeapon(WeaponControl nextWeapon, UnityAction callBack = null)
        {
            isChangingWeapon = true;
            if (CurrentEquipmentWeapon)
            {
                //����������Ă���Ƃ�
                var sequence = DOTween.Sequence();
                sequence.Append(MoveWeapon(downWeaponPos.localPosition, weaponDownDuration, weaponChangeCorrectiveCurvel))
                    .AppendCallback(
                    () =>
                    {
                        nextWeapon.ShowWeapon(true);
                        CurrentEquipmentWeapon.ShowWeapon(false, ResetEquipmentWeaponInfo);
                    })
                    .Append(MoveWeapon(defultWeaponPos.localPosition, weaponUpDuration, weaponChangeCorrectiveCurvel))
                    .AppendCallback(
                    () =>
                    {
                        SetEquipmentWeaponInfo(nextWeapon);
                        isChangingWeapon = false;
                        if (callBack != null) callBack.Invoke();
                    });
            }
            else
            {
                //����������Ă��Ȃ��Ƃ�
                nextWeapon.ShowWeapon(true);
                MoveWeapon(defultWeaponPos.localPosition, weaponUpDuration, weaponChangeCorrectiveCurvel)
                    .OnComplete(
                    () =>
                    {
                        SetEquipmentWeaponInfo(nextWeapon);
                        isChangingWeapon = false;
                        if (callBack != null) callBack.Invoke();
                    });
            }
        }

        /// <summary>
        /// DoTween���g�p���āA����̏o������̓����𐧌䂷��֐�.
        /// �������̂́AWeaponParentSocket�I�u�W�F�N�g�ł��邱�Ƃɒ��ӂ���
        ///</summary>
        private Tween MoveWeapon(Vector3 targetPos, float duration, AnimationCurve animationCurve)
        {
            return weaponParentSocket.DOLocalMove(targetPos, duration).SetEase(animationCurve);
        }

        /// <summary>
        /// �������̕������UI�ɕ\������֐�
        /// </summary>
        private void SetEquipmentWeaponInfo(WeaponControl nextWeapon)
        {
            CurrentEquipmentWeapon = nextWeapon;
            if (CurrentEquipmentWeapon.GetIcon)
            {
                equipmentWeaponIcon.sprite = CurrentEquipmentWeapon.GetIcon.sprite;
            }
            equipmentWeaponInfoUI.SetActive(true);
            CurrentEquipmentWeapon_OnChangedAmmo();
        }

        /// <summary>
        /// �������̕���������Z�b�g����֐�
        /// </summary>
        private void ResetEquipmentWeaponInfo()
        {
            equipmentWeaponIcon.sprite = null;
            ammoCounterSllider.fillAmount = 0f;
            ammoCounterText.text = "";

            equipmentWeaponInfoUI.SetActive(false);
            CurrentEquipmentWeapon = null;
        }

        /// <summary>
        /// �����œn�����WeaponControl���A�^�b�`���ꂽ����Prefab�����Ɏ����Ă��邩�ǂ������肷��֐�
        /// </summary>
        private bool HasWeapon(WeaponControl weaponPrefab)
        {
            foreach (var w in weaponSlots)
            {
                if (w.SourcePrefab == weaponPrefab.gameObject)
                {
                    return true;
                }
            }
            return false;

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// �����ǉ�����ۂ̏������s���֐�
        /// </summary>
        /// <param name="pickupWeaponPrefab">�C���X�^���X�������镐��̃v���n�u</param>
        /// <returns></returns>
        public bool AddWeapon(WeaponControl pickupWeaponPrefab)
        {
            //if cheack already hold this weapon, don't add the weapon.
            if (HasWeapon(pickupWeaponPrefab)) return false;

            //instance weapon and set localPosition and rotation
            var weaponInstance = Instantiate(pickupWeaponPrefab, weaponParentSocket);
            weaponInstance.RootPosition = weaponInstance.GetDefultLocalPositionOffset;
            weaponInstance.transform.rotation = weaponParentSocket.rotation;

            //set  sorcePrefab and don't active weapon
            weaponInstance.SourcePrefab = pickupWeaponPrefab.gameObject;
            weaponInstance.ShowWeapon(false);

            // Assign the first person layer to the weapon
            int layerIndex = Mathf.RoundToInt(Mathf.Log(FPSWeaponLayer.value, 2)); // This function converts a layermask to a layer index
            foreach (Transform t in weaponInstance.gameObject.GetComponentsInChildren<Transform>(true))
            {
                t.gameObject.layer = layerIndex;
            }

            weaponSlots.Add(weaponInstance);
            return true;
        }
        #endregion
    }
}