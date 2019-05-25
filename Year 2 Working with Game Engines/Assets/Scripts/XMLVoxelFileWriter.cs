using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XMLVoxelFileWriter
{
    // Write a voxel chunk to XML file
    public static void SaveChunkToXMLFile(int[,,] voxelArray, string fileName)
    {
        XmlWriterSettings writerSettings = new XmlWriterSettings();
        writerSettings.Indent = true;

        // Create a write instance
        XmlWriter xmlWriter = XmlWriter.Create(fileName + ".xml", writerSettings);

        // Write the beginning of the document
        xmlWriter.WriteStartDocument();

        // Create the root element
        xmlWriter.WriteStartElement("VoxelChunk");

        for (int x = 0; x < voxelArray.GetLength(0); x++)
        {
            for (int y = 0; y < voxelArray.GetLength(1); y++)
            {
                for (int z = 0; z < voxelArray.GetLength(2); z++)
                {
                    if (voxelArray[x, y, z] != 0)
                    {
                        // Create a single voxel element
                        xmlWriter.WriteStartElement("Voxel");

                        // Write an attribute to store the x index
                        xmlWriter.WriteAttributeString("x", x.ToString());

                        // Write an attribute to store the y index
                        xmlWriter.WriteAttributeString("y", y.ToString());

                        // Write an attribute to store the z index
                        xmlWriter.WriteAttributeString("z", z.ToString());

                        // Store the voxel type
                        xmlWriter.WriteString(voxelArray[x, y, z].ToString());

                        // End the voxel element
                        xmlWriter.WriteEndElement();
                    }
                }
            }
        }
        // End the root element
        xmlWriter.WriteEndElement();

        // Write the end of the document
        xmlWriter.WriteEndDocument();
        // Close the document to save
        xmlWriter.Close();

    }

    // Read a voxel chunk from XML file
    public static int[,,] LoadChunkFromXMLFile(int size, string fileName)
    {
        int[,,] voxelArray = new int[size, size, size];
        
        // Create an XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

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

    public static bool ReadStartAndEndPosition(out Vector3 start, out Vector3 end, string fileName)
    {
        bool foundStart = false;
        bool foundEnd = false;

        start = new Vector3(-1, -1, -1);
        end = new Vector3(-1, -1, -1);

        // Create an XML reader with the file supplied
        XmlReader xmlReader = XmlReader.Create(fileName + ".xml");

        // Iterate through and read every line in the XML file
        while (xmlReader.Read())
        {
            if(xmlReader.IsStartElement("start"))
            {
                // Retrieve attributes and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);
                start = new Vector3(x, y, z);
                foundStart = true;
            }
            // Check if this node is an end element
            if (xmlReader.IsStartElement("end"))
            {
                // Retrieve attributes and store as int
                int x = int.Parse(xmlReader["x"]);
                int y = int.Parse(xmlReader["y"]);
                int z = int.Parse(xmlReader["z"]);
                end = new Vector3(x, y, z);
                foundEnd = true;
            }
        }
        return foundStart && foundEnd;
    }
}
