using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FrameMarker : MonoBehaviour
{
    public int frame;
    public Slider frameSlider;
    public RectTransform sliderRect;
    RectTransform rectTrans;


    private void Start()
    {
        rectTrans = GetComponent<RectTransform>();
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
}
