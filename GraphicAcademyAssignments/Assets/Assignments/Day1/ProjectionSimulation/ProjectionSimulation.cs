using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class ProjectionSimulation : MonoBehaviour
{
    [Header("視錐台パラメータ")]
    [SerializeField, Range(0.1f, 100f)] private float _nearClipPlane = 0.5f;
    [SerializeField, Range(0.2f, 100f)] private float _farClipPlane = 10f;
    [SerializeField, Range(5, 175)] private int _fieldOfView = 60;
    [SerializeField, Range(0.5f, 2f)] private float _aspect = 4 / 3f;

    [Header("変換スイッチ")]
    [SerializeField] private bool _showProjection = false;

    [Header("連動して変換するオブジェクト")]
    [SerializeField] private MeshObject[] _meshObjects;

    private Mesh _frustumMesh;
    private float _timer = 0f;
    private float _transformTime = 1.0f;

    private float _tmpNear;
    private float _tmpFar;
    private float _tmpFOV;
    private float _tmpAspect;
    private float _tmpTimer;
    private bool _isDirty = false;

    // 擬似ビュー行列(世界空間からローカル空間へ変換し、さらに左手座標系から右手座標系に変換する)
    public Matrix4x4 ViewMatrix => Matrix4x4.Scale(new Vector3(1, 1, -1)) * transform.worldToLocalMatrix;

    // 投影行列
    public Matrix4x4 ProjectionMatrix => Matrix4x4.Perspective(_fieldOfView, _aspect, _nearClipPlane, _farClipPlane);

    // 視錐台の面を構成する頂点の順番
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
        _frustumMesh = new Mesh();
        _frustumMesh.SetVertices(new Vector3[8]);
        _frustumMesh.SetIndices(indices, MeshTopology.Quads, 0);
        GetComponent<MeshFilter>().mesh = _frustumMesh;
    }

    private void Update()
    {
        UpdateTimer(_showProjection);
        CheckFieldChange();
        if (!_isDirty)
        { 
            return;
        }

        var t = _timer / _transformTime;

        // 投影前の視錐台頂点作る
        var vertices = CreateFrustumVertices(_nearClipPlane, _farClipPlane, _fieldOfView, _aspect);
        // 各頂点座標を投影させて、投影前の座標と線形補間する
        var interpolationVertices = vertices.Select(v => (Vector3)Vector4.Lerp(v, MVPTransform(v, Matrix4x4.identity, Matrix4x4.identity, ProjectionMatrix), t)).ToArray();
        _frustumMesh.SetVertices(interpolationVertices);
        _frustumMesh.RecalculateBounds();

        // 各オブジェクトの頂点変換
        foreach(var obj in _meshObjects)
        {
            var objVertices = obj.GetVertices();
            var mMatrix = obj.GetModelMatrix();
            obj.SetVertices(objVertices.Select(v => Vector4.Lerp(mMatrix * v, MVPTransform(v, mMatrix, ViewMatrix, ProjectionMatrix), t)).ToArray());
        }
    }

    private void CheckFieldChange()
    {
        if (_tmpNear.CompareTo(_nearClipPlane) != 0
            || _tmpFar.CompareTo(_farClipPlane) != 0
            || _tmpFOV.CompareTo(_fieldOfView) != 0
            || _tmpAspect.CompareTo(_aspect) != 0
            || _tmpTimer.CompareTo(_timer) != 0)
        {
            _isDirty = true;
            _tmpNear = _nearClipPlane;
            _tmpFar = _farClipPlane;
            _tmpFOV = _fieldOfView;
            _tmpAspect = _aspect;
            _tmpTimer = _timer;
        }
        else
        {
            _isDirty = false;
        }
    }

    private void UpdateTimer(bool forward)
    {
        var direction = forward ? 1 : -1;
        if (forward && _timer > _transformTime
            || !forward && _timer < 0)
        {
            return;
        }

        _timer += Time.deltaTime * direction;
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

    // モデル空間座標からNDC空間座標まで変換
    private Vector4 MVPTransform(Vector4 vertex, Matrix4x4 model, Matrix4x4 view, Matrix4x4 projection)
    {
        // MVP行列作る
        var mvp = projection * view * model;
        // 右手座標系から左座標系に戻す
        vertex.z *= -1;
        // MVP行列かけて
        vertex = mvp * vertex;
        // w除算でNDC空間へ変換する
        vertex /= vertex.w;

        return vertex;
    }
 }
