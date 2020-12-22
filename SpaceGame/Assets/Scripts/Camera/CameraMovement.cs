using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField]
    Camera myCamera;
    [SerializeField]
    [Range(0,50f)]
    float zoomSpeed;
    [SerializeField]
    [Range(0,50f)]
    float movementSpeed;
    [SerializeField]
    float maxZoomDist;
    [SerializeField]
    float minZoomDist;
    [SerializeField]
    float maxYDist;
    [SerializeField]
    float minYDist;
    [SerializeField]
    float maxXDist;
    [SerializeField]
    float minXDist;
    [SerializeField]
    float maxZoomToRoom;   
    Plane plane;
    Vector3 initCameraPosition;
    Transform currentZoomTarget;
    bool goToTarget;
    [SerializeField]
    VisualManager visualManager; 
    [SerializeField]
    GetEnumVisualState getVisual;
    [SerializeField]
    float distToChange;
    [SerializeField]
    LayerMask layerToCollide;

    private void Awake()
    {
        if (myCamera == null)
            myCamera = Camera.main;
        initCameraPosition = myCamera.transform.position;
        currentZoomTarget = transform;
    }

    private void Update()
    {

        //Update Plane
        if (Input.touchCount >= 1)
            plane.SetNormalAndPosition(transform.up, transform.position);

        Vector3 direction = Vector3.zero;

        //Scroll movement
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            direction = PlanePositionDelta(touch);
            if (touch.phase == TouchPhase.Moved)
            {
                if(direction.y>0 && (transform.position.y-myCamera.transform.position.y)<=maxYDist) 
                    myCamera.transform.position += Vector3.up * direction.y * movementSpeed * Time.deltaTime;
                if(direction.y<0 && (transform.position.y-myCamera.transform.position.y)>=minYDist) 
                    myCamera.transform.position += Vector3.up * direction.y * movementSpeed * Time.deltaTime;
                if(direction.x<0 && (transform.position.x-myCamera.transform.position.x)<=maxXDist) 
                    myCamera.transform.position += Vector3.right * direction.x * movementSpeed * Time.deltaTime;
                if(direction.x>0 && (transform.position.x-myCamera.transform.position.x)>=minXDist) 
                    myCamera.transform.position += Vector3.right * direction.x * movementSpeed * Time.deltaTime;

                //Comprueba la distancia de la cámara respecto al zoomTarget, si está lo suficientemente cerca canvia el estado a ON_ROOM sino a MOVING_AROUND
                if(Vector3.Distance(myCamera.transform.position,currentZoomTarget.position)>=distToChange && getVisual.state ==VisualManager.VisualState.ON_ROOM)
                {
                    getVisual.SetState(VisualManager.VisualState.MOVING_AROUND);
                    visualManager.ChangeState(getVisual);
                }
                else if(Vector3.Distance(myCamera.transform.position,currentZoomTarget.position)<=distToChange)
                {
                    getVisual.SetState(VisualManager.VisualState.ON_ROOM);
                    visualManager.ChangeState(getVisual);
                }
            } 
            if(touch.tapCount>=2)
            {                    
                Ray rayo = Camera.main.ScreenPointToRay(touch.position);                
                goToTarget = Physics.Raycast(rayo,out RaycastHit hit,1000,layerToCollide);
                
                if(goToTarget)
                {
                    currentZoomTarget = hit.transform.GetComponent<BuildingType>().GetZoomObjective(); 
                    visualManager.SetCurrentRoom(hit.transform.gameObject);                    
                }
            }
        }
        if(goToTarget && currentZoomTarget!=null)
        {    
            if(Vector3.Distance(myCamera.transform.position, currentZoomTarget.position) >= minZoomDist)
            {
                myCamera.transform.position = Vector3.Lerp(myCamera.transform.position,currentZoomTarget.position,Time.deltaTime*zoomSpeed);                
            }
            else 
            {
                goToTarget = false;                
                getVisual.SetState(VisualManager.VisualState.ON_ROOM);
                visualManager.ChangeState(getVisual);                
            }            
        }       


        //Zoom
        if (Input.touchCount >= 2)
        {            
            Vector3 pos1  = PlanePosition(Input.GetTouch(0).position);
            Vector3 pos2  = PlanePosition(Input.GetTouch(1).position);
            Vector3 pos1b = PlanePosition(Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition);
            Vector3 pos2b = PlanePosition(Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition);

            //calc zoom
            float zoom = Vector3.Distance(pos1, pos2) /
                       Vector3.Distance(pos1b, pos2b);

            //direction of the zoom
            Vector3 zoomDirection = ((pos1+pos2)/2 - initCameraPosition).normalized;

            //edge case
            if (zoom == 0 || zoom > 10)
                return;

            if(zoom <1 && Vector3.Distance(myCamera.transform.position, transform.position) <= maxZoomDist)
                myCamera.transform.position -= zoomDirection * zoom * Time.deltaTime * zoomSpeed;
  
            else if(zoom > 1 &&  Vector3.Distance(myCamera.transform.position, transform.position) >= maxZoomToRoom)
                myCamera.transform.position += zoomDirection * zoom * Time.deltaTime * zoomSpeed;
        }
    }

    protected Vector3 PlanePositionDelta(Touch touch)
    {
        //not moved
        if (touch.phase != TouchPhase.Moved)
            return Vector3.zero;
    
        //delta
        Ray rayBefore = myCamera.ScreenPointToRay(touch.position - touch.deltaPosition);
        Ray rayNow = myCamera.ScreenPointToRay(touch.position);
        
        if (plane.Raycast(rayBefore, out float enterBefore) && plane.Raycast(rayNow, out float enterNow))
            return rayBefore.GetPoint(enterBefore) - rayNow.GetPoint(enterNow);

        //not on plane
        return Vector3.zero;
    }

    protected Vector3 PlanePosition(Vector2 screenPos)
    {
        //position
        var rayNow = myCamera.ScreenPointToRay(screenPos);
        if (plane.Raycast(rayNow, out var enterNow))
            return rayNow.GetPoint(enterNow);

        return Vector3.zero;
    }
}
