// Teleport_Basic.cs

/**
*
*   Last update: February 27th 2025
*
*   INSTSRUCTIONS
*
*   1. Create an object to serve as a teleport pad
*   2. Create an empty game object to serve as the teleport destination
*   3. Apply this script to the teleport pad as a component
*   4. Reference the destination object within unity's inspector
*   5. (Optional) Fine-tune the teleport radius in unity's inspector to adjust for teleport pad size
*
*/

using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Teleport_Basic : UdonSharpBehaviour
{
    [Tooltip("Reference the gameobject where the player will teleport to")]
    public Transform teleportDestination;
    
    [Tooltip("The radius (in world units) where a player can trigger a teleport")]
    public float teleportRadius = 0.5f;
    
    // Flag so user can only teleport once per entry
    private bool hasTeleported = false;
    
    void Update()
    {
        // Make sure player and teleport destination exist
        if (Networking.LocalPlayer == null || teleportDestination == null)
            return;
        
        // Check player distance from teleport pad's center
        float dist = Vector3.Distance(Networking.LocalPlayer.GetPosition(), transform.position);
        
        if (dist <= teleportRadius)
        {
            if (!hasTeleported)
            {
                //Debug.Log("[Teleport_Basic] Player is within teleport radius (" + dist + " <= " + teleportRadius + "). Teleporting...");
                TeleportLocalPlayer();
                hasTeleported = true;
            }
        }
        else
        {
            if (hasTeleported)
            {
                //Debug.Log("[Teleport_Basic] Player exited teleport radius. Resetting teleport flag.");
                hasTeleported = false;
            }
        }
    }
    
    // Teleports local player to destination
    private void TeleportLocalPlayer()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null)
        {
            //Debug.Log("[Teleport_Basic] Error: Local player is null. Cannot teleport.");
            return;
        }
        
        //Debug.Log("[Teleport_Basic] Teleporting local player to: " + teleportDestination.position);
        localPlayer.TeleportTo(teleportDestination.position, teleportDestination.rotation);
    }
}
