using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LevelData
{
    public string fileName;
    public List<PlacedObjectData> placedObjects;

    // Default constructor
    public LevelData(string fileName, List<PlacedObjectData> placedObjects) {
        this.fileName = fileName;
        this.placedObjects = placedObjects;
    }

}