using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class HealItem : Item
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        protected override void Start()
        {
            base.Start();
            OnUseEvent += Heal;
        }

        public void Heal(GameObject player)
        {
            if (player.TryGetComponent(out PlayerHealthControl healthControl))
            {
                if (healthControl.enabled)
                {
                    if (healthControl.IsMaxHP)
                    {
                        InteractiveMessage.WarningMessage(InteractiveMessage.HPISFull);
                    }
                    else
                    {
                        healthControl.Heal(healPoint, healtime);//回復時間を実装する
                        CanUseItem = true;
                        return;
                    }
                }
                else
                {
                    Debug.LogWarning($"PlayerHealthControlコンポーネントのアクティブがoffになっています");
                }
            }
            else
            {
                Debug.LogWarning($"PlayerHealthControlコンポーネントがPlayerにアタッチされていません");
            }
            CanUseItem = false;
        }
    }
}
