using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerOrthoGraphic : MonoBehaviour
{
    public Transform followTarget;

    [SerializeField] public float distance = 10f;
 
    [SerializeField] public Vector2 framingOffset = new Vector2(0,1);

    [SerializeField] bool invertCameraX;
    [SerializeField] bool invertCameraY = true;

    float rotationY;
    float rotationX;

    float invertXVal;
    float invertYVal;

    private void Start() 
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() 
    {
        invertXVal = (invertCameraX) ? -1 : 1;
        invertYVal = (invertCameraY) ? -1 : 1;
   
        rotationX = 45;
        
        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);

        transform.position = focusPosition - targetRotation * new Vector3(0, 0, distance);
        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);
}
