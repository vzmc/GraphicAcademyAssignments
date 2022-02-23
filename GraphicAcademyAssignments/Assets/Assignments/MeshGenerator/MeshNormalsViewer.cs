using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshNormalsViewer : MonoBehaviour
{
    [SerializeField] private float _length = 0.1f;
    
    private MeshFilter _meshFilter;

    private readonly Color[] _colors = {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow,
        Color.cyan,
        Color.magenta
    };

    private void Start()
    {
        _meshFilter = GetComponent<MeshFilter>();
    }

    private void Update()
    {
        var mesh = _meshFilter.mesh;
        if (mesh == null)
        {
            return;
        }

        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var pivot = transform.position;

        for (int i = 0; i < vertices.Length; i++)
        {
            var vertex = vertices[i];
            var normal = normals[i];
            var colorIndex = i % _colors.Length;
            Debug.DrawRay(pivot + vertex, _length * normal, _colors[colorIndex]);
        }
    }
}
