using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Musashi
{
    /// <summary>
    /// 最初にnodeを１つだけ置いておく。
    ///nodeをCtr+Dで複製したら、パスのリストに加える。
    ///削除されたら、該当するnodeをリストから削除する
    /// </summary>
    public class CreatePathTool : MonoBehaviour
    {
        [SerializeField] EnemyAI owner;
        [SerializeField] Transform nodePrefab;
        [SerializeField] List<Transform> pathes;

        public List<Transform> Pathes { get; set; }

        [ExecuteAlways]
        private void OnEnable()
        {
            if (pathes.Count > 0) return;

            var node = Instantiate(nodePrefab, this.transform.position, Quaternion.identity);
            node.transform.parent = this.transform;
            pathes.Add(node);
        }

        public void DuplicateNode(Transform node)
        {
            if (pathes.Contains(node))
                return;
            pathes.Add(node);

            if (owner)
                owner.SetPatrolPoints(pathes.ToArray());
        }

        public void DeleteNode(Transform node)
        {
            if (!pathes.Contains(node))
                return;

            pathes.Remove(node);

            if (owner)
                owner.SetPatrolPoints(pathes.ToArray());
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
}
