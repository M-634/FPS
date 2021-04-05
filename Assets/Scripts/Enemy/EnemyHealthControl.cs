using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Musashi
{
    public class EnemyHealthControl : BaseHealthControl
    {

        [System.Serializable]
        public struct RenderIndexData
        {
            public Renderer renderer;
            public int materialIndex;

            public RenderIndexData(Renderer renderer, int index)
            {
                this.renderer = renderer;
                this.materialIndex = index;
            }
        }

        [SerializeField] float healthBarHightOffset;

        [Header("被ダメージ時の処理")]
        [SerializeField] Material bodyMaterial;
        [GradientUsage(true)]
        [SerializeField] Gradient onHitBodyGradient;
        [SerializeField] float flashOnHitDuration = 0.5f;

        [Header("RagDoll")]
        [SerializeField] GameObject dethEffect;


        List<RenderIndexData> bodyRenderers = new List<RenderIndexData>();
        MaterialPropertyBlock bodyFlashMaterialPropertyBlock = null;//memo: standard shaderだと反応しない。独自のshaderが必要である
        float lasitTimeDamaged = float.NegativeInfinity;
        int propertyID;

        protected override void Start()
        {
            base.Start();

            if (dethEffect)
                dethEffect.SetActive(false);

            propertyID = Shader.PropertyToID("_Color");

            //set PropertyBlock
            bodyFlashMaterialPropertyBlock = new MaterialPropertyBlock();
            //set Renderer
            var renderers = GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    if (renderer.sharedMaterials[i] == bodyMaterial)//Materialの参照を比較
                    {
                        bodyRenderers.Add(new RenderIndexData(renderer, i));
                    }
                }
            }
        }


        void Update()
        {
            //BillBord
            if (healthBarFillImage)
            {
                healthBarFillImage.transform.parent.position = transform.position + Vector3.up * healthBarHightOffset;
                healthBarFillImage.transform.parent.LookAt(Camera.main.transform.position);
            }

            //set material color

            //var currentColor = onHitBodyGradient.Evaluate((Time.time - lasitTimeDamaged) / flashOnHitDuration);
            //bodyFlashMaterialPropertyBlock.SetColor(propertyID, currentColor);
            //foreach (var data in bodyRenderers)
            //{
            //    data.renderer.SetPropertyBlock(bodyFlashMaterialPropertyBlock, data.materialIndex);
            //}
        }

        public override void OnDamage(float damage)
        {
            base.OnDamage(damage);
            lasitTimeDamaged = Time.time;
        }

        protected override void OnDie()
        {
            if (isDead) return;

            isDead = true;

            //Active ragdoll
            if (dethEffect)
            {
                dethEffect.transform.position = transform.position;
                dethEffect.SetActive(true);
            }

            if (healthBarFillImage)
                healthBarFillImage.transform.parent.gameObject.SetActive(false);
            transform.gameObject.SetActive(false);

            //EventManeger.Instance.Excute(EventType.EnemyDie);
        }
    }
}