using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// プレイヤーとインタラクティブなオブジェット(アイテム、ギミック)が継承するインターファイス
    /// </summary>
    public interface IInteractable
    {
        void Excute(Transform player);
    }
}