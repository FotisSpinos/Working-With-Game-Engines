using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

public class TestDataClass{

    public string name;
    public string description;

    public TestDataClass(string s1, string s2)
    {
        name = s1;
        description = s2;
    }

    TestDataClass()
    {
        
    }
}
