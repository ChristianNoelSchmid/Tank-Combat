using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainSegment
{
    public Vector3 data;
    public TerrainSegment next;
}

public class TerrainSystem : MonoBehaviour
{
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;

    [SerializeField]
    private int terrainSegmentCount;
    private List<Vector2> terrainSegments;

    private List<Vector3> meshIndices;
    private List<int> meshTriangles;

    private MeshFilter meshFilter;
    private PolygonCollider2D polygonCollider;
    private Camera mainCamera;

    void Start()
    {
        meshFilter = GetComponent<MeshFilter>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        terrainSegments = new List<Vector2>();
        meshIndices = new List<Vector3>();
        meshTriangles = new List<int>();

        mainCamera = Camera.main;

        InitializeSegments();
        BuildTerrain();
    }

    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Explode(mainCamera.ScreenToWorldPoint(Input.mousePosition), 3.0f);
            BuildTerrain();
        }
    }

    void Explode(Vector2 position, float force)
    {
        for(int i = 0; i < terrainSegmentCount; ++i)
        {
            float distance = Vector2.Distance(terrainSegments[i], position);
            if(distance < force)
            {
                terrainSegments[i] -= (position - terrainSegments[i]) * (force - distance);
                if(i > 0 && terrainSegments[i].x < terrainSegments[i - 1].x)
                {
                    terrainSegments[i] = new Vector2(terrainSegments[i - 1].x, terrainSegments[i].y);
                }
                if(i < terrainSegmentCount - 1 && terrainSegments[i].x < terrainSegments[i + 1].x)
                {
                    terrainSegments[i] = new Vector2(terrainSegments[i + 1].x, terrainSegments[i].y);
                }

                meshIndices[i * 2] = terrainSegments[i];
            }
        }
    }

    private void InitializeSegments()
    { 
        // Set PolygonColliders count to terrainSegmentCount, for each segment
        // plus the bottom two vertices.
        float offset = width / 2.0f;
        for(int i = 0; i < terrainSegmentCount; ++i)
        {
            terrainSegments.Add(new Vector2(width * (i / (float)terrainSegmentCount) - offset, Mathf.PerlinNoise((i + 1000) * 0.02f, 0.0f) * 50.0f));
        }
        terrainSegments.Add(new Vector2(width - offset, -height));
        terrainSegments.Add(new Vector2(-offset, -height));

        for(int i = 0; i < terrainSegmentCount; ++i)
        {
            meshIndices.Add(terrainSegments[i]);
            if (i != terrainSegmentCount - 1)
            {
                meshIndices.Add(new Vector3(width * ((i + 0.5f) / terrainSegmentCount) - offset, -height / 2.0f, 0.0f));
                meshTriangles.Add(i);
                meshTriangles.Add(i + 2);
                meshTriangles.Add(i + 1);
            }
        }
    }

    /// <summary>
    /// Generates the Mesh and normals for the Mesh terrain, and
    /// updates the PolygonCollider2D to match the current terrain.
    /// </summary>
    private void BuildTerrain()
    { 
        // Set the PolygonCollider2D's path to terrainSegments (simple as that)
        polygonCollider.SetPath(0, terrainSegments.ToArray());

        // Retrieve the bottom-right index of the terrain segments
        // Used to give the segments a third vertex to connect to
        int bottomRightIndex = terrainSegmentCount;

        // Create a list of indices for the triangles for the Mesh
        List<int> indices = new List<int>();

        // The surface meshIndices have been already updated, but now
        // the subsurface ones must be aligned in between the vertices.
        for (int i = 1; i < meshIndices.Count; i += 2)
        {
            Vector3 midPoint = new Vector3(
                (meshIndices[i - 1].x + meshIndices[i + 1].x) / 2.0f,
                (meshIndices[i - 1].y + meshIndices[i + 1].y) / 2.0f,
                0.0f
            );

            float angleRadians = Mathf.Atan2(
                meshIndices[i + 1].y - meshIndices[i - 1].y,
                meshIndices[i + 1].x - meshIndices[i - 1].x
            ) + (Mathf.PI / 2.0f);

            midPoint -= new Vector3(
                Mathf.Cos(angleRadians),
                Mathf.Sin(angleRadians)
            );

            meshIndices[i] = midPoint;
        }

        meshFilter.mesh.vertices = meshIndices.ToArray();
        meshFilter.mesh.SetTriangles(meshTriangles, 0);
    }
}
