using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatScript : MonoBehaviour
{
    public Transform targetPosition;
    public float speed = 5f;
    public bool canMoving = false;

    private bool isMoving = false;
    private Vector3 startPosition;
    private float startTime;
    private float journeyLength;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && !isMoving && canMoving)
        {
            StartMovement();
        }

        if (isMoving)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, fractionOfJourney);

            if (fractionOfJourney >= 1f)
            {
                isMoving = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
            StartMovement();
        if (collision.transform.tag == "Player")
        {
        }
    }

    public void StartMovement()
    {
        isMoving = true;
        startTime = Time.time;
        journeyLength = Vector3.Distance(startPosition, targetPosition.position);
    }
}
