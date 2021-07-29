using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Musashi
{
    public static class AudioSorceExtensions
    {
        /// <summary>
        /// Nullチェックと音量調整をして音を再生する拡張メソッド
        /// </summary>
        public static void Play(this AudioSource audioSource, AudioClip audioClip = null, float volume = 1f)
        {
            if (audioClip)
            {
                if (audioSource == null)
                {
                    Debug.LogError("NUllなんだが");
                }
                audioSource.clip = audioClip;
                audioSource.pitch = 1f;
                //ボリュームが適切になるように調整する
                audioSource.volume = Mathf.Clamp01(volume);

                audioSource.Play();
            }
        }

        /// <param name="range">0 ~ 1</param>
        public static void PlayRandomPitch(this AudioSource audioSource, AudioClip audioClip = null, float volume = 1f, float range = 0.5f)
        {
            if (!audioClip) return;

            audioSource.clip = audioClip;

            //Rando pitch
            range = Mathf.Clamp(range, 0.1f, 1f);
            audioSource.pitch = Random.Range(1 - range, 1 + range);

            //ボリュームが適切になるように調整する
            audioSource.volume = Mathf.Clamp01(volume);

            audioSource.Play();
        }

        /// <summary>
        /// 再生位置をランダムにする拡張クラス
        /// </summary>
        /// <param name="audioSource"></param>
        /// <param name="audioClip"></param>
        /// <param name="volume"></param>
        public static void PlayRandomStart(this AudioSource audioSource, AudioClip audioClip, float volume = 1f)
        {
            if (!audioClip) return;

            audioSource.clip = audioClip;
            audioSource.volume = Mathf.Clamp01(volume);

            //結果がlengthと同値になるとシークエラーを起こすため -0.01秒する
            audioSource.time = Random.Range(0f, audioClip.length - 0.01f);

            audioSource.Play(audioClip);
        }

        public static void PlayWithFadeIn(this AudioSource audioSource, AudioClip audioClip = null, float fadeTime = 0.1f, float endVolume = 1.0f)
        {
            //目標ボリュームを0~1に補正
            float targetVolume = Mathf.Clamp01(endVolume);

            //フェード時間がおかしかったら補正
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;

            //音量0で再生開始
            audioSource.Play(audioClip, 0f);

            //DOTweenを使って目標ボリュームまでFade
            audioSource.DOFade(targetVolume, fadeTime);
        }

        public static void StopWithFadeOut(this AudioSource audioSource, float fadeTime = 0.1f)
        {

            //フェード時間がおかしかったら補正
            fadeTime = fadeTime < 0.1f ? 0.1f : fadeTime;

            audioSource.DOFade(0f, fadeTime);
            audioSource.Stop();
            audioSource.clip = null;
        }

    }
}