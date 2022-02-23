using UnityEngine;

public class CoordinateTransformation : MonoBehaviour
{
    [SerializeField]
    private Camera _camera;
    [SerializeField]
    private bool _applyMatrix;

    private Mesh _mesh;
    private Vector3[] _vertices;

    private void Awake()
    {
        _mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = _mesh;
        _vertices = new Vector3[8];

        // メッシュを初期化
        var triangles = new int[]
        {
            0, 2, 1,
            1, 2, 3,
            1, 3, 5,
            7, 5, 3,
            3, 2, 7,
            6, 7, 2,
            2, 0, 6,
            4, 6, 0,
            0, 1, 4,
            5, 4, 1,
            4, 7, 6,
            5, 7, 4
        };
        var colors = new Color[]
        {
            new Color(0.0f, 0.0f, 0.0f),
            new Color(1.0f, 0.0f, 0.0f),
            new Color(0.0f, 1.0f, 0.0f),
            new Color(1.0f, 1.0f, 0.0f),
            new Color(0.0f, 0.0f, 1.0f),
            new Color(1.0f, 0.0f, 1.0f),
            new Color(0.0f, 1.0f, 1.0f),
            new Color(1.0f, 1.0f, 1.0f),
        };
        _mesh.vertices = _vertices;
        _mesh.triangles = triangles;
        _mesh.colors = colors;
        UpdateVertices();
    }

    private void Update()
    {
        UpdateVertices();
    }

    /// <summary>
    /// 頂点をカメラの視錐台に合わせたものに更新する
    /// </summary>
    private void UpdateVertices()
    {
        var near = _camera.nearClipPlane;
        var far = _camera.farClipPlane;

        // 視錐台の大きさの求め方は下記を参考
        // https://docs.unity3d.com/jp/current/Manual/FrustumSizeAtDistance.html
        var nearFrustumHeight = 2.0f * near * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var nearFrustumWidth = nearFrustumHeight * _camera.aspect;
        var farFrustumHeight = 2.0f * far * Mathf.Tan(_camera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        var farFrustomWidth = farFrustumHeight * _camera.aspect;

        _vertices[0] = new Vector3(nearFrustumWidth * -0.5f, nearFrustumHeight * -0.5f, near);
        _vertices[1] = new Vector3(nearFrustumWidth * 0.5f, nearFrustumHeight * -0.5f, near);
        _vertices[2] = new Vector3(nearFrustumWidth * -0.5f, nearFrustumHeight * 0.5f, near);
        _vertices[3] = new Vector3(nearFrustumWidth * 0.5f, nearFrustumHeight * 0.5f, near);
        _vertices[4] = new Vector3(farFrustomWidth * -0.5f, farFrustumHeight * -0.5f, far);
        _vertices[5] = new Vector3(farFrustomWidth * 0.5f, farFrustumHeight * -0.5f, far);
        _vertices[6] = new Vector3(farFrustomWidth * -0.5f, farFrustumHeight * 0.5f, far);
        _vertices[7] = new Vector3(farFrustomWidth * 0.5f, farFrustumHeight * 0.5f, far);

        if (_applyMatrix)
        {
            // VP行列を適用する
            for (int i = 0; i < _vertices.Length; i++)
            {
                // 検証のため頂点情報を4次元に
                var vertex = new Vector4(_vertices[i].x, _vertices[i].y, _vertices[i].z, 1);
                // VP行列を作成
                var mat = _camera.projectionMatrix * _camera.worldToCameraMatrix;
                // VP行列を適用
                vertex = mat * vertex;
                // W除算
                vertex /= vertex.w;

                _vertices[i] = vertex;
            }
        }

        _mesh.vertices = _vertices;
        _mesh.RecalculateBounds();
    }
}