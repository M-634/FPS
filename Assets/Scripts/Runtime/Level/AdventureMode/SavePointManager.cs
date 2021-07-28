using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AI;

namespace Musashi.Level.AdventureMode
{
    public class SavePointManager : MonoBehaviour
    {
        [SerializeField] GameObject playerPrefab;
        [SerializeField] Transform initSpwanPoint;
        [SerializeField] SavePointTrigger[] savePoints;
        [SerializeField] float spwanYOffset = 0.5f;
        [SerializeField] bool debugMode = false;
        [SerializeField] int debugSpwan;

        [SerializeField] UnityEventWrapper OnInitPlayerSpawnEvent;

        public const string SAVEPOINTTAG = "SavePoint";

        public Transform CurrentSavePoint { get; private set; }


        void Start()
        {
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
                        trigger.OnUpdateSavePoint += UpdateSavepoint;
                    }
                    else
                    {
                        Debug.LogWarning($"{savePoints[trigger.GetSpwanIndex].name}��{trigger.name}�������C���f�b�N�X���w�肵�Ă��܂��B");
                    }
                }
            }
            CurrentSavePoint = initSpwanPoint;

            if (debugMode) CurrentSavePoint = savePoints[debugSpwan].GetSpwan;
            else CurrentSavePoint = initSpwanPoint;
            //spwan player
            SpwanPlayer(true);
            //set event
            if(OnInitPlayerSpawnEvent != null)
            {
                OnInitPlayerSpawnEvent.Invoke();
            }
        }


        /// <summary>
        /// �v���C���[��SavePoint��Collider�ƐڐG������Ă΂��֐�
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSavepoint(int index)
        {
            CurrentSavePoint = savePoints[index].GetSpwan;
            Debug.Log("save point���X�V���܂����B");
        }

        private void SpwanPlayer(bool init = false)
        {
           Instantiate(playerPrefab, CurrentSavePoint.position + new Vector3(0, spwanYOffset, 0), Quaternion.identity);
        }
    }
}


