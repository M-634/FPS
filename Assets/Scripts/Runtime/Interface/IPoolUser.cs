using UnityEngine;
namespace Musashi
{
    public interface IPoolUser<T> where T :MonoBehaviour 
    {
       /// <summary>
       /// 必要な分だけオブジェットプールを生成する関数
       /// </summary>
       void  InitializePoolObject(int poolSize = 1);

        /// <summary>
        /// プールさせたいゲームオブジェットをインスタンスして、プールオブジェットにセットする。
        /// そうしてセットされたプールオブジェットを返す関数
        /// </summary>
        /// <returns></returns>
        PoolObjectManager.PoolObject SetPoolObj();
    }
}
