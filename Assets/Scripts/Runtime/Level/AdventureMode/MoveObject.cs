using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Musashi.Level
{
    public class MoveObject : MonoBehaviour
    {
        public enum RotateAxis { X, Y, Z }

        [SerializeField] Transform rootObject;
        [SerializeField] bool doInitRotate = false;
        [SerializeField] float rotateDuration = 1f;
        [SerializeField] RotateAxis rotateAxis;


        Tweener currentTweener;

        void Start()
        {
            if (!rootObject) rootObject = transform;

            if (doInitRotate) DoRatate();
        }

        public void DoRatate()
        {
            Vector3 endValue = transform.rotation.eulerAngles;
            switch (rotateAxis)
            {
                case RotateAxis.X:
                    endValue += Vector3.right * 360;
                    break;
                case RotateAxis.Y:
                    endValue += Vector3.up * 360;
                    break;
                case RotateAxis.Z:
                    endValue += Vector3.forward * 360;
                    break;
            }

            currentTweener = rootObject.DORotate(endValue, rotateDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);

        }

        private void OnDestroy()
        {
            if (currentTweener != null)
            {
                currentTweener.Kill();
            }
        }
    }
}
