using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
    public class HealItem : BaseItem
    {
        [SerializeField] float healPoint = 30f;
        [SerializeField] float healtime = 60f;

        public override void OnPicked()
        {
            base.OnPicked();
        }

        public override void UseItem()
        {
            //Playerの体力が満タンなら使用しない
            EventManeger.Instance.Excute(healtime, healPoint);
            Debug.Log("回復アイテムを使った");
            Destroy(gameObject);
        }
    }
}
