using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Container of sorts for PlacedObjectData
/// </summary>
public class PlacedObject : MonoBehaviour
{

    // The data associated with this placed object
    public PlacedObjectData placementData;

    // The key corresponding to the object's data in the 'placedObjects' dictionary
    // (See LevelManager)
    public int objectKey;

    public void SetData(PlacedObjectData data) {
        placementData = data;
    }

    public void SetKey(int key) {
        objectKey = key;
    }

    public void UpdatePlacementData(Vector3 newPos, int newRot) {
        placementData.UpdatePlacement(newPos, newRot);
    }
}
