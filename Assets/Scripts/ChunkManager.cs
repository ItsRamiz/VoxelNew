using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public List<Chunk> chunks = new List<Chunk>();

    public Chunk GetChunkContaining(Vector3 worldPosition)
    {
        foreach (var chunk in chunks)
        {
            if (chunk.Contains(worldPosition))
            {
                return chunk;
            }
        }
        return null; 
    }
}