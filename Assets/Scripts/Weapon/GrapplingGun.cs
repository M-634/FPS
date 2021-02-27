using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class GrapplingGun : MonoBehaviour
    {
        [Tooltip("PC: 0 = right, 1 = left")]
        [SerializeField] Transform[] muzzles;
        [SerializeField] Transform  debugPointer,crossHair;

        [SerializeField] LayerMask grappleableLayer;
        [SerializeField] float maxGrappleLength = 100f;

        Vector3 currentGrapplePos;
        LineRenderer lineRenderer;

        public Vector3 GetGrapplePoint { get; private set; }

        private bool isGrappling;
        public bool IsGrappling
        {
            get => isGrappling;
            set
            {
                isGrappling = value;
                if (value == false)
                {
                    lineRenderer.enabled = false;
                    canDrawRope = false;
                    muzzlesIndex = (muzzlesIndex + 1) % muzzles.Length;
                }
            }
        }
        bool canDrawRope;
        int muzzlesIndex;

        private void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }  

        private void Update()
        {
            //test
            if (PlayerInputManager.HasPutGrapplingGunButton())
            {
                HandleHookshotStart();
            }

            if (canDrawRope) DrawRope();
        }

        /// <summary>
        /// VR:左右それぞれの銃口からRayを飛ばし、任意の方でグラップルできる
        /// FPS：Crosshairから飛ばして、左右順番にグラップルする
        /// </summary>
        private void HandleHookshotStart()
        {
            if (IsGrappling) return;

            var ray = Camera.main.ScreenPointToRay(crossHair.position);
            if (Physics.Raycast(ray, out RaycastHit hit, maxGrappleLength, grappleableLayer)) 
            {
                if(debugPointer)
                    debugPointer.position = hit.point;
                GetGrapplePoint = hit.point;
                currentGrapplePos = muzzles[muzzlesIndex].position;
                lineRenderer.enabled = true;
                canDrawRope = true;
            }
        }

        /// <summary>
        /// ロープがフックしたらPlayerをフックした場所まで移動させる
        /// </summary>
        private void DrawRope() 
        {
            currentGrapplePos = Vector3.Lerp(currentGrapplePos, GetGrapplePoint, Time.deltaTime * 8f);
            
            lineRenderer.SetPosition(0, muzzles[muzzlesIndex].position);
            lineRenderer.SetPosition(1, currentGrapplePos);

            if(Vector3.Distance(currentGrapplePos,GetGrapplePoint) < 1f)
            {
                IsGrappling = true;
            }
        }
    }
}