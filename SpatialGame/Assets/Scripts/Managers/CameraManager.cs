using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    Camera localCamera;
    [SerializeField]
    Camera otherCamera;    
    [SerializeField]
    Transform initPosLocal;
    [SerializeField]
    Transform initPosOther;
    [SerializeField]
    Transform attackingPosition;
    [SerializeField]
    float cameraMovementSpeed;
    bool moveToAttackingPos;
    Transform target;

	private void Update()
	{
		if(moveToAttackingPos)
		{
            localCamera.transform.position = Vector3.Lerp(localCamera.transform.position, attackingPosition.position, cameraMovementSpeed * Time.deltaTime);
            localCamera.transform.LookAt(target); 
            otherCamera.transform.position = Vector3.Lerp(otherCamera.transform.position, attackingPosition.position, cameraMovementSpeed * Time.deltaTime);
            otherCamera.transform.LookAt(target);
		}
		else
		{           
            localCamera.transform.position = Vector3.Lerp(localCamera.transform.position, initPosLocal.position, cameraMovementSpeed * Time.deltaTime);
            localCamera.transform.rotation = Quaternion.Lerp(localCamera.transform.rotation, initPosLocal.rotation,cameraMovementSpeed * Time.deltaTime);
            otherCamera.transform.position = Vector3.Lerp(otherCamera.transform.position, initPosOther.position, cameraMovementSpeed * Time.deltaTime);
            otherCamera.transform.rotation = Quaternion.Lerp(otherCamera.transform.rotation, initPosOther.rotation,cameraMovementSpeed * Time.deltaTime);
		}
	}
	public void LookAtAttackingDrone(Transform drone)
	{   
        moveToAttackingPos = true;
        target = drone;
	}
    public void ReturnToCameraPosition()
	{
        target = null;
        moveToAttackingPos = false;
	}
    public void EnableLocalCamera(bool value)
	{
        localCamera.enabled = value;
	}
    public void EnableOtherCamera(bool value)
	{
        otherCamera.enabled = value;
	}    
}
