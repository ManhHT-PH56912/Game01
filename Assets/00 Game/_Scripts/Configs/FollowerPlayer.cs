using UnityEngine;

public class FollowerPlayer : MonoBehaviour
{
    public Transform player; // Reference to the player's transform
    public Vector3 offset;   // Offset between the camera and the player

    // Update is called once per frame
    void LateUpdate()
    {
        if (player != null)
        {
            // Update the camera's position based on the player's position and the offset
            transform.position = player.position + offset;
        }
    }
}
