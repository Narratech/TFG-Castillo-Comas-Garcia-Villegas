using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ProceduralGrassRenderer : MonoBehaviour
{
    [Tooltip("A mesh to create grass from. A blade sprouts from the center of every triangle")]
    [SerializeField] 
    private Mesh sourceMesh = default;

    [Tooltip("El compute shader que crea la geometria")]
    [SerializeField]
    private ComputeShader grassComputeShader = default;

    [Tooltip("El material para renderizar el mesh")]
    [SerializeField] 
    private Material material = default;

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct SourceVertex
    {
        public Vector3 position;
    }

    private bool initialized;
    private ComputeBuffer sourceVertBuffer;
    private ComputeBuffer sourceTriBuffer;
    private ComputeBuffer drawBuffer;
    private ComputeBuffer argsBuffer;
    private int idGrassKernel;
    private int dispatchSize;
    private Bounds localBounds;


    private const int SOURCE_VERT_STRIDE = sizeof(float) * 3;
    private const int SOURCE_TRI_STRIDE = sizeof(int);
    private const int DRAW_STRIDE = sizeof(float) * (3 + (3 + 1) * 3);
    private const int INDIRECT_ARGS_STRIDE = sizeof(int) * 4;

    private int[] argsBufferReset = new int[] { 0, 1, 0, 0 };

    private void OnEnable()
    {
        Debug.Assert(grassComputeShader != null, "The grass compute shader is null", gameObject);
        Debug.Assert(material != null, "The material is null", gameObject);

        if (initialized)
            OnDisable();

        initialized = true;

        Vector3[] positions = sourceMesh.vertices;
        int[] tris = sourceMesh.triangles;

        SourceVertex[] vertices = new SourceVertex[positions.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new SourceVertex()
            {
                position = positions[i]
            };
        }
        int numSourceTriangles = tris.Length / 3;

        sourceVertBuffer = new ComputeBuffer(vertices.Length, SOURCE_VERT_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceVertBuffer.SetData(vertices);
        sourceTriBuffer = new ComputeBuffer(tris.Length, SOURCE_TRI_STRIDE, ComputeBufferType.Structured, ComputeBufferMode.Immutable);
        sourceTriBuffer.SetData(tris);
        drawBuffer = new ComputeBuffer(numSourceTriangles, DRAW_STRIDE, ComputeBufferType.Append);
        drawBuffer.SetCounterValue(0);
        argsBuffer = new ComputeBuffer(1, INDIRECT_ARGS_STRIDE, ComputeBufferType.IndirectArguments);

        idGrassKernel = grassComputeShader.FindKernel("Main");
        grassComputeShader.SetBuffer(idGrassKernel, "_SourceVertices", sourceVertBuffer);
        grassComputeShader.SetBuffer(idGrassKernel, "_SourceTriangles", sourceTriBuffer);
        grassComputeShader.SetBuffer(idGrassKernel, "_DrawTriangles", drawBuffer);
        grassComputeShader.SetBuffer(idGrassKernel, "_IndirectArgsBuffer", argsBuffer);
        grassComputeShader.SetInt("_NumSourceTriangles", numSourceTriangles);

        material.SetBuffer("_DrawTriangles", drawBuffer);

        grassComputeShader.GetKernelThreadGroupSizes(idGrassKernel, out uint threadGroupSize, out _, out _);
        dispatchSize = Mathf.CeilToInt((float)numSourceTriangles / threadGroupSize);

        localBounds = sourceMesh.bounds;
        localBounds.Expand(1);
    }

    private void OnDisable()
    {
        if (initialized)
        {
            sourceVertBuffer.Release();
            sourceTriBuffer.Release();
            drawBuffer.Release();
            argsBuffer.Release();
        }
        initialized = false;
    }

    private void LateUpdate()
    {
        if (Application.isPlaying == false)
        {
            OnDisable();
            OnEnable();
        }

        drawBuffer.SetCounterValue(0);
        argsBuffer.SetData(argsBufferReset);

        Bounds bounds = TransformBoundss(transform, localBounds);

        grassComputeShader.SetMatrix("_LocalToWorld", transform.localToWorldMatrix);

        grassComputeShader.Dispatch(idGrassKernel, dispatchSize, 1, 1);

        Graphics.DrawProceduralIndirect(material, bounds, MeshTopology.Triangles, argsBuffer, 0,
            null, null, ShadowCastingMode.Off, true, gameObject.layer);
    }


    public static Bounds TransformBoundss(Transform _transform, Bounds _localBounds)
    {
        var center = _transform.TransformPoint(_localBounds.center);

        // transform the local extents' axes
        var extents = _localBounds.extents;
        var axisX = _transform.TransformVector(extents.x, 0, 0);
        var axisY = _transform.TransformVector(0, extents.y, 0);
        var axisZ = _transform.TransformVector(0, 0, extents.z);

        // sum their absolute value to get the world extents
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);

        return new Bounds { center = center, extents = extents };
    }
}
