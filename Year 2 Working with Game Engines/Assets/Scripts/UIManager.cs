using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour {

    public  delegate void LoadGameXMLFile();
    public static event LoadGameXMLFile OnLoadButtonPress;


	public void LoadGame()
    {
        Debug.Log("Load Button Pressed");
        OnLoadButtonPress();
    }

    public void SaveGame()
    {
        Debug.Log("Save Button Pressed");
    }
}
