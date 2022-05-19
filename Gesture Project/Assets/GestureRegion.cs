using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GestureRegion : MonoBehaviour
{
    public GestureRegion prevRegion;
    public GestureRegion nextRegion;
    public FrameMarker startMark;
    public FrameMarker endMark;
    public GameObject imageObj;
    public Slider frameSlider;
    public Color validColor;
    public Color invalidColor;


    public int startFrame;
    public int endFrame;

    RectTransform imageRectTrans;
    RectTransform startMarkTrans;
    RectTransform endMarkTrans;
    bool isValid;
    void Awake()
    {
        isValid = true;
        imageRectTrans = imageObj.GetComponent<RectTransform>();
        startMarkTrans = startMark.GetComponent<RectTransform>();
        endMarkTrans = endMark.GetComponent<RectTransform>();
        startFrame = 0;
        endFrame = int.MaxValue;
    }

    // Update is called once per frame
    void Update()
    {

        imageRectTrans.anchoredPosition = new Vector2((startMarkTrans.anchoredPosition.x + endMarkTrans.anchoredPosition.x) / 2, imageRectTrans.anchoredPosition.y);

        var deltaPos = endMarkTrans.anchoredPosition.x - startMarkTrans.anchoredPosition.x;
        imageRectTrans.sizeDelta = new Vector2(Mathf.Abs(deltaPos), imageRectTrans.sizeDelta.y);
        if(deltaPos < 0 && isValid)
        {
            imageObj.GetComponent<Image>().color = invalidColor;
            isValid = false;
        } else if (deltaPos >=0 && !isValid)
        {
            imageObj.GetComponent<Image>().color = validColor;
            isValid = true;
        }

    }

    public void UpdateStartFrame(int frame)
    {
        frame = (int)Mathf.Clamp(frame, (prevRegion == null ? 0 : prevRegion.endFrame + 1), endFrame - 1);
        startFrame = frame;
        startMark.frame = startFrame;
    }

    public void UpdateEndFrame(int frame)
    {
        frame = (int)Mathf.Clamp(frame, startFrame + 1, (nextRegion == null ? frameSlider.maxValue : nextRegion.startFrame - 1));
        endFrame = frame;
        endMark.frame = endFrame;
    }

    public void SetStartFrame(int frame)
    {
        startFrame = frame;
        startMark.frame = startFrame;
        startMark.UpdatePos(frame);
    }

    public void SetEndFrame(int frame)
    {
        endFrame = frame;
        endMark.frame = endFrame;
        endMark.UpdatePos(frame);
    }

}
