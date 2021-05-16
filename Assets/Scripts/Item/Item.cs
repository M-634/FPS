using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Item : MonoBehaviour, IInteractable
    {
        public Action<GameObject> OnPickUpEvent;
        public Action<GameObject> OnUseEvent;
        public Action OnDropEvent;

        [SerializeField] ItemSettingSOData itemSetting;

        [SerializeField] int id;//識別id (ランダム生成)
        [Tooltip("If item type is Ammo box, maxStackSize is MaxAmmoNumber ")]
        [SerializeField,Range(1, 100)] int maxStackSize = 1;
        [Tooltip("If item type is Ammo box, stackSize is  adding ammoNumber ")]
        [SerializeField,Range(1, 100)] int stackSize = 1;
        [SerializeField] float dropPosOffset = 1f;
        public string ItemName { get; private set; }
        public ItemType ItemType { get; private set; }
        public Sprite Icon { get; private set; }
        public bool Stackable { get; private set; }
        public int MaxStacSize { get => maxStackSize; }
        public int StacSize { get => stackSize; set => stackSize = value; }
        public bool CanUseItem { get; set; } //アイテムを使用できたかどうか判定する

        Rigidbody rb;
        new Collider collider;

        protected virtual void Start()
        {
            SetItemDataFromScriptableObject();

            OnPickUpEvent += (GameObject player) =>
            {
                if (ItemType == ItemType.Rifle)
                {
                    player.GetComponent<PlayerWeaponManager>().CanGetItem(this);
                }
                else
                {
                    player.GetComponent<PlayerItemInventory>().CanGetItem(this);
                }
            };

            OnDropEvent += () =>
            {
                Transform playerCamera = Camera.main.transform;
                transform.position = playerCamera.position + playerCamera.forward * dropPosOffset;
                if (ItemType == ItemType.Rifle)
                    transform.rotation = Quaternion.Euler(0, 0, 90f);
                else
                    transform.rotation = playerCamera.rotation;
                gameObject.SetActive(true);
                Drop(playerCamera);
            };
        }

        private void SetItemDataFromScriptableObject()
        {
            if (itemSetting == null)
            {
                Debug.LogError("アイテム情報のスクリプタブルオブジェクトがアサインされていません");
                return;
            }
            ItemName = itemSetting.itemName;
            ItemType = itemSetting.itemType;
            Icon = itemSetting.icon;
            Stackable = itemSetting.stackable;
        }

        public void Excute(GameObject player)
        {
            OnPickUpEvent?.Invoke(player);
        }


        /// <summary>
        /// コンポーネントをアタッチした時に呼ばれる。
        /// </summary>
        private void Reset()
        {
            SetRigdBodyAndColliderProperty(true, false, CollisionDetectionMode.Discrete, true);
            this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        }

        /// <summary>
        /// インベントリからアイテムを捨てる時に呼ばれる関数
        /// </summary>
        public void Drop(Transform playerCamera)
        {
            SetRigdBodyAndColliderProperty(false, true, CollisionDetectionMode.Continuous, false);
            rb.AddForce(playerCamera.forward * 100f);
        }

        /// <summary>
        /// 地面に付いたら物理挙動を辞める
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                SetRigdBodyAndColliderProperty(true, false, CollisionDetectionMode.Discrete, true);
            }
        }

        /// <summary>
        /// RibidBodyとCollierの各種設定
        /// </summary>
        /// <param name="isKinematic"></param>
        /// <param name="useGravity"></param>
        /// <param name="mode"></param>
        /// <param name="isTrigger"></param>
        private void SetRigdBodyAndColliderProperty(bool isKinematic, bool useGravity, CollisionDetectionMode mode, bool isTrigger)
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();
            if (!collider)
                collider = GetComponent<Collider>();

            rb.useGravity = useGravity;
            rb.isKinematic = isKinematic;
            rb.collisionDetectionMode = mode;
            collider.isTrigger = isTrigger;
        }
    }
}
