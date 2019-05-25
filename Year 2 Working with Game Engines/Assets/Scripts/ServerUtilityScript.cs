using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ServerUtilityScript : MonoBehaviour {

    string typeName = "WGEFotiosSpinosGame";    
    string gameName = "WGEJohnDoeRoom"; //WGEJohnDoeRoom
    HostData[] hostList;
    string ipAddress = "10.15.3.147";   //Lab: 10.15.3.147 PC: 192.168.187.1
    List<string> ipAdressList;

    public GameObject voxelChunkPrefab;
    public GameObject networkFPCPrefab;
    public GameObject mainCamera;
    
    public delegate void HostIdentified(string hostName, string[] ipAddress);
    public static event HostIdentified OnHostIdentified;

    public delegate void HostsFound();
    public static event HostsFound OnHostsFound;

    bool findHosts;

    private void Start()
    {
        MasterServer.ipAddress = ipAddress;
        UImanager.OnIpEdited += SetIPAdress;
        UImanager.OnIpEdited += EditIPAdress;

        ipButtonScript.OnButtonPress += ConnectToGameServer;
        ipAdressList = new List<string>();

        findHosts = true;
        StartCoroutine(MyCoroutine());
     }

    IEnumerator MyCoroutine()
    {
        yield return new WaitForEndOfFrame();
        MasterServer.RequestHostList(typeName);
    }

    private void EditIPAdress(string ipAddress)
    {
        this.ipAddress = ipAddress;
    }

    public void AddIPToList()
    {
        Debug.Log("ip added: " + ipAddress);
        ipAdressList.Add(ipAddress);
    }

    private void ConnectToGameServer(string gameName)
    {
        this.gameName = gameName;
        MasterServer.RequestHostList(typeName);
    }

    private void SetIPAdress(string ipAddress)
    {
        MasterServer.ipAddress = ipAddress;
        Debug.Log("SUS" + MasterServer.ipAddress);
    }

    public void StartSetrver()
    {
        if(!Network.isServer && !Network.isClient)
        {
            Network.InitializeServer(4, 25000, !Network.HavePublicAddress());   //25000
            MasterServer.RegisterHost(typeName, gameName);
        }
    }

    private void OnServerInitialized()
    {
        GameObject vc = (GameObject)Network.Instantiate(voxelChunkPrefab, Vector3.zero, Quaternion.identity, 0);
    }

    public void RefreshHostList()
    {
        MasterServer.RequestHostList(typeName);
    }

    void OnConnectedToServer()
    {
        Debug.Log("Server Joined");

        mainCamera.SetActive(false);
        Network.Instantiate(networkFPCPrefab, new Vector3(8, 8, 8), Quaternion.identity, 0);
    }

    private void OnMasterServerEvent(MasterServerEvent msEvent)
    {
        if (msEvent == MasterServerEvent.HostListReceived)
        {
            hostList = MasterServer.PollHostList();
            foreach(HostData hd in hostList)
            {
                Debug.Log("event runs" + gameName + "/" + hd.gameName);
                if (findHosts && OnHostIdentified != null)
                {
                    OnHostIdentified(hd.gameName, hd.ip);
                    Debug.Log(hd.gameType + " " + hd.ip[0].ToString());
                }

                if (hd.gameName == gameName && !findHosts)
                {
                    Network.Connect(hd);
                }
            }
            if (findHosts == true)
            {
                OnHostsFound();
                findHosts = false;
            }
        }
    }
}