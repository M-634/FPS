using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi
{
    /// <summary>
    /// プレイヤーに接触したら、イベントを発火するクラス
    /// (Start,Goal,Exit Trigger)
    /// </summary>
    public class Trigger : MonoBehaviour
    {
        enum TriggerType
        {
            Start, Goal, Exit
        }

        [SerializeField] TriggerType triggerType;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                switch (triggerType)
                {
                    case TriggerType.Start:
                        GameEventManager.Instance.Excute(GameEventType.StartGame);
                        break;
                    case TriggerType.Goal:
                        GameEventManager.Instance.Excute(GameEventType.Goal);
                        break;
                    case TriggerType.Exit:
                        GameManager.Instance.SceneLoder.LoadScene(SceneInBuildIndex.Title, GameManager.Instance.UnlockCusor);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
