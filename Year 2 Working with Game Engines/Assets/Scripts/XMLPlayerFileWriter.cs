using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XMLPlayerFile{

    public static void SavePlayerToXMLFile(Vector3 pos, Vector3 rot, string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        XmlWriter xmlWriter = XmlWriter.Create(fileName + ".xml", writerSettings);

        if(!File.Exists(fileName + ".xml"))
        {
            Debug.Log("Error occured while creating file: " + fileName + ".xml");
            return;
        }
        Debug.Log("File has been successfully created: " + fileName + ".xml");

        xmlWriter.WriteStartDocument();
        xmlWriter.WriteStartElement("PlayerData");

        // Store Position
        xmlWriter.WriteStartElement("PlayerPos");

        xmlWriter.WriteAttributeString("x", pos.x.ToString());
        xmlWriter.WriteAttributeString("y", pos.y.ToString());
        xmlWriter.WriteAttributeString("z", pos.z.ToString());

        xmlWriter.WriteEndElement();

        // Store Rotation
        xmlWriter.WriteStartElement("PlayerRot");
        xmlWriter.WriteAttributeString("x", rot.x.ToString());
        xmlWriter.WriteAttributeString("y", rot.y.ToString());
        xmlWriter.WriteAttributeString("z", rot.z.ToString());

        xmlWriter.WriteString("0");
        // End the voxel element
        xmlWriter.WriteEndElement();

        // End the voxel element
        xmlWriter.WriteEndDocument();

        // Close file
        xmlWriter.Close();
    }

    public static Vector3[] LoadPlayerFromXMLFile(string fileName)
    {
        Vector3[] playerData = new Vector3[2];
        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

        if (!File.Exists(fileName + ".xml"))
        {
            Debug.Log("The file could not be loaded" + fileName + ".xml");
            return playerData;
        }
        Debug.Log("File found for laoding: " + fileName + ".xml");

        while (xmlReader.Read())
        {
            if (xmlReader.IsStartElement("PlayerPos"))
            {
                float x = float.Parse(xmlReader["x"]);
                float y = float.Parse(xmlReader["y"]);
                float z = float.Parse(xmlReader["z"]);

                playerData[0] = new Vector3(x, y, z);
            }
            else if(xmlReader.IsStartElement("PlayerRot"))
            {
                float x = float.Parse(xmlReader["x"]);
                float y = float.Parse(xmlReader["y"]);
                float z = float.Parse(xmlReader["z"]);

                playerData[1] = new Vector3(x, y, z);
            }
        }
        return playerData;
    }
}
