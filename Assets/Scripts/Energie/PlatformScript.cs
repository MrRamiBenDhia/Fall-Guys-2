using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformScript : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public Axis movementAxis = Axis.X;
    public float range = 5f;
    public float speed = 2f;

    private Vector3 initialPosition;
    private float timer = 0f;
    private bool movingPositive = true;

    public bool active = false;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (active)
        {

        TranslatePlatform();
        }
    }

    private void TranslatePlatform()
    {
        timer += Time.deltaTime * speed;

        Vector3 newPosition = initialPosition;

        switch (movementAxis)
        {
            case Axis.X:
                newPosition.x += Mathf.Sin(timer) * range;
                break;
            case Axis.Y:
                newPosition.y += Mathf.Sin(timer) * range;
                break;
            case Axis.Z:
                newPosition.z += Mathf.Sin(timer) * range;
                break;
        }

        transform.position = newPosition;
    }
}
