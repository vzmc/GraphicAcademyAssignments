using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProjectSimulation : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private float _projectingTime = 1.0f;
    [SerializeField] private bool _showProjected = false;

    private Mesh _mesh;
    private float _timer = 0f;
    private bool _isStop = false;

    readonly int[] indices = new int[]
    {
        0, 1, 2, 3,
        5, 4, 7, 6,
        4, 0, 3, 7,
        1, 5, 6, 2,
        0, 4, 5, 1,
        2, 6, 7, 3
    };

    private void Start()
    {
        _mesh = new Mesh();
        _mesh.SetVertices(new Vector3[8]);
        _mesh.SetIndices(indices, MeshTopology.Quads, 0);
        GetComponent<MeshFilter>().mesh = _mesh;
    }

    private void Update()
    {
        UpdateTimer(_showProjected);

        if (_isStop)
        { 
            return;
        }

        // 投影前の視錐台頂点作る
        var vertices = CreateFrustumVertices(_camera.nearClipPlane, _camera.farClipPlane, _camera.fieldOfView, _camera.aspect);
        // 各頂点座標を投影させて、投影前の座標と線形補間する
        var interpolationVertices = vertices.Select(v => (Vector3)Vector4.Lerp(v, ProjectVertex(v), _timer / _projectingTime)).ToArray();
        _mesh.SetVertices(interpolationVertices);
        _mesh.RecalculateBounds();
    }

    private void UpdateTimer(bool forward)
    {
        var direction = forward ? 1 : -1;
        if (forward && _timer > _projectingTime
            || !forward && _timer < 0)
        {
            _isStop = true;
        }
        else
        {
            _isStop = false;
            _timer += Time.deltaTime * direction;
        }
    }

    // 視錐台の8つの頂点を作る
    private Vector4[] CreateFrustumVertices(float near, float far, float fov, float aspect)
    {
        var nearHeight = 2.0f * near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        var nearWidth = nearHeight * aspect;
        var farHeight = 2.0f * far * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
        var farWidth = farHeight * aspect;
        
        return new Vector4[]
        {
            new Vector4(nearWidth * -0.5f, nearHeight * 0.5f, near, 1.0f),
            new Vector4(nearWidth * 0.5f, nearHeight * 0.5f, near, 1.0f),
            new Vector4(nearWidth * 0.5f, nearHeight * -0.5f, near, 1.0f),
            new Vector4(nearWidth * -0.5f, nearHeight * -0.5f, near, 1.0f),
            new Vector4(farWidth * -0.5f, farHeight * 0.5f, far, 1.0f),
            new Vector4(farWidth * 0.5f, farHeight * 0.5f, far, 1.0f),
            new Vector4(farWidth * 0.5f, farHeight * -0.5f, far, 1.0f),
            new Vector4(farWidth * -0.5f, farHeight * -0.5f, far, 1.0f)
        };
    }

    // 投影変換
    private Vector4 ProjectVertex(Vector4 vertex)
    {
        // 投影行列をカメラから取得
        var pMatrix = _camera.projectionMatrix;
        // Unityのカメラ空間は右手座標系なので、
        // Camera.projectionMatrixをかける前にZ軸方法を反転する必要がある
        vertex.z *= -1;
        // 投影行列適用
        vertex = pMatrix * vertex;
        // w除算
        vertex /= vertex.w;

        return vertex;
    }
 }
