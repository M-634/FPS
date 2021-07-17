using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;
using Musashi.Item;
using UnityEngine.UI;
using TMPro;


namespace Musashi.Player
{
    /// <summary>
    /// プレイヤーが扱う武器を制作するクラス。
    /// </summary>
    [RequireComponent(typeof(PlayerInputProvider), typeof(PlayerCharacterStateMchine))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        #region Field
        [SerializeField] WeaponControl startDefultWeapon;
        [SerializeField] Camera weaponCamera;

        [Header("set each pos")]
        [SerializeField] Transform defultWeaponPos;
        [SerializeField] Transform aimmingWeaponPos;
        [SerializeField] Transform downWeaponPos;
        [SerializeField] Transform weaponParentSocket;
        [SerializeField] float adjustPosTime = 0.2f;

        [Header("set aiming propeties")]
        [SerializeField] AnimationCurve weaponChangeCorrectiveCurvel;
        [SerializeField] float weaponUpDuration = 1f;
        [SerializeField] float weaponDownDuration = 1f;

        [Header("current equipment weapon information")]
        [SerializeField] GameObject equipmentWeaponInfoUI;
        [SerializeField] Image equipmentWeaponIcon = default;
        [SerializeField] Image ammoCounterSllider = default;
        [SerializeField] TextMeshProUGUI ammoCounterText = default;

        float targetAimFov;
        Vector3 targetAimPos;

        PlayerInputProvider inputProvider;
        PlayerCharacterStateMchine playerCharacter;
        PlayerItemInventory inventory;

        private WeaponControl currentEquipmentWeapon;
        #endregion

        #region Properties
        public WeaponControl CurrentEquipmentWeapon
        { 
            get => currentEquipmentWeapon; 
            private set
            {
                if(value == null)
                {
                    currentEquipmentWeapon = null;
                    //remove each events
                    currentEquipmentWeapon.CanReloadAmmo -= CurrentEquipmentWeapon_CanReloadAmmo;
                    currentEquipmentWeapon.HaveEndedReloadingAmmo -= CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                    currentEquipmentWeapon.OnChangedAmmo -= CurrentEquipmentWeapon_OnChangedAmmo;
                    return;
                }
                currentEquipmentWeapon = value;
                //set each events;
                currentEquipmentWeapon.CanReloadAmmo += CurrentEquipmentWeapon_CanReloadAmmo;
                currentEquipmentWeapon.HaveEndedReloadingAmmo += CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                currentEquipmentWeapon.OnChangedAmmo += CurrentEquipmentWeapon_OnChangedAmmo;
            }
        }
        #endregion

        #region  Events Methos

        /// <summary>
        /// リロードが完了したらインベントリ内の段数を考慮し、 実際にリロードできる段数を返す関数。
        /// </summary>
        private int CurrentEquipmentWeapon_HaveEndedReloadingAmmo()
        {
            int diff = currentEquipmentWeapon.MaxAmmo - currentEquipmentWeapon.CurrentAmmo;

            if (inventory.SumNumberOfAmmoInInventory - diff >= 0)
            {
                inventory.SumNumberOfAmmoInInventory -= diff;
                return currentEquipmentWeapon.MaxAmmo;
            }

            int temp = currentEquipmentWeapon.CurrentAmmo + inventory.SumNumberOfAmmoInInventory;
            inventory.SumNumberOfAmmoInInventory = 0;
            return temp;
        }

        /// <summary>
        /// リロードできるかどうか判定する関数
        /// </summary>
        private bool CurrentEquipmentWeapon_CanReloadAmmo()
        {
            if (inventory.SumNumberOfAmmoInInventory > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 装備中の武器の銃弾が変化するたびに呼ばれる関数
        /// </summary>
        private void CurrentEquipmentWeapon_OnChangedAmmo()
        {
            ammoCounterText.text = CurrentEquipmentWeapon.CurrentAmmo.ToString() + " | " + inventory.SumNumberOfAmmoInInventory.ToString();
            ammoCounterSllider.fillAmount = (float)CurrentEquipmentWeapon.CurrentAmmo / CurrentEquipmentWeapon.MaxAmmo;
        }
        #endregion

        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            playerCharacter = GetComponent<PlayerCharacterStateMchine>();
            inventory = GetComponent<PlayerItemInventory>();
            InitializeWeapon();
        }

        private void Update()
        {
            if (!CurrentEquipmentWeapon) return;
            InteractiveShooterTypeWeapon();
        }

        private void LateUpdate()
        {
            //装備していないか、銃のアニメーションがIdle状態の時は何もしない。
            if (!CurrentEquipmentWeapon || !CurrentEquipmentWeapon.IsPlayingAnimationStateIdle) return;

            //銃の武器アニメーションはルートモーションで行っている。そのため、リロードやショットを打つたびに座標が若干ずれて行きます。
            //それを防ぐために、補正をかけてweaponParentSocketのローカル座標に合わせてずれを防ぐ。
            CurrentEquipmentWeapon.transform.localPosition = Vector3.Lerp(CurrentEquipmentWeapon.transform.localPosition, Vector3.zero , adjustPosTime * Time.deltaTime);
            CurrentEquipmentWeapon.transform.localRotation = weaponParentSocket.localRotation;
        }

        #region Utility Methods
        private void InteractiveShooterTypeWeapon()
        {
            //Aim
            AimingWeapon(inputProvider.Aim);

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

        private void InitializeWeapon()
        {
            weaponParentSocket.localPosition = downWeaponPos.localPosition;
            var startWeapon = Instantiate(startDefultWeapon, weaponParentSocket);
            startWeapon.transform.localPosition = Vector3.zero;
            startWeapon.transform.rotation = weaponParentSocket.rotation;
            ChangeWeapon(nextWeapon: startWeapon);
        }

        /// <summary>
        /// エイム時の挙動を制御する関数
        /// </summary>
        /// <param name="isAiming"></param>
        private void AimingWeapon(bool isAiming)
        {
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
        /// 武器切り替え時の出し入れを制御する関数
        /// </summary>
        private void ChangeWeapon(WeaponControl nextWeapon, UnityAction callBack = null)
        {
            if (CurrentEquipmentWeapon)
            {
                weaponParentSocket.DOLocalMove(downWeaponPos.localPosition, weaponDownDuration)
                    .SetEase(weaponChangeCorrectiveCurvel)
                    .OnComplete(() =>
                    {
                        CurrentEquipmentWeapon.gameObject.SetActive(false);
                        ResetEquipmentWeaponInfo();
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
                    CurrentEquipmentWeapon = nextWeapon;
                    SetEquipmentWeaponInfo();
                });
        }

        /// <summary>
        /// 装備中の武器情報をUIに表示する関数
        /// </summary>
        public void SetEquipmentWeaponInfo()
        {
            CurrentEquipmentWeapon_OnChangedAmmo();
            //CurrentEquipmentWeapon.OnChangedAmmo += CurrentEquipmentWeapon_OnChangedAmmo;

            if (CurrentEquipmentWeapon.GetIcon)
            {
                equipmentWeaponIcon.sprite = CurrentEquipmentWeapon.GetIcon.sprite;
            }
            equipmentWeaponInfoUI.SetActive(true);
        }

        /// <summary>
        /// 装備中の武器情報をリセットする関数
        /// </summary>
        public void ResetEquipmentWeaponInfo()
        {
            //CurrentEquipmentWeapon.OnChangedAmmo -= CurrentEquipmentWeapon_OnChangedAmmo;

            equipmentWeaponIcon.sprite = null;
            ammoCounterSllider.fillAmount = 0f;
            ammoCounterText.text = "";

            CurrentEquipmentWeapon = null;
            equipmentWeaponInfoUI.SetActive(false);
        }
        #endregion
    }
}
