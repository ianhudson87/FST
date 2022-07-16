// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class TreeGenerator : MonoBehaviour
// {
//     [SerializeField] MeshFilter meshFilter;

//     void Awake() {
//         TreeGenOptions opt = new TreeGenOptions(new Vector3(0,0,0), 3, 0.8f, 10, 2);
//         PlaceTree(opt);
//     }
    
//     public void PlaceTree(TreeGenOptions opt) {
//         meshFilter.mesh = GetTreeMesh(opt, 0);
//     }
//     Mesh GetTreeMesh(TreeGenOptions opt, int vertexCount) {
//         int startingVertexCount = vertexCount;

//         Mesh treeMesh = new Mesh();

//         List<Vector3> vertices = new List<Vector3>();
//         List<int> triangles = new List<int>();

//         float verticesAngle = 2*Mathf.PI / opt.numSides;

//         for(int levelIdx = 0; levelIdx < 5; levelIdx++) {
//             // for each level of vertices create the vertices for this level
//             for(int vertexIdx=0; vertexIdx < opt.numSides; vertexIdx++) {
//                 float vertexAngle = vertexIdx * verticesAngle;
//                 float xCoord = opt.baseRadius * Mathf.Pow(opt.radiusMultiplier, levelIdx) * Mathf.Cos(vertexAngle);
//                 float yCoord = levelIdx * opt.levelHeight;
//                 float zCoord = opt.baseRadius * Mathf.Pow(opt.radiusMultiplier, levelIdx) * Mathf.Sin(vertexAngle);
//                 vertices.Add(new Vector3(xCoord, yCoord, zCoord));
//                 vertexCount++;
//             }
//         }

//         // connect all the vertices together downwards
//         for(int vertexIdx = vertexCount - 1; vertexIdx > startingVertexCount - 1 + opt.numSides; vertexIdx--) {
//             if(vertexIdx % opt.numSides == opt.numSides - 1) {
//                 // wrap around to small vertex indices
//                 triangles.Add(vertexIdx); triangles.Add(vertexIdx - opt.numSides + 1); triangles.Add(vertexIdx - 2*opt.numSides + 1); // top right
//                 triangles.Add(vertexIdx); triangles.Add(vertexIdx - 2*opt.numSides + 1); triangles.Add(vertexIdx - opt.numSides); // bottom left
//             }
//             else {
//                 // don't need to wrap
//                 triangles.Add(vertexIdx); triangles.Add(vertexIdx + 1); triangles.Add(vertexIdx + 1 - opt.numSides); // top right
//                 triangles.Add(vertexIdx); triangles.Add(vertexIdx - opt.numSides + 1); triangles.Add(vertexIdx - opt.numSides); // bottom left
//             }
//         }

//         treeMesh.vertices = vertices.ToArray();
//         treeMesh.triangles = triangles.ToArray();

//         Debug.Log("Vertices" + vertexCount);
//         foreach(Vector3 v in treeMesh.vertices)
//             Debug.Log(v);
//         foreach(int t in treeMesh.triangles)
//             Debug.Log(t);

//         return treeMesh;
//     }
// }
