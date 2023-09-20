using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverwriteData : MonoBehaviour {

    [Header("Screen prompting user to overwrite (or not)")]
    public GameObject overwriteUI;

    [Header("Delete object functionality")]
    public DeleteObject _deleteObject;

    [Header("Level manager")]
    [SerializeField] private LevelManager _levelManager;

    // From LevelManager
    private Dictionary<int, PlacedObjectData> existingObjects;

    private void Start() {
        // Screen is not visible at start
        overwriteUI.SetActive(false);
    }

    // CALLER: LevelManager
    // Conditionally prompt an overwrite
    //      - Has the user placed new objects in the scene, then tried to load an existing scene from data?
    //      - Display prompt if so
    public void PromptOverwrite(Dictionary<int, PlacedObjectData> placedObjects) {
        existingObjects = placedObjects;
        overwriteUI.SetActive(true);
    }

    #region Button callbacks

    // Overwrite unsaved data with saved data
    public void ConfirmOverwrite() {

        // Delete all objects from the scene
        _deleteObject.DeleteAllObjects();

        // Empty the LevelManager's placedObjects dictionary (of unsaved objects)
        // Proceed with loading saved data
        _levelManager.OverwriteLoad();

        // Hide screen
        overwriteUI.SetActive(false);
    }

    public void CancelOverwrite() {
        // Hide screen - do nothing, otherwise
        overwriteUI.SetActive(false);
    }

    #endregion

}