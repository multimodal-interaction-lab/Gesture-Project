using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FrameMarker : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    GestureRegion gestRegion;
    public int frame;
    public Slider frameSlider;
    public RectTransform sliderRect;
    public RectTransform rectTrans;


    private void Awake()
    {
        rectTrans = GetComponent<RectTransform>();
        gestRegion = GetComponentInParent<GestureRegion>();
    }


    public void MarkCurrentFrame()
    {
        GetComponent<Image>().enabled = true;
        frame = (int)frameSlider.value;
        rectTrans.position = sliderRect.position;
        
    }

    public void MarkClicked()
    {
        frameSlider.value = frame;
    }


    public virtual void OnPointerDown(PointerEventData eventData)
    {
        frameSlider.value = frame;
        //FindObjectOfType<SimPlayPause>().SetPaused();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        frameSlider.OnDrag(eventData);
        if(gestRegion.startMark == this)
        {
            gestRegion.UpdateStartFrame((int)frameSlider.value);
            frameSlider.value = gestRegion.startFrame;
            if(gestRegion.startFrame == (int)frameSlider.value)
            {
                rectTrans.position = sliderRect.position;
            }
        } else if (gestRegion.endMark == this)
        {
            gestRegion.UpdateEndFrame((int)frameSlider.value);
            frameSlider.value = gestRegion.endFrame;
            if (gestRegion.endFrame == (int)frameSlider.value)
            {
                rectTrans.position = sliderRect.position;
            }
        }

        
    }

    public void UpdatePos(int newFrame)
    {
        var origVal = frameSlider.value;
        frameSlider.value = newFrame;
        rectTrans.position = sliderRect.position;
        frameSlider.value = origVal;
    }
}
