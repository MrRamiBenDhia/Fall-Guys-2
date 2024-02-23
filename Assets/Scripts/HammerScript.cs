using UnityEngine;

public class HammerScript : MonoBehaviour
{
    public float swingSpeed = 2f; // Speed of swinging motion
    public float maxSwingRange = 45f; // Maximum swing range in degrees
    private Quaternion initialRotation; // Initial rotation of the object

    private void Start()
    {
        // Store the initial rotation of the object
        initialRotation = transform.rotation;
    }

    private void Update()
    {
        // Calculate the angle of swinging motion using a sine wave
        float angle = Mathf.Sin(Time.time * swingSpeed) * maxSwingRange;

        // Calculate the final rotation based on the initial rotation and swinging angle
        Quaternion finalRotation = initialRotation * Quaternion.Euler(-angle, 0, 0);

        // Set the rotation of the object
        transform.rotation = finalRotation;
    }
}
