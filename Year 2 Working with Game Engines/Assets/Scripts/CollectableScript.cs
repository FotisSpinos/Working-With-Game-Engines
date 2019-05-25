using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CollectableScript : MonoBehaviour
{

    public int collectableID;
    public string collectableName;

    private void OnCollisionEnter(Collision hit)
    {
        print("Collsion collectable on " + hit.gameObject.name);

    }
}
