using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamCollision : MonoBehaviour
{
    public Camera cam;
    Vector3 cameraDir;

    public float camDist;
    public Vector2 camDistMinMax = new Vector2(0.5f, 5f);

    private void Start() {
        cameraDir = transform.localPosition.normalized;
        camDist = camDistMinMax.y;
    }

    public void CheckCamOccAndColl(Camera cam)
    {
        Vector3 desiredCamPos = transform.TransformPoint(cameraDir * 3);
        RaycastHit hit;
        Debug.DrawLine(transform.position, desiredCamPos, Color.red);
        if(Physics.Linecast(transform.position, desiredCamPos, out hit))
        {
            camDist = Mathf.Clamp(hit.distance * 0.9f, camDistMinMax.x, camDistMinMax.y);
        }
        else
        {
            camDist = camDistMinMax.y;
        }

        cam.transform.localPosition = cameraDir * (camDist - 0.1f);
    }
}
