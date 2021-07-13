using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Musashi.NPC
{
    /// <summary>
    /// NPC撃破時のアニメーション実行後,Dissolve shaderを制御するクラス
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class NPCOnDieControl : MonoBehaviour
    {
        /// <summary>やられてから画面から消えるまでの秒数</summary>
        [SerializeField] float expirationPeriodAfterDeath = 5f;
        /// <summary>dissove の具合を調整するカーブ</summary>
        [SerializeField] AnimationCurve dissolveCurve;
        /// <summary>dissove のシェーダープロパティ</summary>
        [SerializeField] string dissolvePropertyNameOfShader = "_DissolveAmount";

        Animator animator;
        SkinnedMeshRenderer meshRenderer;
        Material[] materials;

        private async void OnEnable()
        {
            if (!animator)
            {
                animator = GetComponent<Animator>();
            }
            if (!meshRenderer)
            {
                meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
                materials = meshRenderer.sharedMaterials;
            }
            SetDissolveValue(0f);

            await UniTask.WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1f, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());

            float value = 0f;

            DOTween.To(() => value, (x) => value = x, 1f, expirationPeriodAfterDeath)
                .SetEase(dissolveCurve)
                .OnUpdate(() =>
                {
                    SetDissolveValue(value);
                })
                .OnComplete(() => this.gameObject.SetActive(false));
        }


        private void SetDissolveValue(float value)
        {
            foreach (var material in materials)
            {
                if (material.HasProperty(dissolvePropertyNameOfShader))
                {
                    material.SetFloat(dissolvePropertyNameOfShader, value);
                }
            }
        }
    }
}
