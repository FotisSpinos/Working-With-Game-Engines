using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]

public class VoxelGenerator : MonoBehaviour
{
    Mesh mesh;
    MeshCollider meshCollider;

    List<Vector3> vertexList;
    List<int> triIndexList;
    List<Vector2> UVList;

    List<string> texNames;
    List<Vector2> texCoords;
    Dictionary<string, Vector2> texNameCoordDictionary;
    int numQuads;
    
    // Use this for initialization
    void Awake()
    {
        // Initialize variables
        Initialise();
    }

    public void Initialise()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        meshCollider = GetComponent<MeshCollider>();
        vertexList = new List<Vector3>();
        triIndexList = new List<int>();
        UVList = new List<Vector2>();


        texNames = new List<string>();
        texCoords = new List<Vector2>();
        texNameCoordDictionary = new Dictionary<string, Vector2>();

        texNames.Add("Grass");
        texNames.Add("Stone");
        texNames.Add("Dirt");
        texNames.Add("Sand");
        texCoords.Add(new Vector2(0, 0));
        texCoords.Add(new Vector2(0.5f, 0));
        texCoords.Add(new Vector2(0, 0.5f));
        texCoords.Add(new Vector2(0.5f, 0.5f));
        CreateTextureNameCoordDictionary();
    }

    public void CreateVoxel(int x, int y, int z, string texture)
    {
        CreateNegativeZFace(x, y, z, texture);
        CreatePositiveZFace(x, y, z, texture);
        CreateNegativeYFace(x, y, z, texture);
        CreatePositiveYFace(x, y, z, texture);
        CreateNegativeXFace(x, y, z, texture);
        CreatePositiveXFace(x, y, z, texture);
    }

    public void CreateVoxel(int x, int y, int z, Vector2 uvCoords)
    {
        CreateNegativeZFace(x, y, z, uvCoords);
        CreatePositiveZFace(x, y, z, uvCoords);
        CreateNegativeYFace(x, y, z, uvCoords);
        CreatePositiveYFace(x, y, z, uvCoords);
        CreateNegativeXFace(x, y, z, uvCoords);
        CreatePositiveXFace(x, y, z, uvCoords);
    }

    private void CreateTextureNameCoordDictionary()
    {        
        if(texCoords.Count == texNames.Count)
        {
            for(int i = 0; i < texCoords.Count; i++)
            {
                texNameCoordDictionary.Add(texNames[i], texCoords[i]);
            }
        }
        else
        {
            Debug.Log("texNames and texCoords count mismatch");
        }
    }

    void ClearMeshData()
    {
        numQuads = 0;
        vertexList.Clear();
        triIndexList.Clear();
        UVList.Clear();
    }

    public void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertexList.ToArray();
        mesh.uv = UVList.ToArray();
        mesh.triangles = triIndexList.ToArray();
        mesh.RecalculateNormals();

        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;

        ClearMeshData();
    }

    public void CreateNegativeZFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x, y, z));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreateNegativeZFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x, y, z));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveZFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveZFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreateNegativeYFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x, y, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreateNegativeYFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y, z));
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y, z + 1));
        vertexList.Add(new Vector3(x, y, z + 1));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveYFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveYFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y + 1, z));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreateNegativeXFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y, z));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreateNegativeXFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x, y, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z + 1));
        vertexList.Add(new Vector3(x, y + 1, z));
        vertexList.Add(new Vector3(x, y, z));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveXFace(int x, int y, int z, string texture)
    {
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));

        Vector2 uvCoords = texNameCoordDictionary[texture];
        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    public void CreatePositiveXFace(int x, int y, int z, Vector2 uvCoords)
    {
        vertexList.Add(new Vector3(x + 1, y, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z));
        vertexList.Add(new Vector3(x + 1, y + 1, z + 1));
        vertexList.Add(new Vector3(x + 1, y, z + 1));

        AddUVCoords(uvCoords);
        AddTriangleIndices();
    }

    void AddTriangleIndices()
    {
        triIndexList.Add(numQuads * 4);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 3);
        triIndexList.Add((numQuads * 4) + 1);
        triIndexList.Add((numQuads * 4) + 2);
        triIndexList.Add((numQuads * 4) + 3);
        numQuads++;
    }

    public void AddUVCoords(Vector2 uvCoords)
    {
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y + 0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f, uvCoords.y + 0.5f));
        UVList.Add(new Vector2(uvCoords.x + 0.5f, uvCoords.y));
        UVList.Add(new Vector2(uvCoords.x, uvCoords.y));
    }
}
