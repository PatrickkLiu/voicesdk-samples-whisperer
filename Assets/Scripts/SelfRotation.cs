using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRotation : MonoBehaviour
{
    public float rotationSpeed = 30f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around its Y-axis
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
