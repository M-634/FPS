using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Musashi.Player;

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
                        Debug.LogWarning($"{savePoints[trigger.GetSpwanIndex].name}��{trigger.name}�������C���f�b�N�X���w�肵�Ă��܂��B");
                    }
                }
            }
            SpwanPlayer();
        }

        /// <summary>
        /// �v���C���[��SavePoint��Collider�ƐڐG������Ă΂��֐�
        /// </summary>
        /// <param name="index"></param>
        public void UpdateSavepoint(int index)
        {
            currentSavePoint = savePoints[index].GetSpwan;
            Debug.Log("save point���X�V���܂����B");
        }

        /// <summary>
        /// �v���C���[�����S������ɌĂ΂��֐�
        /// </summary>
        public void SpwanPlayer()
        {
            Instantiate(playerPrefab, currentSavePoint.position, Quaternion.identity);
        }
    }
}
