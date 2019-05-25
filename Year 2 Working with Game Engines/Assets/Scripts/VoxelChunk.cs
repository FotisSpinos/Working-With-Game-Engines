using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(VoxelGenerator))]

public class VoxelChunk : MonoBehaviour
{
    VoxelGenerator voxelGenerator;
    int chunkSize = 16;
    int[,,] terrainArray;
    Vector3 start, end; 

    // delegate signature
    public delegate void EventBlockChangedWithType(int blockType, bool voxelDestroyed);
    // event instances for EventBlockChanged
    public static event EventBlockChangedWithType OnEventBlockChanged;

    public delegate string EventVoxelChunkSave();
    // event instances for EventBlockChanged
    public static event EventVoxelChunkSave OnEventChunkSaved;

    public delegate string EventVoxelChunkLoad();
    // event instances for EventBlockChanged
    public static event EventVoxelChunkSave OnEventChunkLoaded;

    private void OnEnable()
    {
        PlayerScipt.OnEventBlockSet += SetBlock;
        PlayerScipt.OnEventBlockGet += BlockGet;
    }

    private int BlockGet(Vector3 point)
    {
        return terrainArray[(int)point.x, (int)point.y, (int)point.z];
    }

    private void OnDisable()
    {
        PlayerScipt.OnEventBlockSet -= SetBlock;
        PlayerScipt.OnEventBlockGet -= BlockGet;
    }

    private void Awake()    //when ovject is instanciated start doesn't run first
    {
        voxelGenerator = GetComponent<VoxelGenerator>();
        terrainArray = new int[chunkSize, chunkSize, chunkSize];
        voxelGenerator.Initialise();


        LoadVoxelChunk();
        Debug.Log(start);

        CreateTerrain();

        voxelGenerator.UpdateMesh();
    }

    public void SaveVoxelChunk()
    {
        string fileName = OnEventChunkSaved();
        Debug.Log("save fileName " + fileName);
        if (fileName == "")
        {
            Debug.Log(fileName + "oops");
            XMLFileWriter.SaveChunkToXMLFile(terrainArray, "NewChunk", start, end);
        }
        else
        {
            XMLFileWriter.SaveChunkToXMLFile(terrainArray, fileName, start, end);
        }
    }

    public void LoadVoxelChunk()
    {
        string fileName = OnEventChunkLoaded != null ? OnEventChunkLoaded() : "";

        Debug.Log(" load fileName " + fileName);
        if (fileName == null || fileName == "")
        {
            terrainArray = XMLFileWriter.LoadChunkFromXMLFile(16, out start, out end, "AssessmentChunk1");
        }
        else
        {
            terrainArray = XMLFileWriter.LoadChunkFromXMLFile(16, out start, out end, fileName);
        }
        // Draw the correct faces
        CreateTerrain();
        // Update mesh info
        voxelGenerator.UpdateMesh();
    }

    private void CreateTerrain()
    {
        for (int x = 0; x < terrainArray.GetLength(0); x++)
        {
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < terrainArray.GetLength(2); z++)
                {
                    if(terrainArray[x,y,z] != 0)
                    {
                        string tex;
                        
                        switch (terrainArray[x, y, z])
                        {
                            case 1:
                                tex = "Grass";
                                break;
                            case 2:
                                tex = "Dirt";
                                break;
                            case 3:
                                tex = "Stone";
                                break;
                            case 4:
                                tex = "Sand";
                                break;
                            default:
                                tex = "Grass";
                                break;
                        }
                        // check if we need to draw the negative x face
                        if (x == 0 || terrainArray[x - 1, y, z] == 0)
                        {
                            voxelGenerator.CreateNegativeXFace(x, y, z, tex);
                        }
                        // check if we need to draw the positive x face
                        if (x == terrainArray.GetLength(0) - 1 || terrainArray[x + 1, y, z] == 0)
                        {
                            voxelGenerator.CreatePositiveXFace(x, y, z, tex);
                        }

                        // check if we need to draw the negative z face
                        if (z == 0 || terrainArray[x, y, z - 1] == 0)
                        {
                            voxelGenerator.CreateNegativeZFace(x, y, z, tex);
                        }

                        // check if we need to draw the positive z face
                        if (z == terrainArray.GetLength(2) - 1 ||
                            terrainArray[x, y, z + 1] == 0)
                        {
                            voxelGenerator.CreatePositiveZFace(x, y, z, tex);
                        }

                        // check if we need to draw the negative y face
                        if (y == 0 || terrainArray[x, y - 1, z] == 0)
                        {
                            voxelGenerator.CreateNegativeYFace(x, y, z, tex);
                        }

                        // check if we need to draw the positive y face
                        if (y == terrainArray.GetLength(1) - 1 || terrainArray[x, y + 1, z] == 0)
                        {
                            voxelGenerator.CreatePositiveYFace(x, y, z, tex);
                        }
                    }
                }
            }
        }
    }

    private void InitialiseTerrain()
    {
        for(int x = 0; x < terrainArray.GetLength(0); x++)
        {
            for (int y = 0; y < terrainArray.GetLength(1); y++)
            {
                for (int z = 0; z < terrainArray.GetLength(0); z++)
                {
                    if(y > 4)
                    {
                        terrainArray[x, y, z] = 0;
                    }
                    else if(y < 3)  // Below top visible surface
                    {
                        terrainArray[x, y, z] = 2;
                    }
                    else if(y == 3) // Top visible surface
                    {
                        terrainArray[x, y, z] = 1;
                    }
                    else
                    {
                        terrainArray[x, y, z] = 0;
                    }
                }
            }
        }
    }

    public int GetChunkSize()
    {
        return chunkSize;
    }

    [RPC] public void SetBlock(Vector3 index, int blockType)
    {
        if ((index.x >= 0 &&
            index.x < terrainArray.GetLength(0)) &&
            (index.y >= 0 &&
            index.y < terrainArray.GetLength(1)) &&
            (index.z >= 0 && index.z < terrainArray.GetLength(2)))
        {

            if(blockType == 0 && OnEventBlockChanged != null)  // If a block has been destroyed
                OnEventBlockChanged(terrainArray[(int)index.x, (int)index.y, (int)index.z], true);

            // Change the block to the required type
            terrainArray[(int)index.x, (int)index.y, (int)index.z] = blockType;
            // Create the new mesh
            CreateTerrain();
            // Update the mesh data
            voxelGenerator.UpdateMesh();

            //Debug.Log("The index is: " + index);
        }
        
        if (blockType != 0 && OnEventBlockChanged != null) // If a block has not been destroyed
            OnEventBlockChanged(blockType, false);
            
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            SaveVoxelChunk();
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            LoadVoxelChunk();
        }
    }

    public bool isTraversable(Vector3 voxel)
    {
        if (voxel.x >= 0 && voxel.x < chunkSize &&
            voxel.y - 1 >= 0 && voxel.y - 1 < chunkSize &&
            voxel.z >= 0 && voxel.z < chunkSize)
        {
            bool isEmpty = terrainArray[(int)voxel.x, (int)voxel.y, (int)voxel.z] == 0;
            bool isBellowStone = terrainArray[(int)voxel.x, (int)voxel.y - 1, (int)voxel.z] == 3;   //(int)voxel.y - 1
            bool isBellowDirt = terrainArray[(int)voxel.x, (int)voxel.y - 1, (int)voxel.z] == 2;
            return isEmpty && (isBellowStone || isBellowDirt);
        }
        return false;
    }

    public int GetDistanceCost(Vector3 point)
    {
        //point = new Vector3(point.x - 1, point.y - 1, point.z - 1);
        if (terrainArray[(int)point.x, (int)point.y - 1, (int)point.z] == 3)
        {
            return 1;
        }
        if (terrainArray[(int)point.x, (int)point.y - 1, (int)point.z] == 2)
        {
            return 3;
        }
        else
            return -1;
    }

    public Vector3 GetStart()
    {
        return start;
    }

    public Vector3 GetEnd()
    {
        return end;
    }
}
