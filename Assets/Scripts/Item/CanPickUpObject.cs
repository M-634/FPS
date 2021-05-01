using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// 武器やアイテムの中で拾って使うもの
    /// </summary>
    [RequireComponent(typeof(Rigidbody),typeof(Collider))]
    public abstract class CanPickUpObject : MonoBehaviour//, IInteractable
    {
        //Rigidbody rb;
        //new Collider collider;

        ///// <summary>
        ///// コンポーネントをアタッチした時に呼ばれる。
        ///// </summary>
        //private void Reset()
        //{
        //    SetRigdBodyAndColliderProperty(true, false, CollisionDetectionMode.Discrete, true);
        //    this.gameObject.layer = LayerMask.NameToLayer("Interactable");
        //}

        //public virtual void OnPicked(GameObject player) { }

        //public void Excute(GameObject player)
        //{
        //    OnPicked(player);
        //}

        ///// <summary>
        ///// インベントリからアイテムを捨てる時に呼ばれる関数
        ///// </summary>
        //public void Drop(Transform playerCamera)
        //{
        //    SetRigdBodyAndColliderProperty(false, true, CollisionDetectionMode.Continuous, false);
        //    rb.AddForce(playerCamera.forward * 100f);
        //}

        ///// <summary>
        ///// 地面に付いたら物理挙動を辞める
        ///// </summary>
        ///// <param name="collision"></param>
        //private void OnCollisionEnter(Collision collision)
        //{
        //    if (collision.collider.CompareTag("Ground"))
        //    {
        //        SetRigdBodyAndColliderProperty(true, false, CollisionDetectionMode.Discrete, true);
        //    }
        //}

        ///// <summary>
        ///// RibidBodyとCollierの各種設定
        ///// </summary>
        ///// <param name="isKinematic"></param>
        ///// <param name="useGravity"></param>
        ///// <param name="mode"></param>
        ///// <param name="isTrigger"></param>
        //private void SetRigdBodyAndColliderProperty(bool isKinematic, bool useGravity, CollisionDetectionMode mode,bool isTrigger)
        //{
        //    if (!rb)
        //        rb = GetComponent<Rigidbody>();
        //    if (!collider)
        //        collider = GetComponent<Collider>();

        //    rb.useGravity = useGravity;
        //    rb.collisionDetectionMode = mode;
        //    rb.isKinematic = isKinematic;
        //    collider.isTrigger = isTrigger;
        //}

    }
}