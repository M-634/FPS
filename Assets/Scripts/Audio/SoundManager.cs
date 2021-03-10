using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Musashi
{
    public class SoundManager : MonoBehaviour
    {
        [SerializeField, Header("SE")]
        List<AudioClip> seAudioClipsList = new List<AudioClip>();
        private AudioSource seAudioSouce;

        [SerializeField, Header("Audio Mixer")]
        AudioMixer audioMixer;

        [SerializeField] AudioMixerGroup seAMG;


        private void Start()
        {
            seAudioSouce = InitializeAudioSource(this.gameObject, false, seAMG);    
        }

        private AudioSource InitializeAudioSource(GameObject parentGameObject, bool isLoop = false, AudioMixerGroup amg = null)
        {
            var audioSource = parentGameObject.AddComponent<AudioSource>();

            audioSource.loop = isLoop;
            audioSource.playOnAwake = false;

            if (amg)
            {
                audioSource.outputAudioMixerGroup = amg;
            }

            return audioSource;
        }

        public void PlaySE(string clipName)
        {
            var audioClip = seAudioClipsList.FirstOrDefault(clip => clip.name == clipName);

            if (audioClip == null)
            {
                Debug.LogWarning(clipName + "は見つかりません");
                return;
            }

            seAudioSouce.Play(audioClip);
        }
    }

    public static class SoundName
    {
        public static string PickUP = "PickUp";
    }
}
