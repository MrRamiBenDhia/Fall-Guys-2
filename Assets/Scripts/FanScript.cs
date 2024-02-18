using UnityEngine;

public class FanScript : MonoBehaviour
{
    public enum Axis { X, Y, Z }

    public Axis movementAxis = Axis.X;
    public float speed = 100f;

    public GameObject rotatingPart;
    public bool active = true;

    void Update()
    {
        if (active)
        {
            Rotate();
        }
    }

    void Rotate()
    {
        switch (movementAxis)
        {
            case Axis.X:
                rotatingPart.transform.Rotate(Vector3.right * speed * Time.deltaTime);
                break;
            case Axis.Y:
                rotatingPart.transform.Rotate(Vector3.up * speed * Time.deltaTime);
                break;
            case Axis.Z:
                rotatingPart.transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
        }
    }
}
