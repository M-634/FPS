using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// 武器やアイテムの中で拾って使うもの
    /// </summary>
    public abstract class CanPickUpObject : MonoBehaviour, IInteractable
    {
        Rigidbody rb;
        new Collider collider;
        public virtual void OnPicked(GameObject player) { }

        public void Excute(GameObject player)
        {
            OnPicked(player);
        }

        /// <summary>
        /// インベントリからアイテムを捨てる時に呼ばれる関数
        /// </summary>
        public void Drop()
        {
            if (!rb)
                rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;//地面に埋まるのを防止するため
            rb.AddForce(transform.forward * 100f);
            rb.useGravity = true;

            if (!collider)
                collider = GetComponent<Collider>();
            collider.isTrigger = false;
        }

        /// <summary>
        /// 地面に付いたら物理挙動を辞める
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                rb.useGravity = false;
                rb.collisionDetectionMode = CollisionDetectionMode.Discrete;
                rb.isKinematic = true;
                collider.isTrigger = true;
            }
        }
    }
}