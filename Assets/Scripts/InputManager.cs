using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Old input manager -- written for non-XR testing.
/// </summary>
public class InputManager : MonoBehaviour
{

    private Camera _camera;

    [Header("Layer masks")]
    // For distinguishing terrain, on which objects can be placed
    public LayerMask terrainLayer;
    // For distinguishing objects, which are placed on terrain
    public LayerMask objectLayer;
    private int combinedTerrainLayer;

    // Storing event-related data
    // To be retrieved from the script of whichever operation is currently active
    [HideInInspector] public Vector3 clickedTerrainPos;
    [HideInInspector] public GameObject clickedObject;

    // Stores the current world-position where the mouse is hovering over the terrain
    [HideInInspector] public Vector3 hoveredTerrainPos;
    [HideInInspector] public GameObject hoveredObject;

    // Defining events to be triggered whenever a relevant world element (i.e. terrain/object) has been interacted with
    // The currently active state then responds accordingly
    public event Action OnClickTerrain, OnClickObject, OnRotate;


    private void Start()
    {
        _camera = Camera.main;

        // Treats existing objects as terrain; i.e. allows placement of new objects onto existing ones
        combinedTerrainLayer = (1 << LayerMask.NameToLayer("Object")) | (1 << LayerMask.NameToLayer("Terrain"));
        
    }

    private void Update()
    {
        // If a click is detected that is NOT over UI,
        // check if a relevant in-world element (i.e. object/terrain) was clicked
        if (Input.GetMouseButtonDown(0) && !IsPointerOverUI()) {

            Vector3 inputPos = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(inputPos);
            RaycastHit hit;

            // First check if the click was on an extant object
            // (takes priority over terrain-check)
            if (Physics.Raycast(ray, out hit, 1000f, objectLayer)) {

                // Debug.Log("Clicked on object");
                clickedObject = hit.transform.parent.gameObject;        // record the object
                clickedTerrainPos = hit.point;                          // allow placement onto objects; treat existing objects as terrain
                OnClickObject?.Invoke();                                // trigger the action

            }

            // Then, after verifying the click was not on an object, check if the click was on terrain
            else if (Physics.Raycast(ray, out hit, 1000f, terrainLayer)) {

                // Debug.Log("Clicked on terrain");
                clickedTerrainPos = hit.point;
                OnClickTerrain?.Invoke();

            }
        }

        // If [R] is pressed
        if (Input.GetKeyDown(KeyCode.R)) {
            OnRotate?.Invoke();
        }
    }

    // For ensuring that UI-clicks are disambiguated from in-world clicks
    public bool IsPointerOverUI() {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // CALLER: CreateObject
    // Used for updating the preview of an object the user is currently attempting to place
    // Returns TRUE if the mouse is currently hovering over the terrain, and FALSE otherwise
    // (Hovered terrain position is stored in hoveredTerrainPos, which is then accessed in CreateObject)
    public bool IsPointerOverTerrain() {

        Vector3 mousePos = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        // If hovering over terrain...
        //  NOTE:       Here, we consider existing objects as valid 'terrain' onto which
        //              new objects can be placed.
        
        if (Physics.Raycast(ray, out hit, 1000f, combinedTerrainLayer)) {
            hoveredTerrainPos = hit.point;
            return true;                                                // Return true.

        }

        return false;                                                   // Else, return false.
    }

    public bool IsPointerOverObject() {

        Vector3 mousePos = Input.mousePosition;
        Ray ray = _camera.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1000f, objectLayer)) {        // If hovering over an object...

            hoveredObject = hit.transform.parent.gameObject;
            return true;                                                // Return true.

        }

        return false;                                                   // Else, return false.
    }
}
