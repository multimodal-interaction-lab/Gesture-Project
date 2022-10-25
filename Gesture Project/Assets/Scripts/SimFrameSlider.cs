using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static UnityEditor.Experimental.GraphView.GraphView;
using UnityEngine.EventSystems;

public class SimFrameSlider : MonoBehaviour
{
    int startFrameIndex;
    int endFrameIndex;
    public Slider slider;
    [SerializeField]
    Simulator sim;
    [SerializeField]
    TextMeshProUGUI frameText;

    [SerializeField]
    float zoomSpeed;

    [SerializeField]
    const float MIN_WIDTH = 400f;

    [SerializeField]
    const float MAX_WIDTH = Mathf.Infinity;

    RectTransform rectTrans;
    [SerializeField]
    GameObject viewPortObj;
    [SerializeField]
    Transform gestureRegionContainer;

    private void Start()
    {
        slider = GetComponent<Slider>();
        rectTrans = GetComponent<RectTransform>();
    }

    private void Update()
    {
        HandleMouseZoom();
    }

    void HandleMouseZoom()
    {

        if (!IsPointerOverThis(GetEventSystemRaycastResults()))
        {
            return;
        }

        float zoomInput = 1 * Input.GetAxis("Mouse ScrollWheel");
        //Debug.Log(zoomInput);
        float newWidth = rectTrans.sizeDelta.x + zoomInput * zoomSpeed;
        if(newWidth != rectTrans.sizeDelta.x)
        {
            newWidth = Mathf.Clamp(newWidth, MIN_WIDTH, MAX_WIDTH);
            rectTrans.sizeDelta = new Vector2(newWidth, rectTrans.sizeDelta.y);
            foreach(GestureRegion reg in gestureRegionContainer.GetComponentsInChildren<GestureRegion>())
            {
                reg.Rebuild();
            }
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
      

    }

    public void ResetZoom()
    {
        if (MIN_WIDTH != rectTrans.sizeDelta.x)
        {
            rectTrans.sizeDelta = new Vector2(MIN_WIDTH, rectTrans.sizeDelta.y);
            foreach (GestureRegion reg in gestureRegionContainer.GetComponentsInChildren<GestureRegion>())
            {
                reg.Rebuild();
            }
            LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
        }
    }

   
    private bool IsPointerOverThis(List<RaycastResult> eventSystemRaycastResults)
    {
        for (int index = 0; index < eventSystemRaycastResults.Count; index++)
        {
            
            RaycastResult curRaycastResult = eventSystemRaycastResults[index];
            //Debug.Log(curRaycastResult.gameObject.name);
            if (curRaycastResult.gameObject == viewPortObj)
                return true;
        }
        return false;
    }
    //Gets all event system raycast results of current mouse or touch position.
    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);
        
        return raycastResults;
    }

    public void NewSliderBounds(int start, int end)
    {
        startFrameIndex = start;
        endFrameIndex = end;
        int range = end - start;
        slider.maxValue = range - 1;
        slider.minValue = 0;
        slider.value = 0;
    }

    public void SliderFrameChanged()
    {
        int frame = (int)slider.value;
        sim.RenderFrame(frame);
        if(frameText != null)
        {
            frameText.text = "Frame: " + frame;
        }

    }
}
