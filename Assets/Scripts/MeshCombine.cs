using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*************************************************
 * マテリアルが共通のオブジェットしか今のところ出来ない。
 * 今後、マテリアルが別々のものでも出来るようにしたい
 ************************************************/

/// <summary>
/// メッシュを結合させたいオブジェットの親に空のオブジェットを生成し、このスクリプトをアタッチする。
/// 結合後のマテリアルをセットして、Applyボタンを押すと、実行される
/// </summary>

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[ExecuteInEditMode]
public class MeshCombine : MonoBehaviour
{
    /// <summary>
    /// マテリアルをセットする
    /// </summary>
    [SerializeField] Material setMaterial;

    /// <summary>
    /// インスペクター上のApplyボタンを押すと実行される
    /// </summary>
    public void ExcuteCombineMesh()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        for (int i = 1; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            DestroyImmediate(meshFilters[i].gameObject);
        }

        //transform.GetComponent<MeshFilter>().mesh = new Mesh();
        //transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        transform.GetComponent<MeshFilter>().sharedMesh.CombineMeshes(combine);

        transform.GetComponent<MeshRenderer>().material = setMaterial;

        DestroyImmediate(this);
    }
}
