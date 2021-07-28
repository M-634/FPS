using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi.Player
{
    /// <summary>
    /// Charactor Contollerコンポーネントは、直接Transformをいじるのが苦手でバクが起きる。
    /// その対処法として、親オブジェクトを目的地へ移動させ、その子オブジェクトであるPlayerを
    /// ローカル座標(0,0,0)と指定してあげる。
    /// </summary>
    public class PlayerTranslate : MonoBehaviour
    {
        /// <summary> プレイヤーの親オブジェクト </summary>
        [SerializeField] Transform main;
        /// <summary>charactor controller がアタッチされたプレイヤーオブジェクト</summary>
        [SerializeField] Transform player;

        /// <summary>
        /// プレイヤーを引数に指定した座標へ瞬間移動させる関数。
        /// </summary>
        /// <param name="worldpoint"></param>
        public void Translate(Transform worldpoint)
        {
            main.SetPositionAndRotation(worldpoint.position, worldpoint.rotation);
            GameManager.Instance.TimeManager.ChangeTimeScale(0f);
            player.localPosition = Vector3.zero;
            player.localEulerAngles = Vector3.zero;
            GameManager.Instance.TimeManager.ChangeTimeScale(1);
        }
    }
}
