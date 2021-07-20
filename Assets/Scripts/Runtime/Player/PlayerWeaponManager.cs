using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;
using Musashi.Item;
using UnityEngine.UI;
using TMPro;
using System;

namespace Musashi.Player
{
    //memo:このゲームでは、武器を捨てる機能は実装しません。プレイヤーはゲーム内に存在する武器の種類の数だけ武器を持てます。
    /// <summary>
    /// プレイヤーが扱う武器を制作するクラス。
    /// </summary>
    [RequireComponent(typeof(PlayerInputProvider), typeof(PlayerCharacterStateMchine))]
    public class PlayerWeaponManager : MonoBehaviour
    {
        #region Field
        [SerializeField] WeaponControl startDefultWeapon;
        [SerializeField] Camera weaponCamera;

        [Header("Set each position")]
        [SerializeField] Transform defultWeaponPos;
        [SerializeField] Transform aimmingWeaponPos;
        [SerializeField] Transform downWeaponPos;
        [SerializeField] Transform weaponParentSocket;
        [SerializeField] float adjustPosTime = 0.2f;

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

        //public : debug
        public List<WeaponControl> weaponSlots = new List<WeaponControl>(); //武器スロット。同じ武器は持てない
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

        /// <summary>装備している武器の操作ができるかどうか判定する/// </summary>
        public bool CanEquipmentWeaponAction => !isChangingWeapon && CurrentEquipmentWeapon != null;
        #endregion

        #region Private Methods
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

        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            playerCharacter = GetComponent<PlayerCharacterStateMchine>();
            inventory = GetComponent<PlayerItemInventory>();
            InitializeWeapon();
        }

        private void Update()
        {
            if (!CanEquipmentWeaponAction) return;
            SwitchWeapon();
            InteractiveShooterTypeWeapon();
        }

        /// <summary>
        /// プレイヤーの入力に応じて武器を切り替える関数
        /// </summary>
        private void SwitchWeapon()
        {
            //test 
            int i = inputProvider.SwichWeaponID;
            if (i == -1 || i > weaponSlots.Count) return;

            var nextWeapon = weaponSlots[i];
            if (nextWeapon != CurrentEquipmentWeapon)
            {
                ChangeWeapon(nextWeapon);
            }
        }

        private void LateUpdate()
        {
            if (!CanEquipmentWeaponAction) return;

            UpdateAimingWeapon();
            //UpdateWeaponBob();

            if (CurrentEquipmentWeapon.IsPlayingAnimationStateIdle)
            {
                //銃の武器アニメーションはルートモーションで行っている。そのため、リロードやショットを打つたびに座標が若干ずれて行きます。
                //それを防ぐために、補正をかけてweaponParentSocketのローカル座標に合わせてずれを防ぐ。
                CurrentEquipmentWeapon.transform.localPosition = Vector3.Lerp(CurrentEquipmentWeapon.transform.localPosition, Vector3.zero, adjustPosTime * Time.deltaTime);
                CurrentEquipmentWeapon.transform.localRotation = weaponParentSocket.localRotation;
            }
        }
        private void InteractiveShooterTypeWeapon()
        {
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
        /// プレイヤーがゲームスタート時に持つ武器を装備させる（初期処理）関数
        /// </summary>
        private void InitializeWeapon()
        {
            weaponParentSocket.localPosition = downWeaponPos.localPosition;
            if (AddWeapon(startDefultWeapon))
            {
                ChangeWeapon(weaponSlots[0]);
            }
        }

        /// <summary>
        /// エイム時の挙動を制御する関数
        /// </summary>
        /// <param name="isAiming"></param>
        private void UpdateAimingWeapon()
        {
            if (isAiming)
            {
                targetAimFov = CurrentEquipmentWeapon.AimCameraFOV;
                targetAimPos = aimmingWeaponPos.localPosition + CurrentEquipmentWeapon.GetAimLocalPositionOffset;
            }
            else
            {
                targetAimFov = playerCharacter.DefultFieldOfView;
                targetAimPos = defultWeaponPos.localPosition + CurrentEquipmentWeapon.GetDefultLocalPositionOffset;
            }
            float aimSpeed = CurrentEquipmentWeapon.AimSpeed;

            //set camera
            playerCharacter.SetFovOfCamera(isAiming, targetAimFov, aimSpeed);
            weaponCamera.fieldOfView = targetAimFov;

            weaponParentSocket.localPosition = Vector3.Lerp(weaponParentSocket.localPosition, targetAimPos, aimSpeed * Time.deltaTime);
        }

        /// <summary>
        /// プレイヤーが地面にいる時、移動時に武器を揺らす動きを制御する関数
        /// (実装保留)
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
        /// 武器切り替え時の出し入れを制御する関数
        /// </summary>
        private void ChangeWeapon(WeaponControl nextWeapon, UnityAction callBack = null)
        {
            isChangingWeapon = true;
            if (CurrentEquipmentWeapon)
            {
                //武器を持っているとき
                var sequence = DOTween.Sequence();
                sequence.Append(MoveWeapon(downWeaponPos, weaponDownDuration, weaponChangeCorrectiveCurvel))
                    .AppendCallback(
                    () =>
                    {
                        nextWeapon.ShowWeapon(true);
                        CurrentEquipmentWeapon.ShowWeapon(false, ResetEquipmentWeaponInfo);
                    })
                    .Append(MoveWeapon(defultWeaponPos, weaponUpDuration, weaponChangeCorrectiveCurvel))
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
                //武器を持っていないとき
                nextWeapon.ShowWeapon(true);
                MoveWeapon(defultWeaponPos, weaponUpDuration, weaponChangeCorrectiveCurvel)
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
        /// DoTweenを使用して、武器の出し入れの動きを制御する関数
        private Tween MoveWeapon(Transform targetPos, float duration, AnimationCurve animationCurve)
        {
            return weaponParentSocket.DOLocalMove(targetPos.localPosition, duration).SetEase(animationCurve);
        }

        /// <summary>
        /// 装備中の武器情報をUIに表示する関数
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
        /// 装備中の武器情報をリセットする関数
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
        /// 引数で渡されるWeaponControlがアタッチされた武器Prefabを既に持っているかどうか判定する関数
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
            #endregion

        }

        #region Public Methods
        /// <summary>
        /// 武器を追加する際の処理を行う関数
        /// </summary>
        /// <param name="pickupWeaponPrefab">インスタンス化させる武器のプレハブ</param>
        /// <returns></returns>
        public bool AddWeapon(WeaponControl pickupWeaponPrefab)
        {
            //if cheack already hold this weapon, don't add the weapon.
            if (HasWeapon(pickupWeaponPrefab)) return false;

            //instance weapon and set localPosition and rotation
            var weaponInstance = Instantiate(pickupWeaponPrefab, weaponParentSocket);
            weaponInstance.transform.localPosition = Vector3.zero + weaponInstance.GetDefultLocalPositionOffset;
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