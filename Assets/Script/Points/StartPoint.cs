using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class StartPoint : MonoBehaviour
{
    public GameObject MeshObj { get; set; }
    public float Size { get; set; }
    public Vector3 Offset { get; set; }
    public Vector3[] IntersectionPoints { get; set; }
    public Vector3[] IntersectionPointSeconds { get; set; }

    public Vector3 IntersectionNegativePoint { get; set; }
    public Vector3 IntersectionPositivePoint { get; set; }

    public Vector3 NegativeFinishPoint => FinishPoint.transform.position - Offset;
    public Vector3 PositiveFinishPoint => FinishPoint.transform.position + Offset;
    public Vector3 PositiveStartPoint => transform.position + Offset;
    public Vector3 NegativeStartPoint => transform.position - Offset;

    public FinishPoint FinishPoint { get; set; }

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;

    public void Init(FinishPoint finishPoint)
    {
        FinishPoint = finishPoint;
        IntersectionNegativePoint = NegativeFinishPoint;
        IntersectionPositivePoint = PositiveFinishPoint;

        Size = Random.Range(0.1f, 1f);

        if (MeshObj == null)
        {
            MeshObjInit();
        }
    }

    public void UpdateMesh(LinkedList<Vector3> vertices)
    {
        meshFilter.mesh.vertices = vertices.ToArray();

        int[] triangles = new int[9];

        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 3;
        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 0;
        triangles[6] = 2;
        triangles[7] = 3;
        triangles[8] = 4;

        meshFilter.mesh.triangles = triangles;
    }

    public void OffsetCalc()
    {
        Vector3 offset = Vector3.right;

        if (transform.position.y < FinishPoint.transform.position.y)
        {
            offset = Vector3.left;
        }

        Vector3 mainVectorNormalized = FinishPoint.transform.position - transform.position;

        Vector3.OrthoNormalize(ref mainVectorNormalized, ref offset);

        Offset = offset * Size;
    }

    private void MeshObjInit()
    {
        MeshObj = new GameObject("LineMesh");
        meshFilter = MeshObj.AddComponent<MeshFilter>();
        meshRenderer = MeshObj.AddComponent<MeshRenderer>();

        meshRenderer.material = RandomizeMaterial();
    }

    private Material RandomizeMaterial()
    {
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = new Color(Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), Random.Range(0f, 1.0f), 1);

        return material;
    }
}
