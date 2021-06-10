using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.VFX
{
    /// <summary>
    /// VFXの中でオブジェクトプールを適用するオブジェクトにアタッチする
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class VFXParticlePoolControl : MonoBehaviour
    {
        ParticleSystem particleVFX;
        ParticleSystem.MainModule mainModule;

        bool init = true;//オブジェットプールで初期化される時は何もしない

        private void Awake()
        {
            particleVFX = GetComponent<ParticleSystem>();
            mainModule = particleVFX.main;
            mainModule.playOnAwake = false;
            mainModule.stopAction = ParticleSystemStopAction.Disable;
        }

        private void OnEnable()
        {
            if (init == false)
            {
                particleVFX.Play();
            }
            init = false;
        }
    }
}