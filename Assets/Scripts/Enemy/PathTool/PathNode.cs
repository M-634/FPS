using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Musashi
{
#if UNITY_EDITOR
    [ExecuteAlways]
    public class PathNode : MonoBehaviour
    {
        CreatePathTool createPathTool;

        private void OnEnable()
        {
            if (!transform.parent) return;
            createPathTool = transform.parent.GetComponent<CreatePathTool>();
            if (!createPathTool) return;

            createPathTool.DuplicateNode(this.transform);
        }

        private void OnDisable()
        {
            if (!createPathTool) return;
            createPathTool.DeleteNode(this.transform);
        }
    }
#endif
}

