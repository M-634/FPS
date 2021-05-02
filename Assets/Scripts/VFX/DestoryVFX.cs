using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXControl : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        //Destroy(gameObject, lifeTime);
    }

    private void OnEnable()
    {
        gameObject.SetActive(false);       
    }
}
