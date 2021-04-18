using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class HealItem : BaseItem
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        public override bool CanUseObject(GameObject player)
        {
            if (player.TryGetComponent(out PlayerHealthControl healthControl))
            {
                if (healthControl.enabled)
                {
                    if (healthControl.IsMaxHP)
                    {
                        InteractiveMessage.WarningMessage(InteractiveMessage.HPISFull);
                        canUseItem = false;
                    }
                    else
                    {
                        canUseItem = true;
                        healthControl.Heal(healPoint, healtime);//回復時間を実装する
                    }

                }
                else
                {
                    canUseItem = false;
                    Debug.LogWarning($"PlayerHealthControlコンポーネントのアクティブがoffになっています");
                }
            }
            else
            {
                canUseItem = false;
                Debug.LogWarning($"PlayerHealthControlコンポーネントがPlayerにアタッチされていません");
            }
            Destroy(gameObject, 0.1f);
            return canUseItem;
        }
    }
}
