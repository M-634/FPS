using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


[ExecuteInEditMode]
public class TestExcludeOutlineEdes : MonoBehaviour
{
    List<Vector3> verticesList;

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Optimize();

        var vertices = mesh.vertices;
        verticesList = vertices.ToList();
        verticesList = verticesList.Distinct().ToList();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var vertex in verticesList)
        {
            Gizmos.DrawSphere(vertex, 0.1f);
        }
    }
}
