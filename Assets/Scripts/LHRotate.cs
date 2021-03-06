using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class LHRotate : MonoBehaviour
{
    [SerializeField] GameObject LHObject;
    [SerializeField] float duration = 0.1f;

    // Start is called before the first frame update
    void Start()
    {
        LHObject.transform.DOLocalRotate(new Vector3(90, 0,  -1 *360f), duration,RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1,LoopType.Restart);
    }
}
