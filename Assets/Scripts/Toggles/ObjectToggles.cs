using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectToggles : ToggleManager
{
    [Header("CreateObject operation")]
    [SerializeField] private CreateObject _createObject;

    private Toggle toggle1;
    private Toggle toggle2;
    private Toggle toggle3;

    protected override void AddToggleListeners() {
        
        // First set references to the toggles themselves
        toggle1 = _toggles[0];
        toggle2 = _toggles[1];
        toggle3 = _toggles[2];

        // Attach event listeners
        toggle1.onValueChanged.AddListener(OnToggle1);
        toggle2.onValueChanged.AddListener(OnToggle2);
        toggle3.onValueChanged.AddListener(OnToggle3);
    }

    #region Event listeners

    private void OnToggle1(bool toggleValue) {

        // Update colors
        HandleToggleValueChanged(toggle1, toggleValue);

        // Communicate w/ createobject operation
        _createObject.SetObjectPrefab(0);
    }

    private void OnToggle2(bool toggleValue) {

        // Update colors
        HandleToggleValueChanged(toggle2, toggleValue);

        // Communicate w/ createobject operation
        _createObject.SetObjectPrefab(1);
    }

    private void OnToggle3(bool toggleValue) {

        // Update colors
        HandleToggleValueChanged(toggle3, toggleValue);
        
        // Communicate w/ createobject operation
        _createObject.SetObjectPrefab(2);
    }

    #endregion
}
