using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCreator : MonoBehaviour
{
    [SerializeField] MeshFilter mf;
    // Mesh m;

    void Awake() {
        Mesh m = new Mesh();
        mf.mesh = m;

        Vector3[] vertices = new Vector3[] {new Vector3(0, 1, 0), new Vector3(-1, 0, 0), new Vector3(1, 0, 0)};
        int[] triangles = new int[] {0, 1, 2, 1, 0, 2};

        m.vertices = vertices;
        m.triangles = triangles;
    }
}
