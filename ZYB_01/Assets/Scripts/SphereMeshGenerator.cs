using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class SphereMeshGenerator : MonoBehaviour
{
    [SerializeField] private float _radius = 1;
    [SerializeField, Range(2, 50)] private int _weftCount = 5;
    [SerializeField, Range(3, 50)] private int _warpCount = 10;
    
    private Mesh _mesh;
    private MeshFilter _meshFilter;

    private float _tmpRadius = 0;
    private int _tmpWeftCount = 0;
    private int _tmpWarpCount = 0;

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        // 球体の情報が変わったら再生成する
        if (_tmpRadius.CompareTo(_radius) != 0
            || _tmpWeftCount.CompareTo(_weftCount) != 0
            || _tmpWarpCount.CompareTo(_warpCount) != 0)
        {
            _tmpRadius = _radius;
            _tmpWeftCount = _weftCount;
            _tmpWarpCount = _warpCount;
            _mesh = GenerateSphereMesh(_radius, _weftCount, _warpCount);
            _meshFilter.mesh = _mesh;
        }
    }

    // 球体上の頂点位置リストを作る
    private List<Vector3> CreateSphereVerticesPositionList(float radius, int weftCount, int warpCount)
    {
        var deltaWeftAngle = Mathf.PI / weftCount;
        var deltaWarpAngle = 2 * Mathf.PI / warpCount;

        var verticesPositionList = new List<Vector3>();
        // 全頂点の位置を計算し、リストに追加する
        for (var i = 1; i < weftCount; i++)
        {
            var weftAngle = deltaWeftAngle * i;
            for (var j = 0; j < warpCount; j++)
            {
                var warpAngle = deltaWarpAngle * j;
                var x = radius * Mathf.Sin(weftAngle) * Mathf.Cos(warpAngle);
                var z = radius * Mathf.Sin(weftAngle) * Mathf.Sin(warpAngle);
                var y = radius * Mathf.Cos(weftAngle);
                verticesPositionList.Add(new Vector3(x, y, z));
            }
        }
        var topVertex = new Vector3(0, radius, 0);
        var bottomVertex = new Vector3(0, -radius, 0);
        verticesPositionList.AddRange(new List<Vector3>{topVertex, bottomVertex});

        return verticesPositionList;
    }

    // 球体メッシュを生成する
    // 頂点位置ごとに1つの頂点を置く
    private Mesh GenerateSphereMesh(float radius, int weftCount, int warpCount)
    {
        var vertices = CreateSphereVerticesPositionList(radius, weftCount, warpCount);
        var topIndex = vertices.Count - 2;
        var bottomIndex = vertices.Count - 1;
        
        var triangles = new List<int>();
        // 頂点リストから三角形の頂点の番号を三角形リストに追加する
        for (var i = 0; i < weftCount - 1; i++)
        {
            int start = i * warpCount;
            int end = start + warpCount;
            for (var j = start; j < end; j++)
            {
                if (i < weftCount - 2)
                {
                    int one = j;
                    int two = one < end - 1 ? one + 1 : start;
                    int three = two + warpCount;
                    int four = one + warpCount;
                    triangles.AddRange(new List<int>{ one, two, three });
                    triangles.AddRange(new List<int>{ one, three, four });
                }
                if (i == 0)
                {
                    int one = topIndex;
                    int two = j;
                    int three = two < end - 1 ? two + 1 : start;
                    triangles.AddRange(new List<int>{ one, three, two });
                }
                if (i == weftCount - 2)
                {
                    int one = bottomIndex;
                    int two = j;
                    int three = two < end - 1 ? two + 1 : start;
                    triangles.AddRange(new List<int>{ one, two, three });
                }
            }
        }

        return CreateMesh(vertices, triangles);
    }

    // メッシュ作る
    private Mesh CreateMesh(List<Vector3> vertices, List<int> triangles)
    {
        var mesh = new Mesh();
        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    // 生成されたメッシュを保存する
    [ContextMenu("SaveMesh")]
    private void SaveMesh()
    {
        if (_mesh == null)
        {
            return;
        }

        var assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Meshes/sphereMesh.asset");
        var mesh = Instantiate(_mesh);
        AssetDatabase.CreateAsset(mesh, assetPath);
        AssetDatabase.SaveAssets();
    }
}
