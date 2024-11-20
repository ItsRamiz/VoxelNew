using Unity.VisualScripting;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public Camera playerCamera;
    public float maxRayDistance = 100f;

    public float x_offset;

    // Basis vectors for the Makbilit transformation
    public Vector3 vector1;
    public Vector3 vector2;
    public Vector3 vector3;

    void Start()
    {
        vector1 = World.Instance.vector1;
        vector2 = World.Instance.vector2;
        vector3 = World.Instance.vector3;

        x_offset = World.Instance.x_offset;
    }

    void Update()
    {
        HandleBlockSelection();
    }

    void HandleBlockSelection()
    {
        Vector3 voxelHitPoint;

        if (Input.GetMouseButtonDown(0))  // Left-click
        {
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {

                Vector3 hitVectorReverted = CalculateHitPoint(hit.point);

                Debug.Log("Hit Converted = " + hitVectorReverted);

                Vector3 VoxelCordsReverted = CalculateVoxelCords(hitVectorReverted);

                DeactivateVoxel(hit,VoxelCordsReverted);

            }
        }
    }
    Vector3 CalculateHitPoint(Vector3 hitPoint)
    {
        Matrix4x4 B = new Matrix4x4(
            new Vector4(vector1.x, vector1.y, vector1.z, 0),
            new Vector4(vector2.x, vector2.y, vector2.z, 0),
            new Vector4(vector3.x, vector3.y, vector3.z, 0),
            new Vector4(0, 0, 0, 1)
        );

        Matrix4x4 BInverse = B.inverse;

        Vector3 localVoxelCoords = BInverse.MultiplyPoint3x4(hitPoint);

        float layer = Mathf.Floor(localVoxelCoords.y); // Determine the layer based on Y coordinate....

        localVoxelCoords.x -= (layer * x_offset); // Subtract the X offset applied during initialization

        return new Vector3(localVoxelCoords.x, localVoxelCoords.y, localVoxelCoords.z);
    }

    Vector3 CalculateVoxelCords(Vector3 hitPoint)
    {
        const float epsilon = 1e-5f;  // Small value to adjust for precision issues...
        Vector3 result = new Vector3(
            Mathf.FloorToInt(hitPoint.x + epsilon),
            Mathf.FloorToInt(hitPoint.y + epsilon),
            Mathf.FloorToInt(hitPoint.z + epsilon)
        );
        return result;
    }
    void DeactivateVoxel(RaycastHit hit, Vector3 voxelPosition)
    {
        Chunk chunk = hit.collider.GetComponentInParent<Chunk>();
        if (chunk == null)
        {
            return;
        }

        Voxel[] voxels = chunk.GetVoxelAt(voxelPosition);

        foreach (Voxel voxel in voxels)
        {
            if (voxel == null) continue;

            Vector3[] originVertices = voxel.GetOriginVertices();
            int faceIdx = GetVoxelFaceIdx(originVertices, CalculateHitPoint(hit.point));

            if (faceIdx != -1)
            {
                if (faceIdx == 1 && x_offset != 0)
                {
                    float topFaceMidY = (originVertices[2].x + originVertices[3].x) / 2.0f;

                    Debug.Log("Big Vertice = " + originVertices[2]);
                    Debug.Log("Small Vertice = " + originVertices[3]);
                    Debug.Log("Hit = " + CalculateHitPoint(hit.point));


                    if (CalculateHitPoint(hit.point).x >= topFaceMidY)
                    {
                        Debug.Log("Upper");
                    }
                    else
                    {
                        Debug.Log("Lower");
                    }
                    chunk.SetVoxelInactive(voxel.position);
                    break;
                }

                chunk.SetVoxelInactive(voxel.position);
                break;
            }
        }

        if (voxels.Length == 1 && voxels[0] != null)
        {
            Vector3[] originVertices = voxels[0].GetOriginVertices();
            int faceIdx = GetVoxelFaceIdx(originVertices, CalculateHitPoint(hit.point));

            chunk.SetVoxelInactive(voxels[0].position);
        }
    }



    int GetVoxelFaceIdx(Vector3[] vertices, Vector3 point)
    {
        //Debug.Log("Points = " + point);

        int[][] faces = new int[][]
        {
        new int[] {0, 1, 4, 5}, // Bottom face
        new int[] {3, 2, 7, 6}, // Top face
        new int[] {4, 5, 7, 6}, // Left face
        new int[] {0, 3, 2, 1}, // Right face
        new int[] {1, 2, 5, 6}, // Front face
        new int[] {0, 3, 4, 7}  // Back face
        };

        for (int i = 0; i < faces.Length; i++)
        {
            int[] face = faces[i];

            Vector3 v0 = vertices[face[0]];
            Vector3 v1 = vertices[face[1]];
            Vector3 v2 = vertices[face[2]];
            Vector3 v3 = vertices[face[3]];

            float minX = Mathf.Min(v0.x, v1.x, v2.x, v3.x);
            float maxX = Mathf.Max(v0.x, v1.x, v2.x, v3.x);
            float minY = Mathf.Min(v0.y, v1.y, v2.y, v3.y);
            float maxY = Mathf.Max(v0.y, v1.y, v2.y, v3.y);
            float minZ = Mathf.Min(v0.z, v1.z, v2.z, v3.z);
            float maxZ = Mathf.Max(v0.z, v1.z, v2.z, v3.z);

            //Debug.Log($"Checking face {i}:");
            //Debug.Log($"Point: {point}");
            //Debug.Log($"X range: {minX} to {maxX}");
            //Debug.Log($"Y range: {minY} to {maxY}");
            //Debug.Log($"Z range: {minZ} to {maxZ}");

            const float epsilon = 0.01f; // Small margin for error

            if (point.x >= minX - epsilon && point.x <= maxX + epsilon &&
                point.y >= minY - epsilon && point.y <= maxY + epsilon &&
                point.z >= minZ - epsilon && point.z <= maxZ + epsilon)
            {
                return i;
            }
        }

        return -1;
    }
}
