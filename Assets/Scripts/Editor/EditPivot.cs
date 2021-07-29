using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
#if UNITY_EDITOR

/// <summary>
/// ピボットの位置を修正するEditor拡張機能。
/// </summary>
public class EditPivot
{
    const string newName = "Mesh";

    [MenuItem("GameObject/EditPivot", false, 0)]
    static void Excute()
    {
        foreach (var seletGameObject in Selection.gameObjects)
        {
            //重複しないようにする
            if (seletGameObject.name == newName) continue;

            Vector3 pivotPos;
            GameObject emptyObj = new GameObject(seletGameObject.name);
            seletGameObject.name = newName;

            //BoxColliderの中心座標をピボットの位置にする
            if (!seletGameObject.TryGetComponent(out BoxCollider boxCollider))
            {
                boxCollider = seletGameObject.AddComponent<BoxCollider>();
            }

            pivotPos = seletGameObject.transform.TransformPoint(boxCollider.center);
            GameObject.DestroyImmediate(boxCollider);

            //座標をセット
            emptyObj.transform.position = pivotPos;
            emptyObj.transform.rotation = seletGameObject.transform.rotation;

            //元のモデルを子オブジェクトにする
            if (seletGameObject.transform.parent)
            {
                emptyObj.transform.SetParent(seletGameObject.transform.parent);
            }
            seletGameObject.transform.SetParent(emptyObj.transform);
        }
    }
}
#endif // UNITY_EDITOR
