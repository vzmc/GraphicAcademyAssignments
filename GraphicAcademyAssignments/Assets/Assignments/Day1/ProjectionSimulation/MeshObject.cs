using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshObject : MonoBehaviour
{
    private Mesh _mesh;
    private Vector4[] _vertices;

    private void Awake()
    {
        _mesh = GetComponent<MeshFilter>().mesh;
        _vertices = _mesh.vertices.Select(v => new Vector4(v.x, v.y, v.z, 1)).ToArray();
    }

    public Vector4[] GetVertices()
    {
        return _vertices;
    }

    public Matrix4x4 GetModelMatrix()
    {
        return transform.localToWorldMatrix;
    }

    public void SetVertices(Vector4[] worldVertices)
    {
        var localVertices = worldVertices.Select(v => (Vector3)(transform.worldToLocalMatrix * v)).ToArray();
        _mesh.SetVertices(localVertices);
        _mesh.RecalculateNormals();
        _mesh.RecalculateTangents();
        _mesh.RecalculateBounds();
    }
}
