using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Musashi
{
    /// <summary>
    /// オブジェットプールを適用するGameObjectをまとめて制御するクラス
    /// </summary>
    public class PoolObjectManager
    {
        /// <summary>
        /// プールしたいゲームオブジェットをまとめて1つのオブジェットとして扱うクラス
        /// </summary>
        public class PoolObject
        {
            public List<GameObject> poolObjects;

            public PoolObject()
            {
                poolObjects = new List<GameObject>();
            }

            public bool ActiveSlef
            {
                get
                {
                    foreach (var item in poolObjects)
                    {
                        if (item.activeSelf) return true;
                    }
                    return false;
                }
            }


            public void AddObj(GameObject obj)
            {
                poolObjects.Add(obj);
            }

            public void SetActiveAll(bool value)
            {
                foreach (var item in poolObjects)
                {
                    item.SetActive(value);
                }
            }

            public void SetActive(GameObject obj, bool value)
            {
                foreach (var item in poolObjects)
                {      
                    if (item.name == obj.name + "(Clone)")
                    {
                        item.SetActive(value);
                        return;
                    }
                }
            }

            public void SetPositionAndRotation(Vector3 pos, Quaternion quaternion)
            {
                foreach (var item in poolObjects)
                {
                    item.transform.position = pos;
                    item.transform.rotation = quaternion;
                }
            }

        }

        public List<PoolObject> poolTable;

        /// <summary>
        /// オブジェットプールのテーブルを初期化
        /// </summary>
        public PoolObjectManager()
        {
            poolTable = new List<PoolObject>();
        }


        /// <summary>
        /// 新しいプールオブジェットを生成し、返す。
        /// プールテーブルに追加する。  
        /// 呼び出し元で、プールする分だけ呼び出す。
        /// </summary>
        /// <returns></returns>
        public PoolObject InstantiatePoolObj()
        {
            var poolObject = new PoolObject();
            poolTable.Add(poolObject);
            return poolObject;
        }


        /// <summary>
        /// プールオブジェットを全て使用する
        /// </summary>
        public void UsePoolObject(Vector3 pos, Quaternion quaternion, Func<PoolObject> func = null)
        {
            foreach (var item in poolTable)
            {
                if (item.ActiveSlef) continue;

                item.SetPositionAndRotation(pos, quaternion);
                item.SetActiveAll(true);
                return;
            }

            if (func == null) return;

            //プールオブジェットの全てのアクティブがtrueなら
            //新しいプールオブジェットを生成する
            var newPoolObj = func.Invoke();
            newPoolObj.SetPositionAndRotation(pos, quaternion);
            newPoolObj.SetActiveAll(true);
        }

        /// <summary>
        /// 指定したゲームオブジェットを使用する
        /// </summary>
        public void UsePoolObject(GameObject gameObject, Vector3 pos, Quaternion quaternion, Func<PoolObject> func = null)
        {
            foreach (var item in poolTable)
            {
                if (item.ActiveSlef) continue;

                item.SetPositionAndRotation(pos, quaternion);
                item.SetActive(gameObject, true);
                return;
            }

            if (func == null) return;

            //プールオブジェットの全てのアクティブがtrueなら
            //新しいプールオブジェットを生成する
            var newPoolObj = func.Invoke();
            newPoolObj.SetPositionAndRotation(pos, quaternion);
            newPoolObj.SetActive(gameObject, true);
        }
    }
}