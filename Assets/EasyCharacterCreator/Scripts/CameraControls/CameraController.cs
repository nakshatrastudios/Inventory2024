using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform followTarget;
    [SerializeField] float rotationSpeed = 2f;

    [SerializeField] public float distance = 3f;

    [SerializeField] float minVerticalAngle = -45;
    [SerializeField] float maxVerticalAngle = 45;

    [SerializeField] public Vector2 framingOffset = new Vector2(0, 1.5f);

    [SerializeField] bool invertCameraX;
    [SerializeField] bool invertCameraY = true;

    float rotationY;
    float rotationX;

    float invertXVal;
    float invertYVal;

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        invertXVal = (invertCameraX) ? -1 : 1;
        invertYVal = (invertCameraY) ? -1 : 1;

        rotationX += Input.GetAxis("Mouse Y") * invertYVal * rotationSpeed;
        rotationX = Mathf.Clamp(rotationX, minVerticalAngle, maxVerticalAngle);

        rotationY += Input.GetAxis("Mouse X") * invertXVal * rotationSpeed;

        var targetRotation = Quaternion.Euler(rotationX, rotationY, 0);

        var focusPosition = followTarget.position + new Vector3(framingOffset.x, framingOffset.y);
        var endPosition = focusPosition - targetRotation * new Vector3(0, 0, distance);

        RaycastHit hit;
        if (Physics.Linecast(focusPosition, endPosition, out hit))
        {
            transform.position = hit.point;
        }
        else
        {
            transform.position = endPosition;
        }

        transform.rotation = targetRotation;
    }

    public Quaternion PlanarRotation => Quaternion.Euler(0, rotationY, 0);

}
