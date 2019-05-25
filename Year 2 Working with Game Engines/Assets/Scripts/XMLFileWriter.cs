using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

public class XMLFileWriter : MonoBehaviour
{
    public static void SaveChunkToXMLFile(int[,,] voxelArray, string fileName, Vector3 start, Vector3 end)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        XmlWriter xmlWriter = XmlWriter.Create(fileName + ".xml", writerSettings);


        if (SaveErrorOccured(fileName))
        {
            return;
        }

        xmlWriter.WriteStartDocument();

        xmlWriter.WriteStartElement("VoxelChunk");

        xmlWriter.WriteStartElement("start");
        xmlWriter.WriteAttributeString("x", start.x.ToString());
        xmlWriter.WriteAttributeString("y", start.y.ToString());
        xmlWriter.WriteAttributeString("z", start.z.ToString());

        xmlWriter.WriteStartElement("end");
        xmlWriter.WriteAttributeString("x", end.x.ToString());
        xmlWriter.WriteAttributeString("y", end.y.ToString());
        xmlWriter.WriteAttributeString("z", end.z.ToString());

        for (int x = 0; x < voxelArray.GetLength(0); x++)
        {
            for (int y = 0; y < voxelArray.GetLength(1); y++)
            {
                for (int z = 0; z < voxelArray.GetLength(2); z++)
                {
                    if (voxelArray[x, y, z] != 0)
                    {
                        xmlWriter.WriteStartElement("Voxel");

                        xmlWriter.WriteAttributeString("x", x.ToString());

                        xmlWriter.WriteAttributeString("y", y.ToString());

                        xmlWriter.WriteAttributeString("z", z.ToString());

                        xmlWriter.WriteString(voxelArray[x, y, z].ToString());

                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndElement();
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndDocument();

        xmlWriter.Close();
    }

    public static int[,,] LoadChunkFromXMLFile(int size, string fileName)
    {
        int[,,] voxelArray = new int[size, size, size];

        // Create an XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

        if (LoadErrorOccured(fileName))
        {
            return new int[0, 0, 0];
        }

        // Iterate through and read every line in the XML file
        while (xmlReader.Read())
        {
            // Check if this node is a Voxel element
            if (xmlReader.IsStartElement("Voxel"))
            {
                // Retrieve x attribute and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);
                xmlReader.Read();

                int value = int.Parse(xmlReader.Value);
                voxelArray[x, y, z] = value;
            }
        }
        return voxelArray;
    }

    public static int[,,] LoadChunkFromXMLFile(int size, out Vector3 start, out Vector3 end, string fileName)
    {
        start = new Vector3(0, 0, 0);
        end = new Vector3(0, 0, 0);
        int[,,] voxelArray = new int[size, size, size];
        // Create an XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

        if (LoadErrorOccured(fileName))
        {
            return new int[0, 0, 0];
        }

        // Iterate through and read every line in the XML file
        while (xmlReader.Read())
        {
            // Check if this node is a Start element
            if (xmlReader.IsStartElement("start"))
            {
                start.x = int.Parse(xmlReader["x"]);
                start.y = int.Parse(xmlReader["y"]);
                start.z = int.Parse(xmlReader["z"]);
                Debug.Log(start);
            }

            // Check if this node is an End element
            if (xmlReader.IsStartElement("end"))
            {
                end.x = int.Parse(xmlReader["x"]);
                end.y = int.Parse(xmlReader["y"]);
                end.z = int.Parse(xmlReader["z"]);
                Debug.Log(end);
            }

            // Check if this node is a Voxel element
            if (xmlReader.IsStartElement("Voxel"))
            {
                // Retrieve x attribute and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);
                xmlReader.Read();

                int value = int.Parse(xmlReader.Value);
                voxelArray[x, y, z] = value;
            }
        }
        return voxelArray;
    }

    public static void SavePlayerToXMLFile(Vector3 pos, Vector3 rot, string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        XmlWriter xmlWriter = XmlWriter.Create(fileName + ".xml", writerSettings);

        if (SaveErrorOccured(fileName))
        {
            return;
        }

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

        if (LoadErrorOccured(fileName))
        {
            return playerData;
        }

        while (xmlReader.Read())
        {
            if (xmlReader.IsStartElement("PlayerPos"))
            {
                float x = float.Parse(xmlReader["x"]);
                float y = float.Parse(xmlReader["y"]);
                float z = float.Parse(xmlReader["z"]);

                playerData[0] = new Vector3(x, y, z);
            }
            else if (xmlReader.IsStartElement("PlayerRot"))
            {
                float x = float.Parse(xmlReader["x"]);
                float y = float.Parse(xmlReader["y"]);
                float z = float.Parse(xmlReader["z"]);

                playerData[1] = new Vector3(x, y, z);
            }
        }
        return playerData;
    }

    static bool SaveErrorOccured(string fileName)
    {
        if (!File.Exists(fileName + ".xml"))
        {
            Debug.Log("Error occured while creating file: " + fileName + ".xml");
            return true;
        }
        Debug.Log("File has been successfully created: " + fileName + ".xml");
        return false;
    }

    static bool LoadErrorOccured(string fileName)
    {
        if (!File.Exists(fileName + ".xml"))
        {
            Debug.Log("The file could not be loaded" + fileName + ".xml");
            return true;
        }
        Debug.Log("File found for laoding: " + fileName + ".xml");
        return false;
    }
}