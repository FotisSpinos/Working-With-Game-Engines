using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerScipt : MonoBehaviour {

    public delegate void EventBlockPicked(string blockName, bool picked);
    public static event EventBlockPicked OnEventBlockPicked;

    public delegate void EventSetBlockWithType(Vector3 point, int blockType);
    public static event EventSetBlockWithType OnEventBlockSet;

    public delegate int EventGetBlockWithType(Vector3 point);
    public static event EventGetBlockWithType OnEventBlockGet;

    public delegate int EventGetItemAmount(string itemName);
    public static event EventGetItemAmount OnEventGetItemAmount;

    private int blockSelected = 1;
    private int collectableSpawned = -1;
    public Material voxelMaterial;
    //VoxelGenerator voxelGenerator;
    bool fpsControllerEnabled = true;
    // Update is called once per frame
    private void Start()
    {
        Cursor.visible = false;
    }

    void Update()
    {
        if (Input.GetKeyDown("1"))
        {
            blockSelected = 1;
        }
        else if (Input.GetKeyDown("2"))
        {
            blockSelected = 2;
        }
        else if (Input.GetKeyDown("3"))
        {
            blockSelected = 3;
        }
        else if (Input.GetKeyDown("4"))
        {
            blockSelected = 4;
        }

        if (Input.GetKey("r"))
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 4);
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].tag == "Collectable")
                {
                    colliders[i].GetComponent<Rigidbody>().AddForce((-colliders[i].transform.position + transform.position).normalized * 500);
                }
            }
        }

        if (Input.GetButtonDown("Fire1") && fpsControllerEnabled)
        {
            Vector3 v;

            if (PickBlock(out v, 4, Input.GetButtonDown("Fire1"))
                && OnEventBlockSet != null
                && OnEventBlockGet != null)
            {
                collectableSpawned = OnEventBlockGet(v);    //The collectable spawned 
                OnEventBlockSet(v, 0);
                SpawnBlock(v);
            }
        }

        else if (Input.GetButtonDown("Fire2") && fpsControllerEnabled)
        {
            Vector3 v;

            if (PickBlock(out v, 4, Input.GetButtonDown("Fire1")) 
                && OnEventGetItemAmount != null
                && OnEventGetItemAmount(GetTex(blockSelected)) > 0 
                && OnEventBlockPicked != null
                && OnEventBlockSet != null)
            {
                OnEventBlockPicked(GetTex(blockSelected), false);
                OnEventBlockSet(v, blockSelected);
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) || (Input.GetButtonDown("Fire1") && Cursor.visible))
        {
            Debug.Log(Cursor.visible);
            if (!GetComponent<FirstPersonController>().enabled)
            {
                Debug.Log("FPC Disabled");
                GetComponent<FirstPersonController>().enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Debug.Log("FPC Enabled");
                Cursor.visible = true;
                GetComponent<FirstPersonController>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.lockState = CursorLockMode.Confined;
            }
            fpsControllerEnabled = !fpsControllerEnabled;
        }
    }

    string GetTex(int id)
    {
        switch (id)
        {
            case 1:
                return "Grass";                
            case 2:
                return "Dirt";
            case 3:
                return "Stone";
            case 4:
                return "Sand";
            default:
                return "Grass";
        }
    }

    private void SpawnBlock(Vector3 point)
    {
        string tex = "Grass";
        tex = GetTex(collectableSpawned);
        
        int blockID = collectableSpawned;

        // Instanciate new game object
        GameObject cubeCollectable = new GameObject();

        // Add Components 
        cubeCollectable.AddComponent<VoxelGenerator>();
        cubeCollectable.AddComponent<CollectableScript>();
        cubeCollectable.AddComponent<Rigidbody>();

        // Edit components
        cubeCollectable.GetComponent<MeshCollider>().convex = true;
        cubeCollectable.GetComponent<Renderer>().material = voxelMaterial;
        cubeCollectable.name = "Collectable Object";
        cubeCollectable.tag = "Collectable";
        cubeCollectable.GetComponent<CollectableScript>().collectableID = blockID;
        cubeCollectable.GetComponent<CollectableScript>().collectableName = tex;

        // Generate Voxel 
        cubeCollectable.GetComponent<VoxelGenerator>().Initialise();
        cubeCollectable.GetComponent<VoxelGenerator>().CreateVoxel(0,0,0, tex);
        cubeCollectable.GetComponent<VoxelGenerator>().UpdateMesh();

        //Set game object's local scale
        cubeCollectable.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); 

        //Set game object's position
        cubeCollectable.transform.position = new Vector3((int)point.x + cubeCollectable.transform.localScale.x /2
            , (int)point.y + cubeCollectable.transform.localScale.y / 2
            , (int)point.z + cubeCollectable.transform.localScale.z / 2);
    }

    private bool PickBlock(out Vector3 v, float dist, bool fire1)
    {
        v = new Vector3();

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, 4))
        {
            if (hit.collider.gameObject.tag != "Collectable")   // If the raycast does not hit a collectable
            {
                v = fire1 ? hit.point - hit.normal / 2 : hit.point + hit.normal / 2;

                v.x = Mathf.Floor(v.x);
                v.y = Mathf.Floor(v.y);
                v.z = Mathf.Floor(v.z);
                return true;
            }
        }
        return false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Collectable" && OnEventBlockPicked != null)
        {
            OnEventBlockPicked(hit.gameObject.GetComponent<CollectableScript>().collectableName, true);
            Destroy(hit.gameObject);
            print("Collision OCCH");
        }
        else if(OnEventBlockPicked == null)
            Debug.Log("OnEventBlockPick is null");
    }

    private void OnCollisionEnter(Collision hit)    //on collision from collectable is responsible for trigering that as well
    {
        if (hit.gameObject.tag == "Collectable"
            && OnEventBlockPicked != null)
        {
            OnEventBlockPicked(hit.gameObject.GetComponent<CollectableScript>().collectableName, true);
            Destroy(hit.gameObject);
            print("Collision OCE");
        }
        else if (OnEventBlockPicked == null)
            Debug.Log("OnEventBlockPick is null");
    }
}
