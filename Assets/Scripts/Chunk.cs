    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.LightTransport;



    public class Chunk : MonoBehaviour
    {
        public Vector3 vector1;
        public Vector3 vector2;
        public Vector3 vector3;

        private List<Vector3> vertices = new List<Vector3>();
        private List<int> triangles = new List<int>();
        private List<Vector2> uvs = new List<Vector2>();

        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;
        private MeshCollider meshCollider;

        public int chunkSize;
        public Voxel[,,] voxels;
        public Vector3 chunkOrigin;
        public Material VoxelMaterial;

        public bool Startup = true;

        public Vector3 o_vector1 = new Vector3(1, 0, 0);
        public Vector3 o_vector2 = new Vector3(0, 1, 0);
        public Vector3 o_vector3 = new Vector3(0, 0, 1);

        void Start()
        {
            vector1 = World.Instance.vector1;
            vector2 = World.Instance.vector2;
            vector3 = World.Instance.vector3;

            chunkSize = World.Instance.chunkSize;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshCollider = gameObject.AddComponent<MeshCollider>();

            voxels = new Voxel[chunkSize, chunkSize, chunkSize];
            chunkOrigin = Vector3.zero;
            GenerateMesh();
        }
        public void GenerateMesh()
        {
            vertices.Clear();
            triangles.Clear();
            uvs.Clear();

            InitializeVoxels();


            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray(),
                uv = uvs.ToArray()
            };

            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshCollider.sharedMesh = mesh;

            meshRenderer.material = VoxelMaterial;
        }
        public void InitializeVoxels()
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        Vector3 voxelPosition = chunkOrigin + (x * vector1) + (y * vector2) + (z * vector3);

                        if (Startup == true)
                        {
                            voxels[x, y, z] = new Voxel(voxelPosition);

                            Vector3 verticeBase = new Vector3(x, y, z);

                            Vector3[] originVertices = new Vector3[]
                            {
                            verticeBase,                            // (0, 0, 0)
                            verticeBase + o_vector1,                  // (1, 0, 0)
                            verticeBase + o_vector1 + o_vector2,        // (1, 1, 0)
                            verticeBase + o_vector2,                  // (0, 1, 0)
                            verticeBase + o_vector3,                  // (0, 0, 1)
                            verticeBase + o_vector1 + o_vector3,        // (1, 0, 1)
                            verticeBase + o_vector1 + o_vector2 + o_vector3, // (1, 1, 1)
                            verticeBase + o_vector2 + o_vector3         // (0, 1, 1)
                            };

                            voxels[x, y, z].SetOriginVertices(originVertices);
                        }

                        if (voxels[x, y, z].isActive)
                        {
                            if (IsFaceVisible(x, y + 1, z)) 
                                AddFaceData(x, y, z, 0); // Top
                            if (IsFaceVisible(x, y - 1, z)) 
                                AddFaceData(x, y, z, 1); // Bottom
                            if (IsFaceVisible(x - 1, y, z))
                                AddFaceData(x, y, z, 2); // Left
                            if (IsFaceVisible(x + 1, y, z)) 
                                AddFaceData(x, y, z, 3); // Right
                            if (IsFaceVisible(x, y, z + 1)) 
                                AddFaceData(x, y, z, 4); // Front
                            if (IsFaceVisible(x, y, z - 1)) 
                                AddFaceData(x, y, z, 5); // Back
                        }
                    }
                }
            }
            Startup = false;
        }
        public bool IsFaceVisible(int x, int y, int z)
        {
            if (x < 0 || x >= chunkSize || y < 0 || y >= chunkSize || z < 0 || z >= chunkSize)
                return true; 

            if (voxels[x, y, z] != null && !voxels[x, y, z].isActive)
                return true; 

            return false; 
        }
        private Vector3 TransformVertex(Vector3 vertex)
        {
            Matrix4x4 transformationMatrix = new Matrix4x4(
                new Vector4(vector1.x, vector1.y, vector1.z, 0),
                new Vector4(vector2.x, vector2.y, vector2.z, 0),
                new Vector4(vector3.x, vector3.y, vector3.z, 0),
                new Vector4(0, 0, 0, 1)
            );

            return transformationMatrix.MultiplyPoint3x4(vertex);
        }

        public void AddFaceData(int x, int y, int z, int faceIndex)
        {
            Vector3 voxelPosition = new Vector3(x, y, z); 

            if (faceIndex == 0) // Top Face
            {
                AddTransformedQuad(
                    new Vector3(x, y + 1, z),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x + 1, y + 1, z)
                );
            }
            else if (faceIndex == 1) // Bottom Face
            {
                AddTransformedQuad(
                    new Vector3(x, y, z),
                    new Vector3(x + 1, y, z),
                    new Vector3(x + 1, y, z + 1),
                    new Vector3(x, y, z + 1)
                );
            }
            else if (faceIndex == 2) // Back Face
            {
                AddTransformedQuad(
                    new Vector3(x, y, z),
                    new Vector3(x, y, z + 1),
                    new Vector3(x, y + 1, z + 1),
                    new Vector3(x, y + 1, z)
                );
            }
            else if (faceIndex == 3) // Front Face
            {
                AddTransformedQuad(
                new Vector3(x + 1, y, z),    
                new Vector3(x + 1, y + 1, z), 
                new Vector3(x + 1, y + 1, z + 1), 
                new Vector3(x + 1, y, z + 1)  
                );
            }
            else if (faceIndex == 4) // Left Face
            {
                AddTransformedQuad(
                    new Vector3(x, y, z + 1),
                    new Vector3(x + 1, y, z + 1),
                    new Vector3(x + 1, y + 1, z + 1),
                    new Vector3(x, y+1, z + 1)
                );
            }
            else if (faceIndex == 5) // Right
            {
                AddTransformedQuad(
                    new Vector3(x + 1, y, z),
                    new Vector3(x, y, z),
                    new Vector3(x, y + 1, z),
                    new Vector3(x + 1, y + 1, z)
                );
            }
        }

        private void AddTransformedQuad(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 v3)
        {
            vertices.Add(TransformVertex(v0));
            vertices.Add(TransformVertex(v1));
            vertices.Add(TransformVertex(v2));
            vertices.Add(TransformVertex(v3));

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(1, 0));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(0, 1));

            AddTriangleIndices(true);
        }
        public Voxel[] GetVoxelAt(Vector3 position)
        {
            int x = (int)position.x;
            int y = (int)position.y;
            int z = (int)position.z;

            if (x < 0 || x >= voxels.GetLength(0) || y < 0 || y >= voxels.GetLength(1) || z < 0 || z >= voxels.GetLength(2) || voxels[x,y,z].isActive == false)
            {
                Debug.LogWarning("Position is out of bounds. R  eturning neighboring voxels.");

                List<Voxel> neighbors = new List<Voxel>();

                if (z - 1 >= 0 && z - 1 < chunkSize && x >= 0 && x < chunkSize && y >= 0 && y < chunkSize)
                {
                    Voxel na = new Voxel(new Vector3(x, y, z - 1));
                    na.SetOriginVertices(voxels[x, y, z - 1].GetOriginVertices());
                    neighbors.Add(na);
                }

                if (y - 1 >= 0 && y - 1 < chunkSize && x >= 0 && x < chunkSize && z >= 0 && z < chunkSize)
                {
                    Voxel na = new Voxel(new Vector3(x, y-1, z));
                    na.SetOriginVertices(voxels[x, y-1, z].GetOriginVertices());
                    neighbors.Add(na);
                }       

                if (x - 1 >= 0 && x - 1 < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
                {
                    Voxel na = new Voxel(new Vector3(x - 1, y, z));
                    na.SetOriginVertices(voxels[x-1, y, z ].GetOriginVertices());
                    neighbors.Add(na);

                }

                return neighbors.ToArray();
            }
            Voxel n = new Voxel(position);
            return new Voxel[] { n };
        }
        public void AddTriangleIndices(bool isQuad)
        {
            int vertCount = vertices.Count;

            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 3);
            triangles.Add(vertCount - 2);

            triangles.Add(vertCount - 4);
            triangles.Add(vertCount - 2);
            triangles.Add(vertCount - 1);
        }
        public bool Contains(Vector3 worldPosition)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int y = 0; y < chunkSize; y++)
                {
                    for (int z = 0; z < chunkSize; z++)
                    {
                        Vector3 voxelPosition = voxels[x, y, z].position;

                        if (voxelPosition == worldPosition)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void SetVoxelInactive(Vector3 localVoxelPosition)
        {
            Debug.Log("Removing: " + localVoxelPosition);

            int x = Mathf.FloorToInt(localVoxelPosition.x);
            int y = Mathf.FloorToInt(localVoxelPosition.y);
            int z = Mathf.FloorToInt(localVoxelPosition.z);

            if (x >= 0 && x < chunkSize && y >= 0 && y < chunkSize && z >= 0 && z < chunkSize)
            {

                voxels[x, y, z].isActive = false;
                UpdateMesh();
            }
        }
        public void UpdateMesh()
        {
            GenerateMesh();
        }
    }

