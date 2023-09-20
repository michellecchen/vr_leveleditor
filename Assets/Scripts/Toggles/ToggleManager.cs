using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleManager : MonoBehaviour
{
    [Header("Toggles in this collection")]
    public Toggle[] _toggles;

    [Header("On/off colors")]
    public Color defaultColor;          // when toggle is off
    public Color highlightedColor;      // when toggle is on

    private void Start() {

        ResetToggles();

        // Add listeners to toggles, as specified
        AddToggleListeners();
    }

    #region Virtual methods

    protected virtual void AddToggleListeners() {
        // ...
    }

    #endregion

    // For updating toggle UI elements' colors
    protected void HandleToggleValueChanged(Toggle thisToggle, bool isOn) {

        ColorBlock cb = thisToggle.colors;

        if (!isOn) {
            cb.normalColor = defaultColor;
            cb.highlightedColor = defaultColor;
            cb.selectedColor = defaultColor;
        } else {
            cb.normalColor = highlightedColor;
            cb.highlightedColor = highlightedColor;
            cb.selectedColor = highlightedColor;
        }

        thisToggle.colors = cb;

    }

    // Enable or disable both toggles' interactability.
    public void ToggleInteractability(bool toggleValue) {
        foreach (Toggle toggle in _toggles) {
            toggle.interactable = toggleValue;
        }
    }

    // Turn off all toggles
    public void ResetToggles() {
        foreach (Toggle toggle in _toggles) {
            toggle.isOn = false;
            HandleToggleValueChanged(toggle, false);
        }
    }

}
