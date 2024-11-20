using UnityEngine;

public class World : MonoBehaviour
{
    public static World Instance { get; private set; }

    public Vector3 vector1 = new Vector3(0.7f, 0, 0);
    public Vector3 vector2 = new Vector3(0.4f, 0.7f, 0);
    public Vector3 vector3 = new Vector3(0, 0, 0.7f);

    public float x_offset;

    public int chunkSize = 5;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
