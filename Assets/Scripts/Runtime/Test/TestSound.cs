using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class TestSound : MonoBehaviour
{
    [SerializeField] AnimationCurve attenuationCurve;
    [SerializeField] float attenuationTime;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 1f;

        audioSource.DOFade(0f, attenuationTime).SetEase(attenuationCurve);
    }
}
