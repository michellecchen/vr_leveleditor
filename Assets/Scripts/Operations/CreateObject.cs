using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Operation for creating an object, i.e. placing it on terrain or an existing object.
/// Process consists of (a) opening up a menu of creatable objects, and (b) allowing users to place whichever object they select from the menu.
/// </summary>
public class CreateObject : MonoBehaviour
{

    [Header("Other components")]
    [SerializeField] private LevelManager _levelManager;

    [Tooltip("Used for tracking where the mouse is currently hovering over the terrain, when displaying the preview")]
    [SerializeField] private InputManager_XR _input;

    [Header("Object database & menu/toggles UI")]
    [SerializeField] private ObjectDatabase _database;
    public GameObject objectMenu;
    public ObjectToggles objectToggles;

    [Header("Parent transform for all created objects")]
    // Just for organizations' sake in the hierarchy
    public Transform objectParent;

    [Header("For displaying object previews")]
    public Material previewMat;
    // We'll create a material instance of the provided preview material
    // so that the original material data remains unaltered.
    private Material matInstance;

    // Are we currently displaying a preview of the current object?
    private bool displayingPreview;

    // The current object that the user is in the process of placing (whose placement has yet to be finalized)
    private GameObject currentObjectPreview;

    private GameObject objectPrefab;                                // Prefab for the object that will be created
    private int objectID;                                           // ID for the object

    // Tracking the number of 90 deg. rotations around y-axis that have been applied to the preview
    // so as to apply it to the actual object, when created
    private int numRotations;

    private void Start() {

        displayingPreview = false;

        // Object menu begins as inactive, and is only activated when 'create' mode becomes active
        objectMenu.SetActive(false);

        // Create a material instance so that the original material data is unchanged
        matInstance = new Material(previewMat);

    }

    private void Update() {

        if (displayingPreview && currentObjectPreview != null) {

            // If the mouse/pointer is currently hovering over the terrain,
            if (_input.IsPointerOverTerrain() || _input.IsPointerOverObject()) {

                // Get the hovered world-space position on the terrain
                Vector3 updatedPreviewPos = _input.hoveredTerrainPos;

                // Update the object preview so that it 'follows' the pointer
                // (updates its position alongside the pointer)
                currentObjectPreview.transform.position = new Vector3(updatedPreviewPos.x, updatedPreviewPos.y, updatedPreviewPos.z);
            }
        }
    }

    // CALLER: OperationsManager
    public void BeginObjectCreation() {

        // Begin by opening up the menu of objects that can be created
        objectMenu.SetActive(true);

        objectPrefab = null;

    }

    // Extract the prefab of the object that has been chosen for placement - from the database, through the provided ID
    public void SetObjectPrefab(int objID) {

        if (currentObjectPreview != null) {
            DestroyObjectPreview();
        }
        
        objectID = objID;
        objectPrefab = _database.objects[objID].prefab;
        
        if (objectPrefab != null) {
            BeginObjectPlacement();
        }
    }

    // Create a new object on the terrain, at the clicked/'target' position
    public void BeginObjectPlacement() {

        // If there is not already an existing object that the user is attempting to place,
        // start placing an object on the terrain - at the clicked position.
        if (currentObjectPreview == null) {

            // Begin by displaying a preview
            DisplayObjectPreview();
        }
    }

    // Cancel the ongoing object creation process.
    // CALLER: ToggleManager > OperationsManager
    public void CancelObjectPlacement() {

        if (currentObjectPreview != null) {

            // Destroy the object preview
            DestroyObjectPreview();
        }

        // Close the object menu
        objectMenu.SetActive(false);

    }

    // Instantiate an object preview
    //      - Make it follow the cursor position (see Update())
    //      - Apply the preview material to it
    private void DisplayObjectPreview() {

        // Instantiate preview GameObject
        currentObjectPreview = Instantiate(objectPrefab);

        // Reset the rotations counter for this new preview
        numRotations = 0;

        // Apply preview material to the CHILD of the instantiated GameObject
        // (The parent is just an empty pivot; the child has the actual mesh + MeshRenderer)
        Renderer childRenderer = currentObjectPreview.transform.GetChild(0).gameObject.GetComponent<Renderer>();
        childRenderer.material = matInstance;
        
        displayingPreview = true;
    }

    // Destroy the object preview, either when user finalizes/confirms or cancels the ongoing placement operation
    private void DestroyObjectPreview() {

        // Destroy the preview GameObject
        Destroy(currentObjectPreview);
        displayingPreview = false;

        // Reset the reference - allow for new objects to be created/placed
        currentObjectPreview = null;
    }

    // Rotate the object preview by 90 deg. clockwise when the user presses [R]
    // CALLER: OperationsManager (upon [R] press)
    public void RotateObjectPreview() {

        // Note that we apply rotations to the object itself - not the pivot, which is the parent Transform

        // First ensure that there is an active preview
        if (displayingPreview && currentObjectPreview != null) {
            
            // Rotate the child 90 deg. around y-axis
            Transform childToRotate = currentObjectPreview.transform.GetChild(0);
            childToRotate.Rotate(Vector3.up, 90.0f);

            // Update counter
            numRotations++;
        }
    }

    // Confirm/finalize the object placement
    public void ConfirmObjectPlacement(Vector3 targetPos) {

        if (currentObjectPreview != null) {

            // Instantiate the 'real' object
            PlacedObject placedObject = PlaceObject(objectID, targetPos, numRotations);
            PlacedObjectData placedObjectData = placedObject.placementData;

            // Update LevelManager with new PlacedObjectData
            // Retrieve the index of where the object has been inserted into the dictionary
            int objectKey = _levelManager.AddObject(placedObjectData);
            placedObject.SetKey(objectKey);

            // Destroy the object preview, now that it's no longer necessary
            DestroyObjectPreview();

            // Allow the user to place another object
            BeginObjectPlacement();
        }
    }

    public PlacedObject PlaceObject(int objID, Vector3 targetPos, int numRot) {

        // Get the prefab corresponding to the provided ID from the object database
        GameObject objPrefab = _database.objects[objID].prefab;

        // Instantiate the new object
        GameObject newObject = Instantiate(objPrefab, targetPos, Quaternion.identity, objectParent);
        // Apply rotations
        newObject.transform.GetChild(0).Rotate(Vector3.up, 90.0f * numRot);

        // Move new object & child to the 'Object' layer
        newObject.layer = LayerMask.NameToLayer("Object");
        newObject.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("Object");

        // Create PlacedObjectData for new object
        PlacedObjectData placedObjectData = new PlacedObjectData(objID, targetPos, numRot);
        // Attach data to object via PlacedObject container
        PlacedObject placedObject = newObject.AddComponent(typeof(PlacedObject)) as PlacedObject;
        placedObject.SetData(placedObjectData);

        return placedObject;

    }
}
