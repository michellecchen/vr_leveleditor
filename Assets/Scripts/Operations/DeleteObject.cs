using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    [Header("Other scripts")]
    // Used for tracking the object that the pointer is currently hovering over (if any)
    [SerializeField] private InputManager_XR _input;
    [SerializeField] private LevelManager _levelManager;

    [Header("Parent transform of all objects")]
    public Transform objectParent;

    [Header("For previewing object deletion")]
    public Material previewMat;
    public Material defaultMat;

    // --- PRIVATE/NON-EXPOSED VARIABLES ---

    // Is delete mode currently active?
    private bool isDeleting = false;

    // The current object that *could* be deleted
    private GameObject currentObject;
    // The renderer of the current object
    private Renderer currentRenderer;

    private void Update()
    {

        if (isDeleting) {

            if (_input.IsPointerOverObject()) {
                
                // If the user hovers into a new object...
                // (either from a diff. object, or from empty space)
                if (_input.hoveredObject != currentObject) {

                    // If there was a previously hovered object, reset its material.
                    if (currentObject != null && currentRenderer != null) {
                        StopDisplayingPreview();
                    }

                    // Update references
                    currentObject = _input.hoveredObject;
                    currentRenderer = currentObject.transform.GetChild(0).gameObject.GetComponent<Renderer>();
                    // Preview deletion of this newly hovered object
                    DisplayDeletionPreview();

                }
            }

            else {

                // If the user hovers 'off' of an object (and into empty space),
                // reset the formerly hovered object's material.
                if (currentObject != null && currentRenderer != null) {
                    StopDisplayingPreview();
                    // Reset references
                    currentObject = null;
                    currentRenderer = null;
                }
            }

        }
    }

    public void BeginObjectDeletion()
    {
        isDeleting = true;
    }

    public void CancelObjectDeletion()
    {
        isDeleting = false;
    }

    // Preview deletion for the current object;
    // Apply the deletion preview material
    private void DisplayDeletionPreview()
    {
        currentRenderer.material = previewMat;
    }

    // Stop previewing deletion for the current object;
    // Reapply the old material
    private void StopDisplayingPreview()
    {
        currentRenderer.material = defaultMat;
    }

    // Delete the current(ly hovered) object, upon click
    // CALLER: OperationsManager
    public void Delete()
    {
        DeleteObjectFromScene(currentObject);

        // Reset references
        currentObject = null;
        currentRenderer = null;
    }

    // Delete a specified GameObject from the scene
    public void DeleteObjectFromScene(GameObject objectToDelete) {
        // Delete from placedObjects dictionary
        int objKey = objectToDelete.GetComponent<PlacedObject>().objectKey;
        _levelManager.RemoveObject(objKey);

        // Destroy the object in the scene
        Destroy(objectToDelete);
    }

    // CALLER: OverwriteData
    // Delete all objects from the scene
    public void DeleteAllObjects() {
        foreach (Transform child in objectParent) {
            DeleteObjectFromScene(child.gameObject);
        }
    }
}
