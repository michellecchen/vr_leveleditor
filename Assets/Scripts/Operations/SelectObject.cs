using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectObject : MonoBehaviour
{

    [Header("Other components")]

    [Tooltip("For checking whether or not the pointer is still hovering over valid terrain")]
    [SerializeField] private InputManager_XR _input;
    [Tooltip("For sending updates to selected objects' positions/rotations")]
    [SerializeField] private LevelManager _levelManager;

    [Header("Selection material")]
    public Material selectionMat;

    // Whether or not we're currently repositioning the currently selected object
    // For now, this is true by default every time an object is selected, but this can be
    // augmented to only be true when a certain button, for instance, is pressed.
    private bool repositioning;

    // The currently selected object, if any
    private GameObject selectedObject;
    private Renderer objectRenderer;
    private Material originalMat;

    // Number of 90-deg. rotations around the y-axis applied *during selection*
    private int numRotations;

    private void Update() {

        // Make object follow pointer, if pointer is over valid terrain
        if (selectedObject != null && repositioning && (_input.IsPointerOverTerrain() || _input.IsPointerOverObject())) {

            // Get the hovered world-space position on the terrain
            Vector3 updatedPos = _input.hoveredTerrainPos;

            // Update the object's position
            selectedObject.transform.position = new Vector3(updatedPos.x, updatedPos.y, updatedPos.z);
        }
    }

    // Called upon rotation button press press
    public void RotateSelection() {

        if (selectedObject != null && repositioning) {

            // Rotate the child 90 deg. around y-axis
            Transform childToRotate = selectedObject.transform.GetChild(0);
            childToRotate.Rotate(Vector3.up, 90.0f);

            // Update num. rotations counter
            numRotations++;
        }
    }

    // Called upon clicking an object
    public void StartSelection(GameObject selection) {

        if (selectedObject == null) {

            Debug.Log("Starting selection");

            // Set references, state, etc.
            selectedObject = selection;
            objectRenderer = selectedObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
            originalMat = objectRenderer.material;
            numRotations = selection.GetComponent<PlacedObject>().placementData.numRotations;
            
            repositioning = true;                                   // True by default w/ selection, for now, but can be modified

            // Apply selection material to object
            ApplyMaterial(selectionMat);

            // While object is selected, temporarily move it off 'Object' layer
            // to a special 'Selection' layer
            selectedObject.layer = LayerMask.NameToLayer("Selection");
            selectedObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Selection");
        }
    }

    private void ApplyMaterial(Material updatedMat) {
        if (selectedObject != null && objectRenderer != null) {
            objectRenderer.material = updatedMat;
        }
    }

    // Called upon clicking anywhere while an object is selected
    public void ExitSelection() {

        if (selectedObject != null) {

            Debug.Log("Ending selection");

            // Move the object back to the 'Object' layer
            selectedObject.layer = LayerMask.NameToLayer("Object");
            selectedObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Object");

            // Apply the default/unselected material to the object
            ApplyMaterial(originalMat);

            // Update placement data with new position, rotation
            Vector3 updatedPos = selectedObject.transform.position;
            PlacedObject placedObject = selectedObject.GetComponent<PlacedObject>();
            placedObject.UpdatePlacementData(updatedPos, numRotations);
            // Send updates to LevelManager
            int objKey = placedObject.objectKey;
            _levelManager.EditObject(objKey, updatedPos, numRotations);

            // Reset references, state, etc.
            selectedObject = null;
            objectRenderer = null;
            repositioning = false;
            numRotations = 0;
        }
    }
}
