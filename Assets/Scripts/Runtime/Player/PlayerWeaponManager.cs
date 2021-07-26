using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Musashi.Weapon;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using Musashi.UI;
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
        [SerializeField] CircularMenu circularWeaponMenu;

        int currentWeaponIndex = 0;//when not having weapon, index = -1;
        bool isChangingWeapon;
        bool isAiming;
        float targetAimFov;
        Vector3 targetAimPos;
        private Tween currentTween;

        private float weaponBobFactor;
        private Vector3 lastPlayerCharacterPosition;
        private Vector3 weaponBobLocalPosition;

        private readonly List<WeaponControl> weaponSlots = new List<WeaponControl>(); //武器スロット。プレハブからインスタンス化したゲームオブジェクトを格納する。同じ武器は持てない
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
                if (value == null)
                {
                    //remove each events
                    currentEquipmentWeapon.CanReloadAmmo -= CurrentEquipmentWeapon_CanReloadAmmo;
                    currentEquipmentWeapon.HaveEndedReloadingAmmo -= CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                    currentEquipmentWeapon.OnChangedAmmo -= CurrentEquipmentWeapon_OnChangedAmmo;
                    currentEquipmentWeapon = null;
                }
                else
                {
                    if (value != currentEquipmentWeapon)
                    {
                        //set each events;
                        currentEquipmentWeapon = value;
                        currentEquipmentWeapon.CanReloadAmmo += CurrentEquipmentWeapon_CanReloadAmmo;
                        currentEquipmentWeapon.HaveEndedReloadingAmmo += CurrentEquipmentWeapon_HaveEndedReloadingAmmo;
                        currentEquipmentWeapon.OnChangedAmmo += CurrentEquipmentWeapon_OnChangedAmmo;
                    }
                }
            }
        }

        /// <summary>
        /// 武器を装備している時に、プレイヤーの入力を受け付けるかどうか判定するプロパティ
        /// </summary>
        public bool CanProcessWeapon => CurrentEquipmentWeapon && !isChangingWeapon;
        #endregion

        #region Private Methods

        #region Unity MonoBehaviour methods
        private void Start()
        {
            inputProvider = GetComponent<PlayerInputProvider>();
            playerCharacter = GetComponent<PlayerCharacterStateMchine>();
            inventory = GetComponent<PlayerItemInventory>();

            inputProvider.EquipWeaponAction += SwitchWeapon;
            inputProvider.HolsterWeaponAction += HolsterWeaponAction;
            inventory.ChangedAmmoInInventoryEvent += CurrentEquipmentWeapon_OnChangedAmmo;

            inputProvider.PlayerInputActions.SwitchCycleWeapon.performed += SwitchCycleWeapon_performed;
            inputProvider.PlayerInputActions.SwitchCycleWeapon.canceled += SwitchCycleWeapon_canceled;

            InitializeWeapon();

            if (circularWeaponMenu != null)
            {
                circularWeaponMenu.OnSelectAction += SwitchWeapon;
            }
        }

        private void Update()
        {
            InteractiveShooterTypeWeapon();
        }
        private void LateUpdate()
        {
            //UpdateWeaponBob();
            UpdateAimingWeapon();
            UpdateWeaponPosOffet();
        }

        /// <summary>
        /// 武器の位置調整をする関数
        /// </summary>
        private void UpdateWeaponPosOffet()
        {
            if (CanProcessWeapon && CurrentEquipmentWeapon.IsPlayingAnimationStateIdle)
            {
                var offsetLoacalPosition = isAiming ? CurrentEquipmentWeapon.GetAimLocalPositionOffset : CurrentEquipmentWeapon.GetDefultLocalPositionOffset;
                //銃の武器アニメーションはルートモーションで行っている。そのため、リロードやショットを打つたびに座標が若干ずれて行きます。
                //それを防ぐために、補正をかけてweaponParentSocketのローカル座標に合わせてずれを防ぐ。
                CurrentEquipmentWeapon.RootPosition = offsetLoacalPosition;
                CurrentEquipmentWeapon.transform.localRotation = weaponParentSocket.localRotation;
            }
        }
        private void OnDestroy()
        {
            if(DOTween.instance != null)
            {
                currentTween.Kill();
            }
        }
        #endregion

        #region methods called from events
        /// <summary>
        /// リロードが完了したらインベントリ内の段数を考慮し、 実際にリロードできる段数を返す関数。
        /// </summary>
        private int CurrentEquipmentWeapon_HaveEndedReloadingAmmo()
        {
            if (CurrentEquipmentWeapon.weaponType == WeaponType.ShotGun)
            {
                if (inventory.SumAmmoInInventory == 0) return 0;
                //一発ずつリロードする
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
        /// リロードできるかどうか判定する関数
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
        /// 装備中の武器の銃弾が変化するたびに呼ばれる関数
        /// </summary>
        private void CurrentEquipmentWeapon_OnChangedAmmo()
        {
            if (CurrentEquipmentWeapon)
            {
                ammoCounterText.text = CurrentEquipmentWeapon.CurrentAmmo.ToString() + " | " + inventory.SumAmmoInInventory.ToString();
                ammoCounterSllider.fillAmount = (float)CurrentEquipmentWeapon.CurrentAmmo / CurrentEquipmentWeapon.MaxAmmo;

                if (circularWeaponMenu)
                {
                    circularWeaponMenu.UptateInfo(currentWeaponIndex, CurrentEquipmentWeapon.CurrentAmmo);
                }
            }
        }
        #endregion

        #region methods called from Update or LaterUpdate
        /// <summary>
        /// プレイヤーの入力に応じて、装備中の武器を操作する関数
        /// </summary>
        private void InteractiveShooterTypeWeapon()
        {
            if (!CanProcessWeapon) return;

            //Aim
            isAiming = inputProvider.Aim;


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
       
        /// <summary>
        /// エイム時の挙動を制御する関数
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
        #endregion

        #region player input action methods
        private void SwitchCycleWeapon_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (GameManager.Instance.ShowConfig || isChangingWeapon || (CurrentEquipmentWeapon && CurrentEquipmentWeapon.Reloding)) return;
            circularWeaponMenu.Show();
        }

        private void SwitchCycleWeapon_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            circularWeaponMenu.Close();
        }

        private void HolsterWeaponAction()
        {
            if (isChangingWeapon) return;
            if (CurrentEquipmentWeapon && CurrentEquipmentWeapon.Reloding) return;
            isChangingWeapon = true;
            currentTween =  MoveWeapon(downWeaponPos.localPosition, weaponDownDuration, weaponChangeCorrectiveCurvel)
                .OnComplete(() =>
                {
                    isChangingWeapon = false;
                    currentWeaponIndex = -1;
                    CurrentEquipmentWeapon.ShowWeapon(false);
                    ResetEquipmentWeaponInfo();
                });
        }

        /// <summary>
        /// プレイヤーの入力に応じて武器を切り替える関数
        /// </summary>
        private void SwitchWeapon(int index)
        {
            if (index == currentWeaponIndex || isChangingWeapon || index >= weaponSlots.Count) return;
            if (CurrentEquipmentWeapon && CurrentEquipmentWeapon.Reloding) return;
            currentWeaponIndex = index;
            ChangeWeapon(weaponSlots[index]);
        }

        /// <summary>
        /// 武器切り替え時の出し入れを制御する関数
        /// </summary>
        private void ChangeWeapon(WeaponControl nextWeapon)
        {
            isChangingWeapon = true;
            if (CurrentEquipmentWeapon)
            {
                //武器を持っているとき
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
                    });
                currentTween = sequence;
            }
            else
            {
                //武器を持っていないとき
                nextWeapon.ShowWeapon(true);
                currentTween = MoveWeapon(defultWeaponPos.localPosition, weaponUpDuration, weaponChangeCorrectiveCurvel)
                    .OnComplete(
                    () =>
                    {
                        SetEquipmentWeaponInfo(nextWeapon);
                        isChangingWeapon = false;
                    });
            }
        }

        /// <summary>
        /// DoTweenを使用して、武器の出し入れの動きを制御する関数.
        /// 動かすのは、WeaponParentSocketオブジェクトであることに注意する
        ///</summary>
        private Tween MoveWeapon(Vector3 targetPos, float duration, AnimationCurve animationCurve)
        {
            return weaponParentSocket.DOLocalMove(targetPos, duration).SetEase(animationCurve);
        }
        #endregion

        #region set/reset weapon info on ui
        /// <summary>
        /// 装備中の武器情報をUIに表示する関数
        /// </summary>
        private void SetEquipmentWeaponInfo(WeaponControl nextWeapon)
        {
            CurrentEquipmentWeapon = nextWeapon;
            if (CurrentEquipmentWeapon.GetIcon)
            {
                equipmentWeaponIcon.sprite = CurrentEquipmentWeapon.GetIcon;
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
        #endregion

        #region Utility 
        /// <summary>
        /// 初期に持たせる武器をプレイヤーに装備、出現位置を設定する関数。
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
        /// 引数で渡されるWeaponControlがアタッチされた武器Prefabを既に持っているかどうか判定する関数
        /// </summary>
        private bool HasWeaponInInventory(WeaponControl weaponPrefab)
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

        #endregion

        #region Public Methods
        /// <summary>
        /// 武器を追加する際の処理を行う関数
        /// </summary>
        /// <param name="pickupWeaponPrefab">インスタンス化させる武器のプレハブ</param>
        /// <returns></returns>
        public bool AddWeapon(WeaponControl pickupWeaponPrefab)
        {
            //if cheack already hold this weapon, don't add the weapon.
            if (HasWeaponInInventory(pickupWeaponPrefab)) return false;

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
            if (circularWeaponMenu)
            {
                circularWeaponMenu.AddInfoInButton(weaponInstance.weaponName, weaponInstance.MaxAmmo, weaponInstance.GetIcon);
            }
            return true;
        }
        #endregion
    }
}