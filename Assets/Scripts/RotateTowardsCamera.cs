using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used for making sure points of interest/markers are always facing the camera
/// </summary>
public class RotateTowardsCamera : MonoBehaviour
{
    Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }
    void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}
