using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Musashi
{
    /// <summary>
    /// 1frame前の位置から、現在の位置までにRayを飛ばして当たり判定をとる。
    /// リファクタリングメモ：bulletControl ⇒ ProjectileControlにまとめる
    /// </summary>
    //public class BulletControl : MonoBehaviour
    //{
    //    [SerializeField] int lifeTime = 1;

    //    float shotDamage;
    //    float shotPower;

    //    bool init = true;

    //    HitVFXManager hitVFXManager;
    //    Rigidbody rb;
    //    Vector3 prevPos;

    //    private void OnEnable()
    //    {
    //        if (rb == null)
    //        {
    //            rb = GetComponent<Rigidbody>();
    //        }

    //        prevPos = transform.position;
    //        rb.velocity = transform.forward * shotPower;

    //        if (init == false)
    //        {
    //            gameObject.SetActive(false, lifeTime);
    //        }
    //        init = false;
    //    }

    //    public void SetInfo(float shotPower, float shotDamage, HitVFXManager hitVFXManager)
    //    {
    //        this.shotDamage = shotDamage;
    //        this.shotPower = shotPower;
    //        this.hitVFXManager = hitVFXManager;
    //    }

    //    private void Update()
    //    {
    //        //hit check; 修正どころ：
    //        RaycastHit[] hits = Physics.RaycastAll(new Ray(prevPos, (transform.position - prevPos).normalized), (transform.position - prevPos).magnitude);

    //        for (int i = 0; i < hits.Length;)
    //        {
    //            if (hits[i].collider.TryGetComponent(out IDamageable target))
    //            {
    //                var type = target.GetTargetType();
    //                if (type != TargetType.Defult)
    //                {
    //                    hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.SelectVFX(type), hits[i].point, Quaternion.LookRotation(hits[i].normal), hitVFXManager.SetPoolObj);
    //                }
    //                target.OnDamage(shotDamage);
    //            }
    //            else
    //            {
    //                hitVFXManager.PoolObjectManager.UsePoolObject(hitVFXManager.DecalVFX, hits[i].point + hits[i].normal * 0.01f, Quaternion.LookRotation(hits[i].normal * -1f), hitVFXManager.SetPoolObj);
    //                hitVFXManager.AudioSource.Play(hitVFXManager.HitSFX);
    //            }
    //            gameObject.SetActive(false);
    //            break;
    //        }
    //        prevPos = transform.position;
    //    }
    //}
}
