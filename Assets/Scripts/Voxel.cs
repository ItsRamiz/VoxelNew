using UnityEngine;
using UnityEngine.Rendering;


public class Voxel
{
    public Vector3 position;
    public bool isActive;

    Vector3 vector1 = World.Instance.vector1;
    Vector3 vector2 = World.Instance.vector2;
    Vector3 vector3 = World.Instance.vector3;

    public Vector3[] originVertices = new Vector3[8];

    public Voxel(Vector3 position, bool isActive = true)
    {
        this.position = position;
        this.isActive = isActive;
    }

    Vector3[] cubeVertices = new Vector3[]
{
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 1),
            new Vector3(0, 1, 1),
    };

    public void SetOriginVertices(Vector3[] vertices)
    {
        this.originVertices = vertices;
    }
    public Vector3[] GetOriginVertices()
    {
        return this.originVertices;
    }
    public void PrintOriginVertices()
    {
        for (int i = 0; i < originVertices.Length; i++)
        {
            Debug.Log($"Vertex {i}: {originVertices[i]}");
        }
    }

    /*
    public void CreateCube()
    {

        int[] cubeTriangles = new int[]
        {
            0, 2, 1, 0, 3, 2, // Bottom
            4, 5, 6, 4, 6, 7, // Top
            0, 1, 5, 0, 5, 4, // Front
            2, 3, 7, 2, 7, 6, // Back
            0, 4, 7, 0, 7, 3, // Left
            1, 2, 6, 1, 6, 5  // Right
        };

        Matrix4x4 transformationMatrix = new Matrix4x4(
            new Vector4(vector1.x, vector1.y, vector1.z, 0),
            new Vector4(vector2.x, vector2.y, vector2.z, 0),
            new Vector4(vector3.x, vector3.y, vector3.z, 0),
            new Vector4(0, 0, 0, 1)
        );

        Vector3[] makbilitVertices = new Vector3[cubeVertices.Length];
        for (int i = 0; i < cubeVertices.Length; i++)
        {
            makbilitVertices[i] = transformationMatrix.MultiplyPoint3x4(cubeVertices[i]);
        }

    }
    */


}
