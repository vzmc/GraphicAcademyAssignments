using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CubeMeshGenerator : MonoBehaviour
{
    private Mesh _mesh;

    private void Start()
    {
        //Cube1();
        //Cube2();
        Cube3();
    }

    // 8頂点だけだと、陰影がおかしくなる（頂点ことの法線方向の問題）
    private void Cube1()
    {
        var vertices = new List<Vector3>
        {
            new(0.0f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 1.0f),
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 1.0f),
            new(0.0f, 0.0f, 1.0f)
        };
        var triangles = new List<int>
        {
            0, 2, 1, 0, 3, 2, //正面
            2, 3, 5, 3, 4, 5, //上面
            1, 2, 5, 1, 5, 6, //右面
            0, 7, 4, 0, 4, 3, //左面
            5, 4, 6, 4, 7, 6, //背面
            7, 0, 1, 7, 1, 6 //下面
        };

        var mesh = new Mesh(); // メッシュを作成
        mesh.Clear(); // メッシュ初期化
        mesh.SetVertices(vertices); // メッシュに頂点を登録する
        // mesh.SetTriangles(triangles, 0);    // メッシュにインデックスリストを登録する
        mesh.SetIndices(triangles, MeshTopology.Triangles, 0);
        mesh.RecalculateNormals(); // 法線の再計算
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();

        // 作成したメッシュをメッシュフィルターに設定する
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private void Cube2()
    {
        var vertices = new List<Vector3>
        {
            // 0
            new(0.0f, 0.0f, 0.0f), // 正面
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 0.0f),
            // 4
            new(1.0f, 1.0f, 0.0f), // 上面
            new(0.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 1.0f),
            new(1.0f, 1.0f, 1.0f),
            // 8
            new(1.0f, 0.0f, 0.0f), // 右面
            new(1.0f, 1.0f, 0.0f),
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 1.0f),
            // 12
            new(0.0f, 0.0f, 0.0f), // 左面
            new(0.0f, 1.0f, 0.0f),
            new(0.0f, 1.0f, 1.0f),
            new(0.0f, 0.0f, 1.0f),
            // 16
            new(0.0f, 1.0f, 1.0f), // 背面
            new(1.0f, 1.0f, 1.0f),
            new(1.0f, 0.0f, 1.0f),
            new(0.0f, 0.0f, 1.0f),
            // 20
            new(0.0f, 0.0f, 0.0f), // 下面
            new(1.0f, 0.0f, 0.0f),
            new(1.0f, 0.0f, 1.0f),
            new(0.0f, 0.0f, 1.0f)
        };
        var triangles = new List<int>
        {
            0, 3, 2, 0, 2, 1, //前面 ( 0 -  3)
            5, 6, 7, 5, 7, 4, //上面 ( 4 -  7)
            8, 9, 10, 8, 10, 11, //右面 ( 8 - 11)
            15, 14, 13, 15, 13, 12, //左面 (12 - 15)
            16, 18, 17, 16, 19, 18, //奥面 (16 - 19)
            23, 20, 21, 23, 21, 22 //下面 (20 - 23)
        };

        var mesh = new Mesh(); // メッシュを作成
        mesh.Clear(); // メッシュ初期化
        mesh.SetVertices(vertices); // メッシュに頂点を登録する
        mesh.SetTriangles(triangles, 0); // メッシュにインデックスリストを登録する
        mesh.RecalculateNormals(); // 法線の再計算

        // 作成したメッシュをメッシュフィルターに設定する
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }

    private void Cube3()
    {
        var mesh = new Mesh();
        _mesh = mesh;

        var vertices = new List<Vector3>();
        var indices = new List<int>();

        var axis = new[] { Vector3.right, Vector3.up, Vector3.forward };
        for (var i = 0; i < 3; ++i)
        {
            var normal = axis[i];
            var binormal = axis[(i + 1) % 3];
            var tangent = Vector3.Cross(normal, binormal);

            vertices.AddRange(new[]
            {
                normal + binormal + tangent,
                normal - binormal + tangent,
                normal + binormal - tangent,
                normal - binormal - tangent,
                -normal + binormal + tangent,
                -normal - binormal + tangent,
                -normal + binormal - tangent,
                -normal - binormal - tangent
            });

            indices.AddRange(new[]
            {
                0, 1, 2,
                2, 1, 3,
                5, 4, 6,
                5, 6, 7
            }.Select(j => i * 8 + j));
        }

        mesh.SetVertices(vertices);
        mesh.SetIndices(indices.ToArray(), MeshTopology.Triangles, 0);

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.RecalculateTangents();
        var meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
    }
}