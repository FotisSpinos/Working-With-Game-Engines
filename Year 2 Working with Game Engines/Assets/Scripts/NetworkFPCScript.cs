using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFPCScript : MonoBehaviour {

    float lastSyncTime = 0f;
    float syncDelay = 0f;
    float syncTime = 0f;
    Vector3 startPosition;
    Vector3 endPosition = Vector3.zero;

    void Start()
    {
        if (GetComponent<NetworkView>().isMine)
        {
            MonoBehaviour[] components = GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour m in components)
            {
                m.enabled = true;
            }
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }
        }
    }

    private void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
    {
        Vector3 syncPosition = Vector3.zero;
        if (stream.isWriting)
        {
            syncPosition = transform.position;
            stream.Serialize(ref syncPosition);
        }
        else
        {
            stream.Serialize(ref syncPosition);
            syncTime = 0f;
            syncDelay = Time.time - lastSyncTime;
            lastSyncTime = Time.time;
            startPosition = transform.position;
            endPosition = syncPosition + GetComponent<Rigidbody>().velocity * syncDelay;//syncPosition;
        }
    }

    private void Update()
    {
        if (!GetComponent<NetworkView>().isMine)
        {
            syncTime += Time.deltaTime;
            if (syncTime < syncDelay)
            {
                transform.position = Vector3.Lerp(startPosition, endPosition, syncTime / syncDelay);
            }
        }
        else
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Vector3 v;
                VoxelChunk vcs;

                if (PickThisBlock(out v, out vcs, true, 4))
                {
                    NetworkView nv = vcs.GetComponent<NetworkView>();
                    if (nv != null)
                    {
                        nv.RPC("SetBlock", RPCMode.All, v, 0);
                    }
                }
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                Vector3 v;
                VoxelChunk vcs;
                if (PickThisBlock(out v, out vcs, false, 4))
                {
                    NetworkView nv = vcs.GetComponent<NetworkView>();
                    if (nv != null)
                    {
                        nv.RPC("SetBlock", RPCMode.All, v, 1);
                    }
                }
            }
        }
    }

    bool PickThisBlock(out Vector3 v, out VoxelChunk voxelChunkScript, bool destr, float dist)
    {
        v = new Vector3();
        voxelChunkScript = null;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            // check if the target we hit has a VoxelChunk script
            voxelChunkScript = hit.collider.gameObject.GetComponent<VoxelChunk>();
            if (voxelChunkScript != null)
            {
                // offset toward centre of the block hit
                //v = hit.point - hit.normal / 2;
                v = destr ? hit.point - hit.normal / 2 : hit.point + hit.normal / 2;
                // round down to get the index of the block hit
                v.x = Mathf.Floor(v.x);
                v.y = Mathf.Floor(v.y);
                v.z = Mathf.Floor(v.z);
                return true;
            }
        }
        return false;
    }
}
