using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace Musashi.NPC
{
    /// <summary>
    /// NPC���j���̃A�j���[�V�������s��,Dissolve shader�𐧌䂷��N���X
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public class NPCOnDieControl : MonoBehaviour
    {
        /// <summary>����Ă����ʂ��������܂ł̕b��</summary>
        [SerializeField] float expirationPeriodAfterDeath = 5f;
        /// <summary>dissove �̋�𒲐�����J�[�u</summary>
        [SerializeField] AnimationCurve dissolveCurve;
        /// <summary>dissove �̃V�F�[�_�[�v���p�e�B</summary>
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
