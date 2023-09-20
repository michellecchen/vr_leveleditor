using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Random = System.Random;

/// <summary>
/// Communicating with the SaveSystem: saving & loading level data
/// </summary>
public class LevelManager : MonoBehaviour
{
    // Updated via Create/DeleteObject
    //      * key = object index
    //      * value = data of placed object
    private Dictionary<int, PlacedObjectData> placedObjects;

    [Header("CreateObject functionality - used for loading objects into the scene")]
    [SerializeField] private CreateObject _createObject;
    [Header("Sequence for overwriting data")]
    [SerializeField] private OverwriteData _overwriteData;

    // For random key generation
    private static readonly Random random = new Random();

    private void Start()
    {
        // Create a new dictionary for each session
        placedObjects = new();
    }

    #region General operations (save, load)

    // Called when user presses 'Save' button
    public void Save()
    {

        // Extract list of PlacedObjectData from the dictionary
        List<PlacedObjectData> dataList = placedObjects.Values.ToList();

        // Save data in system
        SaveSystem.SaveLevel(dataList);
    }

    // Called when user presses 'Load' button
    public void Load()
    {
        if (placedObjects.Count > 0) {
            _overwriteData.PromptOverwrite(placedObjects);
        }
        else {
            LoadObjectsFromData();
        }
    }

    // Load saved data, after overwriting unsaved data in the scene
    public void OverwriteLoad() {
        ClearExistingData();
        Load();
    }

    private void ClearExistingData() {
        foreach (int objKey in placedObjects.Keys) {
            RemoveObject(objKey);
        }
    }

    private void LoadObjectsFromData() {

        LevelData levelData = SaveSystem.LoadLevel();

        // If data was loaded successfully...
        if (levelData != null) {

            // Populate world with loaded objects
            List<PlacedObjectData> loadedObjects = levelData.placedObjects;

            foreach (PlacedObjectData objectData in loadedObjects) {

                // Place the object in the scene using CreateObject's placement method
                Vector3 placementPos = new Vector3(objectData.position[0], objectData.position[1], objectData.position[2]);
                PlacedObject placedObject = _createObject.PlaceObject(objectData.objectID, placementPos, objectData.numRotations);
                PlacedObjectData placedObjectData = placedObject.placementData;
                
                // Generate a new random key for the object, for this session
                int objectKey = GenerateObjectKey();
                
                // Update this session's dictionary
                placedObjects[objectKey] = placedObjectData;
                
                // Attach the new key to the placed object
                placedObject.SetKey(objectKey);

            }
        }
    }

    #endregion

    #region Object operations (add, remove, edit)

    // CALLER: CreateObject
    // Add a newly placed object (i.e. its data) to the ongoing placedObjects list
    public int AddObject(PlacedObjectData newObject)
    {

        // Generate a random/unique key for the object
        int objectKey = GenerateObjectKey();

        // Insert the data into the dictinonary @ that key
        placedObjects[objectKey] = newObject;

        return objectKey;
    }

    // CALLER: DeleteObject
    // Deletes an object from the dictionary by key
    public void RemoveObject(int objectKey)
    {
        placedObjects.Remove(objectKey);
    }

    public void EditObject(int objectKey, Vector3 newPos, int newRot)
    {
        PlacedObjectData data = placedObjects[objectKey];
        data.UpdatePlacement(newPos, newRot);
    }

    #endregion

    // Generate a random/unique 3-digit key for the object before inserting it into the placedObjects dictionary
    private int GenerateObjectKey()
    {

        int randKey;

        // Generate & check for uniqueness
        do
        {
            randKey = random.Next(100, 1000);
        } while (placedObjects.ContainsKey(randKey));

        return randKey;
    }
}
