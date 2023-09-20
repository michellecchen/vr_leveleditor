using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

/// <summary>
/// For detecting inputs from the user in VR.
/// </summary>
public class InputManager_XR : MonoBehaviour
{

    [Header("Layer masks")]
    public LayerMask terrainLayer;              // For distinguishing terrain, on which objects can be placed
    public LayerMask objectLayer;               // For distinguishing objects, which are placed on terrain

    [Header("XR Input")]
    public XRBaseController controller;
    [Tooltip("Used for confirming operations, similar to clicking; rec: A button")]
    public InputActionReference primaryButton;
    [Tooltip("Used for rotating a currently selected object; rec: Y button")]
    public InputActionReference rotateButton;
    [Tooltip("Used for switching between perspectives; rec: grip")]
    public InputActionReference perspectiveButton;

    // Defining events to be triggered whenever a relevant world element (i.e. terrain/object) has been interacted with
    // The currently active state then responds accordingly
    public event Action OnClickTerrain, OnClickObject, OnRotate, TogglePerspective;

    private bool hoveringTerrain;          // Is the user currently hovering over terrain?
    private bool hoveringObject;           // Is the user currently hovering over an object?

    // Storing event-related data
    // To be retrieved from the script of whichever operation is currently active
    [HideInInspector] public Vector3 clickedTerrainPos;
    [HideInInspector] public GameObject clickedObject;

    // Stores the current world-position where the mouse is hovering over the terrain
    [HideInInspector] public Vector3 hoveredTerrainPos;
    [HideInInspector] public GameObject hoveredObject;

    private void Awake() {
        // Attach button listeners
        primaryButton.action.started += PrimaryButtonPressed;
        rotateButton.action.started += RotateButtonPressed;
        perspectiveButton.action.started += PerspectiveButtonPressed;
    }

    private void OnDestroy() {
        // Detach button listeners
        primaryButton.action.started -= PrimaryButtonPressed;
        rotateButton.action.started -= RotateButtonPressed;
        perspectiveButton.action.started -= PerspectiveButtonPressed;
    }

    private void Update()
    {

        Ray ray = new Ray(controller.transform.position, controller.transform.forward);
        RaycastHit hitInfo;
        bool intersecting = Physics.Raycast(ray, out hitInfo, 2000f);

        // Update hovered position, if hovering over terrain/object
        if (intersecting) {

            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Object")) {
                
                // Debug.DrawRay(controller.transform.position, controller.transform.forward * hitInfo.distance, Color.yellow);
                
                hoveringObject = true;
                hoveredObject = hitInfo.collider.gameObject.transform.parent.gameObject;
                
                hoveringTerrain = false;
                hoveredTerrainPos = hitInfo.point;
            }
            else if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Terrain")) {
                
                // Debug.DrawRay(controller.transform.position, controller.transform.forward * hitInfo.distance, Color.green);
                
                hoveringObject = false;

                hoveringTerrain = true;
                hoveredTerrainPos = hitInfo.point;
            }
        }

        else {

            // Debug.DrawRay(controller.transform.position, controller.transform.forward * 2000f, Color.white);

            hoveringTerrain = false;
            hoveringObject = false;
        }
    }

    #region Public methods

    // Called in OperationsManager & in specific operations to determine what actions should be taken, in the context of that operation,
    // while the user has their pointer over an in-world elements (terrain/object).

    public bool IsPointerOverTerrain() {
        return hoveringTerrain;
    }

    public bool IsPointerOverObject() {
        return hoveringObject;
    }

    #endregion

    // Registering 'clicks' on in-world elements (objects/terrain)
    private void PrimaryButtonPressed(InputAction.CallbackContext context) {

        if (hoveringObject) {                                       // 'Clicked' on object
            clickedObject = hoveredObject;
            clickedTerrainPos = hoveredTerrainPos;
            OnClickObject?.Invoke();
        }

        else if (hoveringTerrain) {                                 // 'Clicked' on terrain
            clickedTerrainPos = hoveredTerrainPos;
            OnClickTerrain?.Invoke();
        }
    }

    // Rotating, if applicable, an active/currently selected object
    private void RotateButtonPressed(InputAction.CallbackContext context) {
        OnRotate?.Invoke();
    }

    // Toggling between first-person/overhead perspectives
    private void PerspectiveButtonPressed(InputAction.CallbackContext context) {
        TogglePerspective?.Invoke();
    }
}
