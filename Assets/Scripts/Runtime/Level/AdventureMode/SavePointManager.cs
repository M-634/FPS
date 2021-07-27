using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;
using System;
using Cysharp.Threading.Tasks;

namespace Musashi.Level.AdventureMode
{
    public class SavePointManager : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Transform initSpwanPoint;
        [SerializeField] SavePointTrigger[] savePoints;
        [SerializeField] float spwanYOffset = 0.5f;

        public const string SAVEPOINTTAG = "SavePoint";

        private Transform currentSavePoint;

        public int sumOfAmmo;
        public List<PlayerItemInventory.ItemInventoryTable> inventoryTable;
        public List<Weapon.WeaponControl> pickUpedWeaponSourcePrefabList;

        void Start()
        {
            currentSavePoint = initSpwanPoint;
            //set save point triggers 
            var getSavePoints = GameObject.FindGameObjectsWithTag(SAVEPOINTTAG);
            savePoints = new SavePointTrigger[getSavePoints.Length];
            foreach (var point in getSavePoints)
            {
                if (point.TryGetComponent(out SavePointTrigger trigger))
                {
                    if (!savePoints[trigger.GetSpwanIndex])
                    {
                        savePoints[trigger.GetSpwanIndex] = trigger;
                        trigger.OnSavePoint += UpdateSavepoint;
                    }
                    else
                    {
                        Debug.LogWarning($"{savePoints[trigger.GetSpwanIndex].name}と{trigger.name}が同じインデックスを指定しています。");
                    }
                }
            }
            SpwanPlayer(true);
        }


        /// <summary>
        /// プレイヤーがSavePointのColliderと接触したら呼ばれる関数
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSavepoint(int index)
        {
            currentSavePoint = savePoints[index].GetSpwan;
            Debug.Log("save pointを更新しました。");
        }

        private void SpwanPlayer(bool init = false)
        {
            var player = Instantiate(playerPrefab, currentSavePoint.position + new Vector3(0, spwanYOffset, 0), Quaternion.identity);
            //if (!init)
            //{
            //    var inventory = player.GetComponentInChildren<PlayerItemInventory>();
            //    inventory.SumAmmoInInventory = sumOfAmmo;
            //    inventory.AddItemFromSpwan(inventoryTable);
            //    player.GetComponentInChildren<PlayerWeaponManager>().AddWeaponFromSpwan(pickUpedWeaponSourcePrefabList);
            //}
        }

        /// <summary>
        /// プレイヤーが死亡した後に呼ばれる関数
        /// charactor controller を直接テレポートさせるとバグるので、新たにプレイヤーをインスタンスさせる。
        /// ただし；現状だと落ちたら武器とアイテム全ロスしてしまうのでどうにかしたい
        /// </summary>
        public void ReSpwan(Transform player)
        {
            //var inventory = player.GetComponentInChildren<PlayerItemInventory>();
            //sumOfAmmo = inventory.SumAmmoInInventory;
            //inventoryTable = inventory.ItemTable;
            //var weapons = player.GetComponent<PlayerWeaponManager>().WeaponSlots;
            //foreach (var w in weapons)
            //{
            //    var instance = Instantiate(w.SourcePrefab).GetComponent<Weapon.WeaponControl>();
            //    instance.SourcePrefab = w.SourcePrefab;
            //    pickUpedWeaponSourcePrefabList.Add(instance);
            //}

            Destroy(player.parent.gameObject);
            SpwanPlayer();
        }
    }
}


