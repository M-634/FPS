using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.VFX
{
    /// <summary>
    /// オブジェットプールを適用する、デカールを制御するクラス
    /// </summary>
    public class VFXDecalControl : MonoBehaviour
    {
        [SerializeField] float lifeTime = 1;
        bool init = true;

        private void OnEnable()
        {
            if (init == false)
            {
                gameObject.DelaySetActive(false, lifeTime, this.GetCancellationTokenOnDestroy());
            }
            init = false;
        }
    }
}
