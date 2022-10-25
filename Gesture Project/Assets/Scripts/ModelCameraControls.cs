using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelCameraControls : MonoBehaviour
{
    Camera cam;
    [SerializeField]
    GameObject workSpace;
    [SerializeField]
    KeyCode dragKey;
    [SerializeField]
    float rotationSpeed;
    [SerializeField]
    float zoomSpeed;

    float zoomLevel;

    const float MIN_ZOOM = .2f;
    const float MAX_ZOOM = .6f;

    bool isDragging;
    Vector3 rotXAxis;
    Vector3 rotYAxis;


    // Start is called before the first frame update
    void Start()
    {
        cam = GetComponent<Camera>();
        zoomLevel = .4f;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMouseDrag();
        HandleMouseZoom();
        
    }


    void HandleMouseDrag()
    {
        if (Input.GetKeyUp(dragKey))
        {
            isDragging = false;
            return;
        }


        if (!isDragging)
        {
            if(mouseInWorkArea() && Input.GetKeyDown(dragKey))
            {
                isDragging = true;
                rotXAxis = Vector3.up;
                rotYAxis = Vector3.right;
            } else
            {
                return;
            }
            
        }
        if (Input.GetKey(dragKey))
        {
            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSpeed;

            workSpace.transform.Rotate(rotXAxis, XaxisRotation);
            workSpace.transform.Rotate(rotYAxis, YaxisRotation, Space.World);
        }

    }

    void HandleMouseZoom()
    {
        if (!mouseInWorkArea())
        {
            return;
        }

        float zoomInput = -1 * Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(zoomInput);
        zoomLevel += zoomInput * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, MIN_ZOOM, MAX_ZOOM);
        cam.orthographicSize = zoomLevel;

    }

    bool mouseInWorkArea()
    {
        var viewPos = cam.ScreenToViewportPoint(Input.mousePosition);
        if(viewPos.x <= 1f && viewPos.x >= 0f && viewPos.y <= 1f && viewPos.y >= 0f)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void ZoomInPressed()
    {
        var zoomInput = -.1f;

        zoomLevel += zoomInput * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, MIN_ZOOM, MAX_ZOOM);
        cam.orthographicSize = zoomLevel;
    }

    public void ZoomOutPressed()
    {
        var zoomInput = .1f;

        zoomLevel += zoomInput * zoomSpeed;
        zoomLevel = Mathf.Clamp(zoomLevel, MIN_ZOOM, MAX_ZOOM);
        cam.orthographicSize = zoomLevel;

    }

    public void YAxisPressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(-90f, 0f, 0f)));
    }

    public void XAxisPressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(0f, 90f, 0f)));
    }

    public void ZAxisPressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(0f, 180f, 0f)));
    }

    public void YInversePressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(90f, 0f, 0f)));
    }

    public void XInversePressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(0f, -90f, 0f)));
    }

    public void ZInversePressed()
    {
        StopAllCoroutines();
        StartCoroutine(RotateCoroutine(Quaternion.Euler(0f, 0f, 0f)));
    }

    IEnumerator RotateCoroutine(Quaternion targetRotation)
    {
        for(int i = 0; i < 50; i++)
        {
            workSpace.transform.rotation = Quaternion.Lerp(workSpace.transform.rotation, targetRotation, .1f);
            yield return new WaitForSeconds(.01f);
        }

        workSpace.transform.rotation = targetRotation;

    }
}
