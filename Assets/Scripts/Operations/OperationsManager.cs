using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For communicating between InputManager & individual operations;
/// determines which action to take, in the currently active operation, in response to input.
/// </summary>
public class OperationsManager : MonoBehaviour
{
    // Relevant input detected from the user;
    // we use this reference to attach listeners to input events
    public InputManager_XR _input;

    // Using a 'key' for tracking the active operation
    //      * -1 = no active operations
    //      * 0 = create (a new object)
    //      * 1 = delete (an existing object)
    //      * 2 = select (an existing object, & potentially reposition/rotate it)
    public int activeOperation;

    [Header("Specific operations")]
    [SerializeField] private CreateObject _createObject;    // Creating a new object
    [SerializeField] private DeleteObject _deleteObject;    // Deleting an existing object
    [SerializeField] private SelectObject _selectObject;    // Selecting an existing object (& modifying it)

    private void Start()
    {
        // Attach listeners to input events
        _input.OnClickTerrain += HandleTerrainClick;
        _input.OnClickObject += HandleObjectClick;
        _input.OnRotate += HandleRotation;

        // Explicitly begin w/o no operations active
        activeOperation = -1;
    }

    #region Handling input events

    private void HandleTerrainClick() {

        // If 'create' operation is active while user clicks somewhere on the terrain,
        // then an object is created at that clicked position.
        if (activeOperation == 0) {
            _createObject.ConfirmObjectPlacement(_input.clickedTerrainPos);
        }
        // If 'select/edit' operation is active,
        // then the selected object is placed in the new location.
        else if (activeOperation == 2) {
            _selectObject.ExitSelection();
            activeOperation = -1;
        }
    }

    private void HandleObjectClick() {

        // If 'create' operation is active while user clicks on an existing object,
        // a new object is placed onto the existing object.
        if (activeOperation == 0) {
            _createObject.ConfirmObjectPlacement(_input.clickedTerrainPos);
        }

        // If 'delete' operation is active while user clicks an existing object,
        // then the object is destroyed.
        else if (activeOperation == 1) {
            _deleteObject.Delete();
        }

        // If 'select/edit' operation is active,
        // the object being repositioned is placed down.
        else if (activeOperation == 2) {
            _selectObject.ExitSelection();
            activeOperation = -1;
        }

        // If no operation is active,
        // an object becomes selected.
        else if (activeOperation == -1) {
            _selectObject.StartSelection(_input.clickedObject);
            activeOperation = 2;
        }
    }

    private void HandleRotation() {
        
        // The object that is currently being placed in the scene (which is either being created, or edited)
        // will be rotated upon input.

        if (activeOperation == 0) {                         // rotation occurs during creation
            _createObject.RotateObjectPreview();
        }
        else if (activeOperation == 2) {                    // rotation occurs during selection/editing
            _selectObject.RotateSelection();
        }

    }

    #endregion

    #region Handling UI/toggle events

    // When toggle corresponding to create operation has been switched on/off
    // CALLER: ToggleManager
    public void OnCreateToggle(bool toggleValue) {

        if (toggleValue) {
            activeOperation = 0;
            _createObject.BeginObjectCreation();            // entering create mode
        }
        else {
            activeOperation = -1;
            _createObject.CancelObjectPlacement();          // exiting create mode
        }
    }

    // When toggle corresponding to delete operation has been switched on/off
    // CALLER: ToggleManager
    public void OnDeleteToggle(bool toggleValue) {

        if (toggleValue) {

            // begin deletion operation
            activeOperation = 1;
            _deleteObject.BeginObjectDeletion();
        }
        else {
            activeOperation = -1;
            _deleteObject.CancelObjectDeletion();
        }
    }

    #endregion
}
