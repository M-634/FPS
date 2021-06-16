using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkinMeshRendererToMeshRenderer : MonoBehaviour
{
    [SerializeField] SkinnedMeshRenderer[] skinMeshes;

    public void Reset()
    {
        skinMeshes = GetComponentsInChildren<SkinnedMeshRenderer>();
    }

    public void Excute()
    {
        //store mesh and material
        for (int i = 0; i < skinMeshes.Length; i++)
        {
            var meshFilter = skinMeshes[i].gameObject.AddComponent<MeshFilter>();
            var meshRenderer = skinMeshes[i].gameObject.AddComponent<MeshRenderer>();

            meshFilter.sharedMesh = skinMeshes[i].sharedMesh;
            meshRenderer.sharedMaterial = skinMeshes[i].sharedMaterial;

            DestroyImmediate(skinMeshes[i]);
        }
        DestroyImmediate(this);
    }
}
