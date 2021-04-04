using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public enum KindOfItem
    {
        //defult
        None = 0,
        //health
        HealthKit, Apple,
        //other
        Battery, Key,AmmoBox,
    }

    public abstract class BaseItem : MonoBehaviour, IPickUpObjectable
    {
        public KindOfItem kindOfItem;
        protected bool canPickUp = true;
        protected bool canUseItem = true;

        Rigidbody rb;

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody>();
        }
        public virtual void OnPicked()
        {
            canPickUp = ItemInventory.Instance.CanGetItem(this,1);
            if (canPickUp)
            {
                GameManager.Instance.SoundManager.PlaySE(SoundName.PickUP);
                Destroy(gameObject);
            }
        }

        public virtual bool CanUseObject()
        {
            return canUseItem;
        }

        /// <summary>
        /// インベントリからアイテムを捨てる時に呼ばれる関数
        /// </summary>
        public virtual void Drop()
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
}