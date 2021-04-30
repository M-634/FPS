using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// インタラクティブなオブジェットが継承するインターファイス
    /// </summary>
    public interface IInteractable
    {
        void Excute(GameObject player);
    }
}