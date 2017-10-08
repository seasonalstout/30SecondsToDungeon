using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour
{
    private Vector3 offset;         //Private variable to store the offset distance between the player and camera

    private GameObject player;
    // Use this for initialization
    public void SetCamera()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        //Calculate and store the offset value by getting the distance between the player's position and camera's position.
        offset = transform.position - player.transform.position;
    }

    // LateUpdate is called after Update each frame
    void LateUpdate()
    {
        if (player != null)
        // Set the position of the camera's transform to be the same as the player's, but offset by the calculated offset distance.
        transform.position = player.transform.position + offset;
    }
}