using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Toggle between overhead & first-person perspectives
/// Enable locomotion in FP
/// </summary>
public class PerspectiveManager : MonoBehaviour
{

    [Header("XR origin/player")]
    public Transform player;

    [Header("Spawnpoints for both perspectives")]
    public Transform fpSpawn;
    public Transform overheadSpawn;

    [Header("Other components")]
    [SerializeField] private ActionBasedContinuousMoveProvider _continuousMovement;
    [SerializeField] private InputManager_XR _input;
    
    private bool overheadPerspective;

    private void Start() {

        // Start in overhead perspective
        overheadPerspective = true;
        _continuousMovement.enabled = false;

        // Attach listener to relevant button press event
        _input.TogglePerspective += OnTogglePerspective;

    }

    private void OnTogglePerspective() {
        
        // Toggle perspective
        overheadPerspective = !overheadPerspective;

        // Update position & rotation
        Vector3 updatedPos = overheadPerspective ? overheadSpawn.position : fpSpawn.position;
        Quaternion updatedRot = overheadPerspective ? overheadSpawn.rotation : fpSpawn.rotation;
        player.SetPositionAndRotation(updatedPos, updatedRot);

        // Conditionally enable/disable locomotion - available in first-person
        _continuousMovement.enabled = !overheadPerspective;

    }
}
