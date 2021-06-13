using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// プレイヤーとインタラクティブなオブジェットが継承するインターファイス
    /// </summary>
    public interface IInteractable
    {
        void Excute(Transform player);
    }
}