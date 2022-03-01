using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshGeneratorWithColor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CubeWithColor();
    }

    void CubeWithColor()
    {
        List<Vector3> vertices = new List<Vector3>() {
            // 0
            new Vector3(0.0f, 0.0f, 0.0f), // 正面
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 0.0f),
            // 4
            new Vector3(1.0f, 1.0f, 0.0f), // 上面
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            // 8
            new Vector3(1.0f, 0.0f, 0.0f), // 右面
            new Vector3(1.0f, 1.0f, 0.0f),
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            // 12
            new Vector3(0.0f, 0.0f, 0.0f), // 左面
            new Vector3(0.0f, 1.0f, 0.0f),
            new Vector3(0.0f, 1.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            // 16
            new Vector3(0.0f, 1.0f, 1.0f), // 背面
            new Vector3(1.0f, 1.0f, 1.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 1.0f),
            // 20
            new Vector3(0.0f, 0.0f, 0.0f), // 下面
            new Vector3(1.0f, 0.0f, 0.0f),
            new Vector3(1.0f, 0.0f, 1.0f),
            new Vector3(0.0f, 0.0f, 1.0f),

        };
        List<int> triangles = new List<int> {
            0, 3, 2,  0, 2, 1, //前面 ( 0 -  3)
            5, 6, 7,  5, 7, 4, //上面 ( 4 -  7)
            8, 9,10,  8,10,11, //右面 ( 8 - 11)
           15,14,13, 15,13,12, //左面 (12 - 15)
           16,18,17, 16,19,18, //奥面 (16 - 19)
           23,20,21, 23,21,22, //下面 (20 - 23)
        };
        List<Color> colors = new List<Color>
        {
            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.grey,
            Color.magenta,
            Color.red,
            Color.yellow,

            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.grey,
            Color.magenta,
            Color.red,
            Color.yellow,

            Color.blue,
            Color.cyan,
            Color.gray,
            Color.green,
            Color.grey,
            Color.magenta,
            Color.red,
            Color.yellow,
        };
        

        Mesh mesh = new Mesh();             // メッシュを作成
        mesh.Clear();                       // メッシュ初期化
        mesh.SetVertices(vertices);         // メッシュに頂点を登録する
        mesh.SetColors(colors);             // 頂点カラーを登録
        mesh.SetTriangles(triangles, 0);    // メッシュにインデックスリストを登録する
        mesh.RecalculateNormals();          // 法線の再計算

        // 作成したメッシュをメッシュフィルターに設定する
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}
