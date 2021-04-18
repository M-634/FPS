using UnityEngine;
namespace Musashi
{
    /// <summary>
    /// インタラクティブなオブジェット
    /// </summary>
    public interface IInteractable
    {
        void Excute(GameObject player);
    }
}