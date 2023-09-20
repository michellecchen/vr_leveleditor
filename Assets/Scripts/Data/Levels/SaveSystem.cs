using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Saving level data to the system
/// </summary>
public static class SaveSystem
{

    #region Saving data

    public static void SaveLevel(List<PlacedObjectData> placedObjectData) {

        BinaryFormatter formatter = new BinaryFormatter();

        // Generate filepath
        string filePathSuffix = "/level";
        string filePath = Application.persistentDataPath + filePathSuffix;

        // Creating a new file
        FileStream stream = new FileStream(filePath, FileMode.Create);

        // Create level data from passed-in data re: placed objects
        // string fileName = GenerateFileName();
        LevelData levelData = new LevelData("0", placedObjectData);

        // Format data, write to stream
        formatter.Serialize(stream, levelData);
        // Close stream
        stream.Close();

    }

    // // Generate a file name comprising 4 random numerical digits
    // // For saving files
    // private string GenerateFileName() {

    //     string fileName = "";

    //     for (int i = 0; i < 4; i++) {
    //         fileName += Random.Range(0,10).ToString();
    //     }

    //     return fileName;
    // }

    #endregion
    
    #region Loading data

    public static LevelData LoadLevel() {

        string filePath = Application.persistentDataPath + "/level";

        if (File.Exists(filePath)) {

            // Basic setup
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);

            // Load data from stream
            LevelData levelData = formatter.Deserialize(stream) as LevelData;

            // Close stream
            stream.Close();

            // Return data
            return levelData;
        }
        else {
            Debug.LogError("ERROR: File does not exist at path " + filePath);
            return null;
        }
    }

    #endregion
}
