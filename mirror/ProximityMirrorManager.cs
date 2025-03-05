// ProximityMirrorManager.cs

/**
*
*   Created March 4th 2025
*
*   INSTRUCTIONS
*
*   1. Create a transparent/PNG mirror (See https://wiki.vrchat.com/wiki/Mirrors for VRC's mirror prefab)
*   2. Create an empty GameObject to act as the proximity mirror manager
*   3. Add this script as a component to your proximity mirror manager GameObject
*   4. From within Unity's inspector, configure the following:
*       4a. Reference to your transparent mirror GameObject
*       4b. Set your desired activation and deactivation distances
*   5. Optional - Add an audio source on the manager:
*       5a. Reference the audio source on the mirror manager
*       5b. Add sound clips for mirror activation/deactivation
*   6. Position the mirror manager object near your mirror
*
*   HOW IT WORKS
*
*   - The mirror will automatically become visible when a player enters the activation range
*   - The mirror will automatically hide when a player leaves the deactivation range
*   - No buttons or manual interaction needed
*
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class ProximityMirrorManager : UdonSharpBehaviour
{
    [Header("Mirror Object")]
    [Tooltip("Reference to your transparent/PNG mirror GameObject")]
    public GameObject mirrorObject;
    
    [Header("Proximity Settings")]
    [Tooltip("Distance (in world units) at which the mirror will activate")]
    public float activationDistance = 3.0f;
    
    [Tooltip("Distance (in world units) at which the mirror will deactivate")]
    public float deactivationDistance = 5.0f;
    
    [Header("Audio Settings (Optional)")]
    [Tooltip("Optional audio source for activation/deactivation sounds")]
    public AudioSource audioSource;
    
    [Tooltip("Sound to play when mirror activates")]
    public AudioClip mirrorActivateSound;
    
    [Tooltip("Sound to play when mirror deactivates")]
    public AudioClip mirrorDeactivateSound;

    [Header("Performance Settings (Optional)")]
    [Tooltip("How often (in seconds) to check player distance")]
    public float updateFrequency = 0.5f;
    
    // Internal variables
    private bool mirrorActive = false;
    private float timeSinceLastUpdate = 0f;
    
    void Start()
    {
        // Ensure mirror is off at start
        if (mirrorObject != null)
        {
            mirrorObject.SetActive(false);
        }
        mirrorActive = false;
        
        // Validate settings
        if (deactivationDistance < activationDistance)
        {
            Debug.LogWarning("ProximityMirrorManager: Deactivation distance is smaller than activation distance. This may cause rapid toggling.");
        }
    }
    
    void Update()
    {
        // Only update distance checks periodically to improve performance
        timeSinceLastUpdate += Time.deltaTime;
        if (timeSinceLastUpdate < updateFrequency)
        {
            return;
        }
        timeSinceLastUpdate = 0f;
        
        // Check if local player exists
        if (Networking.LocalPlayer == null || mirrorObject == null)
        {
            return;
        }
        
        // Get distance between player and this manager object
        float distanceToPlayer = Vector3.Distance(transform.position, Networking.LocalPlayer.GetPosition());
        
        // Handle mirror activation
        if (!mirrorActive && distanceToPlayer <= activationDistance)
        {
            ActivateMirror();
        }
        // Handle mirror deactivation
        else if (mirrorActive && distanceToPlayer >= deactivationDistance)
        {
            DeactivateMirror();
        }
    }
    
    private void ActivateMirror()
    {
        mirrorObject.SetActive(true);
        mirrorActive = true;
        
        // Play activation sound if available
        if (audioSource != null && mirrorActivateSound != null)
        {
            audioSource.PlayOneShot(mirrorActivateSound);
        }
    }
    
    private void DeactivateMirror()
    {
        mirrorObject.SetActive(false);
        mirrorActive = false;
        
        // Play deactivation sound if available
        if (audioSource != null && mirrorDeactivateSound != null)
        {
            audioSource.PlayOneShot(mirrorDeactivateSound);
        }
    }
}