using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Musashi.Tool
{
#if UNITY_EDITOR
    /// <summary>
    /// 最初にnodeを１つだけ置いておく。
    ///nodeをCtr+Dで複製したら、パスのリストに加える。
    ///削除されたら、該当するnodeをリストから削除する
    /// </summary>
    [ExecuteAlways]
    public class CreatePathTool : MonoBehaviour
    {
        [SerializeField] List<Transform> pathes;//Bug :時々、pathesのNode順が勝手に変わってしまう。
        [SerializeField] AddORDeletNodeEvent addORDeletNodeEvent = default;

        private void Reset()
        {
            pathes = new List<Transform>();
            var initNode = new GameObject("Node");
            initNode.transform.position = this.transform.position;
            initNode.transform.parent = this.transform;
            initNode.AddComponent<PathNode>();
            DuplicateNode(initNode.transform);
        }

        public void DuplicateNode(Transform node)
        {
            if (pathes.Contains(node))
            {
                return;
            }

            pathes.Add(node);

            ExcuteAddORDeleteNodeEvent();
        }

        public void DeleteNode(Transform node)
        {
            if (!pathes.Contains(node))
            {
                return;
            }

            pathes.Remove(node);

            ExcuteAddORDeleteNodeEvent();
        }

        private void ExcuteAddORDeleteNodeEvent()
        {
            if (addORDeletNodeEvent != null)
            {
                addORDeletNodeEvent.Invoke(pathes.ToArray());
            }
        }

        void OnDrawGizmos()
        {
            //null check
            for (int i = 0; i < pathes.Count; i++)
            {
                if (pathes[i] == null)
                    pathes.RemoveAt(i);
            }

            if (pathes.Count < 1) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < pathes.Count; i++)
            {
                int nextIndex = (i + 1) % pathes.Count;

                Gizmos.DrawLine(pathes[i].position, pathes[nextIndex].position);
                Gizmos.DrawSphere(pathes[i].position, 0.1f);
            }
        }
    }
#endif

    /// <summary>
    /// nodeの追加、削除時のイベント。Inspectorに表示できるよう
    /// UnityEvnentを継承してラップする
    /// </summary>
    [System.Serializable]
    public class AddORDeletNodeEvent : UnityEvent<Transform[]> { }
}
