using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegionButtonInteractivity : MonoBehaviour
{
    public GameObject overlay;
    public Transform gestureRegionContainer;
    public Slider frameSlider;
    public RectTransform sliderHandle;
    public GameObject gestureRegionObj;

    public Button addButton;
    public Button removeButton;
    public Button splitButton;



    public void UpdateInteractivity()
    {
        if (overlay.activeInHierarchy)
        {
            addButton.interactable = false;
            removeButton.interactable = false;
            splitButton.interactable = false;
            return;
        }

        int currentFrame = (int)frameSlider.value;


        //Find the first gesture region
        GestureRegion existingRegion = gestureRegionContainer.GetComponentInChildren<GestureRegion>();
        if (existingRegion == null)
        {
            addButton.interactable = true;
            removeButton.interactable = false;
            splitButton.interactable = false;
            return;
        }

        while(existingRegion.prevRegion != null)
        {
            existingRegion = existingRegion.prevRegion;
        }

        do
        {
            //If region is hovered
            if(currentFrame >= existingRegion.startFrame && currentFrame <= existingRegion.endFrame)
            {
                removeButton.interactable = true;
                addButton.interactable = false;
                if(currentFrame > existingRegion.startFrame && currentFrame < existingRegion.endFrame)
                {
                    splitButton.interactable = true;
                } else
                {
                    splitButton.interactable = false;
                }
                return;
            } 


            existingRegion = existingRegion.nextRegion;
        } while (existingRegion != null);

        //No region hovered
        addButton.interactable = true;
        removeButton.interactable = false;
        splitButton.interactable = false;


    }

    public GestureRegion GetHoveredRegion()
    {
        int currentFrame = (int)frameSlider.value;
        //Find the first gesture region
        GestureRegion existingRegion = gestureRegionContainer.GetComponentInChildren<GestureRegion>();
        if (existingRegion == null)
        {
            return null;
        }

        while (existingRegion.prevRegion != null)
        {
            existingRegion = existingRegion.prevRegion;
        }

        do
        {
            //If region is hovered
            if (currentFrame >= existingRegion.startFrame && currentFrame <= existingRegion.endFrame)
            {
                return existingRegion;
            }


            existingRegion = existingRegion.nextRegion;
        } while (existingRegion != null);

        //No region hovered
        return null;

    }

    public GestureRegion FindPreviousRegion()
    {
        int currentFrame = (int)frameSlider.value;
        //Find the last gesture region
        GestureRegion existingRegion = gestureRegionContainer.GetComponentInChildren<GestureRegion>();
        if (existingRegion == null)
        {
            return null;
        }

        while (existingRegion.nextRegion != null)
        {
            existingRegion = existingRegion.nextRegion;
        }

        do
        {
            //If region is before frame
            if (currentFrame > existingRegion.endFrame)
            {
                return existingRegion;
            }


            existingRegion = existingRegion.prevRegion;
        } while (existingRegion != null);

        //No region hovered
        return null;
    }

    public GestureRegion FindNextRegion()
    {
        int currentFrame = (int)frameSlider.value;
        //Find the first gesture region
        GestureRegion existingRegion = gestureRegionContainer.GetComponentInChildren<GestureRegion>();
        if (existingRegion == null)
        {
            return null;
        }

        while (existingRegion.prevRegion != null)
        {
            existingRegion = existingRegion.prevRegion;
        }

        do
        {
            //If region is after frame
            if (currentFrame < existingRegion.startFrame)
            {
                return existingRegion;
            }


            existingRegion = existingRegion.nextRegion;
        } while (existingRegion != null);

        //No region hovered
        return null;
    }

    public void AddRegion()
    {
        int currentFrame = (int)frameSlider.value;

        var prevGestReg = FindPreviousRegion();
        var nextGestReg = FindNextRegion();

        var newRegionObj = Instantiate(gestureRegionObj, gestureRegionContainer.transform);
        var currentGestReg = newRegionObj.GetComponent<GestureRegion>();
        
        currentGestReg.prevRegion = prevGestReg;
        if (prevGestReg != null)
        {
            prevGestReg.nextRegion = currentGestReg;
        }
        
        currentGestReg.nextRegion = nextGestReg;
        if(nextGestReg != null)
        {
            nextGestReg.prevRegion = currentGestReg;
        }

        currentGestReg.frameSlider = frameSlider;
        foreach (FrameMarker marker in currentGestReg.GetComponentsInChildren<FrameMarker>())
        {
            marker.frameSlider = frameSlider;
            marker.sliderRect = sliderHandle;
            marker.rectTrans = marker.GetComponent<RectTransform>();
            
        }
        currentGestReg.SetStartFrame(currentFrame);
        currentGestReg.SetEndFrame(currentFrame);
        currentGestReg.UpdateStartFrame(currentFrame - 10);
        currentGestReg.UpdateEndFrame(currentFrame + 10);
    }

    public void DeleteRegion()
    {
        int currentFrame = (int)frameSlider.value;

        var hoveredRegion = GetHoveredRegion();
        if(hoveredRegion.prevRegion != null)
        {
            hoveredRegion.prevRegion.nextRegion = hoveredRegion.nextRegion;
        }

        if(hoveredRegion.nextRegion != null)
        {
            hoveredRegion.nextRegion.prevRegion = hoveredRegion.prevRegion;
        }

        Destroy(hoveredRegion.gameObject);

    }

    public void SplitRegion()
    {
        int currentFrame = (int)frameSlider.value;

        var hoveredRegion = GetHoveredRegion();

        var tempEnd = hoveredRegion.endFrame;



        var newRegionObj = Instantiate(gestureRegionObj, gestureRegionContainer.transform);
        var newGestReg = newRegionObj.GetComponent<GestureRegion>();

        newGestReg.prevRegion = hoveredRegion;
        newGestReg.nextRegion = hoveredRegion.nextRegion;
        hoveredRegion.nextRegion = newGestReg;

        newGestReg.frameSlider = frameSlider;
        foreach (FrameMarker marker in newGestReg.GetComponentsInChildren<FrameMarker>())
        {
            marker.frameSlider = frameSlider;
            marker.sliderRect = sliderHandle;
            marker.rectTrans = marker.GetComponent<RectTransform>();

        }
        hoveredRegion.SetEndFrame(currentFrame);
        newGestReg.SetStartFrame(currentFrame + 1);
        newGestReg.SetEndFrame(tempEnd);




    }

}