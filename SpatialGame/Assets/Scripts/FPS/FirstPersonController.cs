using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [SerializeField]
    float walkingSpeed;
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    Joystick moveJoystick;
    [SerializeField]
    Joystick cameraJoystick;
    [SerializeField]
    CharacterController controller;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = moveJoystick.Direction.normalized;
        Vector3 cameraDirection = cameraJoystick.Direction.normalized;
        direction.z = direction.y;
        direction.y = 0;
        controller.Move(direction*walkingSpeed*Time.deltaTime);
    }
}
