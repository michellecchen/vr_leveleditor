using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// For managing the create/delete modes, which are structured as toggles;
/// i.e. if create mode is active, then delete mode is inactive,
/// and vice versa.
/// </summary>
public class OperationToggles : ToggleManager
{

    [Header("Operations manager")]
    [SerializeField] private OperationsManager _operationsManager;

    private Toggle toggle1;
    private Toggle toggle2;

    protected override void AddToggleListeners() {

        // First set references to the toggles themselves
        toggle1 = _toggles[0];
        toggle2 = _toggles[1];

        // Attach event listeners
        toggle1.onValueChanged.AddListener(OnToggle1);
        toggle2.onValueChanged.AddListener(OnToggle2);
    }

    #region Event listeners

    // Handle updates to toggle 1's value
    // (toggle 1 = create)
    private void OnToggle1(bool toggleValue) {

        // Update colors
        HandleToggleValueChanged(toggle1, toggleValue);

        // Communicate w/ operations manager
        _operationsManager.OnCreateToggle(toggleValue);

    }

    // Handle updates to toggle 2's value
    // (toggle 2 = delete)
    private void OnToggle2(bool toggleValue) {

        // Update colors
        HandleToggleValueChanged(toggle2, toggleValue);

        // Communicate w/ operations manager
        _operationsManager.OnDeleteToggle(toggleValue);

    }

    #endregion
}
