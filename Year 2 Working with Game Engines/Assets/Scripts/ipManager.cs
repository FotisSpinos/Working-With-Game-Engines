using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ipManager : MonoBehaviour {

    public List<string> ipList = new List<string>();
    public List<string> hostList = new List<string>();
    List<GameObject> ipButtons = new List<GameObject>();

    public GameObject ipItem;
    public Transform startSpawnObj;

    private void AddHostToList(string hostName, string[] ipAddress)
    {
        SetSpawnItem();

        if (!ipList.Contains(ipAddress[ipList.Count]))
        {
            // Instanciate Button
            GameObject ipItem = (GameObject)GameObject.Instantiate(this.ipItem, startSpawnObj.transform.position, Quaternion.identity);

            // Add data to lists
            hostList.Add(hostName);
            ipList.Add(ipAddress[0]);
            ipButtons.Add(ipItem);

            // Set button parent
            ipItem.transform.SetParent(startSpawnObj);

            ipItem.GetComponent<ipButtonScript>().SetGameName(hostName);

            // Set button text
            ipItem.transform.GetChild(0).GetComponent<Text>().text = hostList[hostList.Count - 1] + " " + ipList[ipList.Count - 1];
        }
    }

    void SetSpawnItem()
    {
        if (GameObject.Find("ipPanel"))
        {
            Debug.Log("ipPanel object found");
            startSpawnObj = GameObject.Find("ipPanel").transform;
        }
        else
        {
            Debug.Log("ipPanel Object has not been found");
        }
    }

    private void Awake()
    {
        //ServerUtilityScript.OnHostsFound += DisplayList;
        //ServerUtilityScript.OnHostIdentified += AddHostToList;
    }

    private void Start()
    {
        ServerUtilityScript.OnHostsFound += DisplayList;
        ServerUtilityScript.OnHostIdentified += AddHostToList;
        SetSpawnItem();
    }

    private void DisplayList()
    {
        float yOffset = 50f;
        Vector3 startPosition = startSpawnObj.transform.position - new Vector3(0,30,0);

        if (startSpawnObj != null)
        {
            startPosition = startSpawnObj.position;
        }
        else
        {
            startPosition = new Vector3(0, 0, 0);
        }

        foreach (GameObject button in ipButtons)
        {
            button.transform.position = startPosition + new Vector3(0, yOffset, 0); 
            startPosition.y -= yOffset;
        }
    }
}