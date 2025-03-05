// MirrorToggleManager.cs

/**
*
*   Updated March 4th 2025
*
*   INSTRUCTIONS
*
*   1. Create mirrors with 2 different qualities: High and Transparent (See https://wiki.vrchat.com/wiki/Mirrors for VRC's mirror prefab)
*   2. Create quads for each mirror button. Apply button texture to quads as a material.
*   3. Add collider component to each button, or create separate colliders to act as triggers
*   4. Create an empty object to serve as the mirror manager. Place near your mirrors.
*   4. Add this script to the mirror manager. The component will allow you to add the following:
*       4a. References to all mirrors (High/PNG)
*       4b. References to all quads where the buttons are rendered
*       4c. References to the materials for the quads, so they can be swapped between active/inactive
*   5. Add an audio source on the mirror manager object
*       5a. Reference the audio source on the mirror manager
*   6. Select sound files for toggling mirrors on and off
*   7. Optional - Set a custom distance for the mirror automatically turning off
*   8. Apply the MirrorButton.cs script to each mirror button
*
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MirrorToggleManager : UdonSharpBehaviour
{
    [Header("Mirror Objects (GameObjects)")]
    public GameObject mirrorHigh;
    public GameObject mirrorPNG;
    
    [Header("Button Renderers (Quads)")]
    public MeshRenderer buttonHigh;
    public MeshRenderer buttonPNG;
    
    [Header("Button Materials")]
    public Material mirrorHighActiveMaterial;
    public Material mirrorHighInactiveMaterial;
    public Material mirrorPNGActiveMaterial;
    public Material mirrorPNGInactiveMaterial;
    
    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip mirrorOnSound;
    public AudioClip mirrorOffSound;
    
    [Header("Auto Off Settings")]
    [Tooltip("Distance (in world units) at which the mirror will auto-turn off")]
    public float autoOffDistance = 10f;
    
    // State: -1 = no mirror active, 0 = high, 1 = PNG/transparent.
    private int currentActive = -1;
    

    // Called when the High-quality button is pressed
    public void ToggleHigh()
    {
        ToggleMirror(0);
    }
    
    // Called when the Transparent/PNG button is pressed.
    public void TogglePNG()
    {
        ToggleMirror(1);
    }
    
    // Toggles the mirror mode. If requested mode is active, mirror will turn off
    // Otherwise activating a mirror will deactivate others
    private void ToggleMirror(int mode)
    {
        // If toggled mirror is already active, turn it off
        if (currentActive == mode)
        {
            currentActive = -1;
            // Play mirror kill sound
            if (audioSource != null && mirrorOffSound != null)
            {
                audioSource.PlayOneShot(mirrorOffSound);
            }
        }
        else
        {
            currentActive = mode;
            // Play mirror toggle sound
            if (audioSource != null && mirrorOnSound != null)
            {
                audioSource.PlayOneShot(mirrorOnSound);
            }
        }
        UpdateMirrorAndButtons();
    }
    
    // Updates the mirror objects and button materials based on current state
    private void UpdateMirrorAndButtons()
    {
        // Only the active mirror will be enabled
        if (mirrorHigh != null) mirrorHigh.SetActive(currentActive == 0);
        if (mirrorPNG != null) mirrorPNG.SetActive(currentActive == 1);
        
        // Update button materials.
        if (buttonHigh != null)
        {
            buttonHigh.material = (currentActive == 0) ? mirrorHighActiveMaterial : mirrorHighInactiveMaterial;
        }
        if (buttonPNG != null)
        {
            buttonPNG.material = (currentActive == 1) ? mirrorPNGActiveMaterial : mirrorPNGInactiveMaterial;
        }
    }
    
    void Start()
    {
        // Mirror inactive on init
        currentActive = -1;
        UpdateMirrorAndButtons();
    }
    
    void Update()
    {
        // Kill mirror if player is too far from mirror manager
        if (Networking.LocalPlayer != null)
        {
            float dist = Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position);
            if (dist > autoOffDistance && currentActive != -1)
            {
                // Don't play a sound when mirror disabled by distance
                currentActive = -1;
                UpdateMirrorAndButtons();
            }
        }
    }
}