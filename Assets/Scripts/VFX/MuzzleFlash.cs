using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.5f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

}
