// MirrorButton.cs

/**
*
*   Updated February 27th 2025
* 
*   INSTRUCTIONS
*
*   1. Add this script as a component to your mirror toggle buttons
*   2. Reference the appropriate mirrorManager
*   3. Within the unity inspector, select the appropriate mirror quality for the button (see switch statement below)
*
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MirrorButton : UdonSharpBehaviour
{
    [Tooltip("Reference for the Object containing the Mirror Toggle Manager.")]
    public MirrorToggleManager mirrorManager;

    [Tooltip("Mode: 0 = High, 1 = Low, 2 = Transparent")]
    public int mode;

    // Function will automatically be called when button is interacted with
    public override void Interact()
    {
        switch (mode)
        {
            case 0:
                mirrorManager.ToggleHigh();
                break;
            case 1:
                mirrorManager.ToggleLow();
                break;
            case 2:
                mirrorManager.TogglePNG();
                break;
        }
    }
}
