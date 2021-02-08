using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    Joystick cameraJoystick;
    [SerializeField]
    Transform cameraHolder;
    float pitch = 0.0f, yaw = 0.0f;
    [SerializeField]
    bool invertCamera;

    // Update is called once per frame
    void Update()
    {       

        pitch += cameraJoystick.Direction.normalized.y * rotationSpeed*Time.deltaTime;
        yaw += cameraJoystick.Direction.normalized.x * rotationSpeed*Time.deltaTime;
        
        pitch = Mathf.Clamp(pitch, -80,80);       
        
        if(invertCamera)
            cameraHolder.rotation = Quaternion.Euler(-pitch, yaw ,0);
        else
            cameraHolder.rotation = Quaternion.Euler(pitch, yaw ,0);
    }
}
