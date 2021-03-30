using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public abstract class BaseItem : MonoBehaviour, IPickUpObjectable
    {
        public KindOfItem kindOfitem;
        protected bool canPickUp = true;
        protected bool canUseItem = true;

        Rigidbody rb;

        public void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        public virtual void OnPicked()
        {
            canPickUp = Inventory.Instance.CanGetItem(this);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                Destroy(gameObject);
            }
        }

        public virtual bool CanUseItem()
        {
            return canUseItem;
        }

        /// <summary>
        /// インベントリからアイテムを捨てる時に呼ばれる関数
        /// </summary>
        public void ThrowAway()
        {
            if(!rb) 
                rb = GetComponent<Rigidbody>();

            rb.isKinematic = false;
            rb.useGravity = true;
        }


        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.CompareTag("Ground"))
            {
                rb.isKinematic = true;
                rb.useGravity = false;
            }
        }
    }


    /// <summary>
    /// 武器やアイテムの中で拾って使うもの
    /// </summary>
    public interface IPickUpObjectable
    {
        /// <summary>
        /// アイテムを拾った時に呼ばれる関数
        /// </summary>
        void OnPicked();
        /// <summary>
        /// アイテムを使用できたかどうか判定する関数。
        /// </summary>
        /// <returns></returns>
        bool CanUseItem();
    }

    public enum KindOfItem
    {
        //defult
        None = 0,
        //health
        HealthKit, Apple,
        //weapon
        ShotGun, HandGun, AssaultRifle, Grenade, Axe, CrossBow,
        //other
        Battery, Key,
    }
}