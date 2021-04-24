using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.5f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if(timer > lifeTime)
        {
            timer = 0f;
            gameObject.SetActive(false);
           
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {
        
    }

}
