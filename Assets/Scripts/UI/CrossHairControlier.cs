using UnityEngine;
using UnityEngine.UI;

namespace Musashi.FPS
{
    /// <summary>
    ///FPS視点の時だけ
    /// </summary>
    public class CrossHairControlier : MonoBehaviour 
    {
        [SerializeField] Transform crossHair;
        [SerializeField] int crossHairDefultSize;
        [SerializeField] int crossHairHighlightSize;
        [SerializeField] Color crossHairDefultColor;
        [SerializeField] Color crossHairHilightGrapplePointColor;
        [SerializeField] Color crossHairHilightEnemyPointColor;

        [SerializeField] LayerMask grappleableLayer;
        [SerializeField] float maxGrappleLength;

        [SerializeField] LayerMask enemyLayer;
        [SerializeField] float maxRangeDistance;

        RectTransform rectTransform;
        Image crossHairImage;


        private void Start()
        {
            rectTransform = GetComponent<RectTransform>();
            crossHairImage = GetComponent<Image>();
        }

        private void Update()
        {
            if (AimAtGrappleableObj())
            {
                rectTransform.sizeDelta = crossHairHighlightSize * Vector2.one;
                crossHairImage.color = Color.Lerp(crossHairImage.color, crossHairHilightGrapplePointColor, Time.deltaTime * 5f);
            }
            else if (AimAtEnemy()) 
            {
                rectTransform.sizeDelta = crossHairHighlightSize * Vector2.one;
                crossHairImage.color = Color.Lerp(crossHairImage.color, crossHairHilightEnemyPointColor, Time.deltaTime * 5f);
            }
            else
            {
                rectTransform.sizeDelta = crossHairDefultSize * Vector2.one;
                crossHairImage.color =  crossHairDefultColor;
            }
        }

        public bool AimAtGrappleableObj() 
        {
            var ray = Camera.main.ScreenPointToRay(crossHair.position);
            return Physics.Raycast(ray, maxGrappleLength, grappleableLayer);
        }

        public bool AimAtEnemy()
        {
            var ray = Camera.main.ScreenPointToRay(crossHair.position);
            return Physics.Raycast(ray, maxRangeDistance, enemyLayer);
        }
        
    }
}