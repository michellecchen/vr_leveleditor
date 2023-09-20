using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlacedObjectData {
    public int objectID;
    public float[] position;
    public int numRotations;

    // Default constructor
    public PlacedObjectData(int objectID, Vector3 position, int numRotations) {

        this.objectID = objectID;

        // deconstructing Vector3 into 3 floats for serializability
        this.position = new float[3];
        this.position[0] = position.x;
        this.position[1] = position.y;
        this.position[2] = position.z;
        
        this.numRotations = numRotations;
    }

    // Update position/rotation
    public void UpdatePlacement(Vector3 newPos, int newRot) {

        this.position[0] = newPos.x;
        this.position[1] = newPos.y;
        this.position[2] = newPos.z;

        this.numRotations = newRot;
    }
}