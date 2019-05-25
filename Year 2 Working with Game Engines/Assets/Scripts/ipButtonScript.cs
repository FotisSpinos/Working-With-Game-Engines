using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ipButtonScript : MonoBehaviour {

    string[] ipAddress;
    string gameName;

    public delegate void ButtonPress(string gameName);
    public static event ButtonPress OnButtonPress;

    public void ButtonPressed()
    {
        OnButtonPress(gameName);
    }

    public void SetGameName(string gameName)
    {
        this.gameName = gameName;
    }
}
