using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    // Parent object inventory item
    public Transform parentPanel;
    // Item info to build inventory items
    public List<string> itemNames;
    public List<int> itemAmounts;
    // Starting template item
    public GameObject startItem;
    public GameObject itemPrefub;

    List<InventoryItemScript> inventoryList;

    public delegate bool CompareItems(InventoryItemScript a, InventoryItemScript b);
    CompareItems compareItems;


    private void OnEnable()
    {
        PlayerScipt.OnEventBlockPicked += UpdateItemAmounts;
        PlayerScipt.OnEventGetItemAmount += GetItemAmount;

        UImanager.OnDropdownValueChanged += SetSoringMode;
    }

    private int GetItemAmount(string itemName)
    {
        foreach (InventoryItemScript iis in inventoryList)
        {
            if (iis.itemName == itemName)
            {
                return iis.itemAmount;
            }
        }
        return -1;
    }

    void Start()
    {
        inventoryList = new List<InventoryItemScript>();

        for (int i = 0; i < itemNames.Count; i++)
        {
            //Create a duplicate of the starter item
            GameObject inventoryItem = (GameObject)Instantiate(itemPrefub);
            // UI items need to parented by the canvas or an object within the canvas
            inventoryItem.transform.SetParent(parentPanel);
            // Original start item is disabled – so the duplicate must be enabled
            inventoryItem.SetActive(true);
            // Get InventoryItemScript component so we can set the data
            InventoryItemScript iis = inventoryItem.GetComponent<InventoryItemScript>();
            iis.itemNameText.text = itemNames[i];
            iis.itemName = itemNames[i];
            iis.itemAmountText.text = itemAmounts[i].ToString();
            iis.itemAmount = itemAmounts[i];
            //Keep a list of inventory items
            inventoryList.Add(iis);
        }
        DisplayListInOrder();

        compareItems = AscendingName;
    }

    public void SetSoringMode(string sortingMode)
    {
        switch (sortingMode)
        {
            case "Num. Ascending":
                compareItems = AscendingAmount;
                break;
            case "Num. Descending":
                compareItems = DescendingAmount;
                break;
            case "Alph. Ascending":
                compareItems = AscendingName;
                break;
            case "Alph. Descending":
                compareItems = DescendingName;
                break;
            default:
                Debug.Log("Sorting mode in drop down is not recognised, check drop down option names");
                break;
        }
    }

    void DisplayListInOrder()
    {
        // Height of item plus space between each
        float yOffset = 40f;
        Vector3 startPosition;
        // Use the start position for the first item
        if (startItem != null)
        {
            startPosition = startItem.transform.position;
        }
        else
        {
            startPosition = new Vector3(0, 0, 0);
        }

        foreach (InventoryItemScript iis in inventoryList)
        {
            iis.transform.position = startPosition;
            startPosition.y -= yOffset;
        }
    }

    void UpdateItemAmounts(string itemName, bool itemPicked)
    {

        foreach (InventoryItemScript iis in inventoryList)
        {
            if (iis.itemName == itemName)
            {
                iis.itemAmount = itemPicked ? iis.itemAmount + 1 : iis.itemAmount - 1;
                iis.itemAmountText.text = iis.itemAmount.ToString();
                break;
            }
        }
    }

    public void StartMergeSort()
    {
        Debug.Log("Merge sort started");
        inventoryList = MergeSort(inventoryList);
        DisplayListInOrder();
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            StartMergeSort();
        }
    }

    public List<InventoryItemScript> MergeLists(List<InventoryItemScript> list1, List<InventoryItemScript> list2, CompareItems ci)
    {
        List<InventoryItemScript> list = new List<InventoryItemScript>();
        int list1Index = 0, list2Index = 0;

        while (list1Index < list1.Count && list2Index < list2.Count)
        {
            if (ci(list1[list1Index], list2[list2Index]))
            {
                list.Add(list1[list1Index]);
                list1Index++;
            }
            else
            {
                list.Add(list2[list2Index]);
                list2Index++;
            }
        }

        if (list1Index < list1.Count)
        {
            list1.RemoveRange(0, list1Index);
            list.AddRange(list1);
        }
        else
        {
            list2.RemoveRange(0, list2Index);
            list.AddRange(list2);
        }

        return list;
    }

    public List<InventoryItemScript> MergeSort(List<InventoryItemScript> a)
    {
        // Return list when it cointains only 1 element
        if (a.Count <= 1)
            return a;

        // Find mid 
        int mid = a.Count / 2;

        // Instansiate left and right lists
        List<InventoryItemScript> left = new List<InventoryItemScript>();
        List<InventoryItemScript> right = new List<InventoryItemScript>();

        //Add elements to left list
        for (int i = 0; i < mid; i++)
        {
            left.Add(a[i]);
        }

        //Add elements to right list
        for (int i = mid; i < a.Count; i++)
        {
            right.Add(a[i]);
        }

        left = MergeSort(left);
        right = MergeSort(right);

        List<InventoryItemScript> mergedList = MergeLists(left, right, compareItems);
        return mergedList;
    }

    public bool AscendingAmount(InventoryItemScript a, InventoryItemScript b)
    {
        if (a.itemAmount <= b.itemAmount)
        {
            return true;
        }
        return false;
    }

    public bool DescendingAmount(InventoryItemScript a, InventoryItemScript b)
    {
        if (a.itemAmount >= b.itemAmount)
        {
            return true;
        }
        return false;
    }

    public bool AscendingName(InventoryItemScript a, InventoryItemScript b)
    {
        if (string.Compare(a.itemName, b.itemName) <= 0)
        {
            return true;
        }
        return false;
    }

    public bool DescendingName(InventoryItemScript a, InventoryItemScript b)
    {
        if (string.Compare(a.itemName, b.itemName) >= 0)
        {
            return true;
        }
        return false;
    }
}
