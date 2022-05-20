using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimPlayPause : MonoBehaviour
{
    [SerializeField]
    Slider simSlider;

    [SerializeField]
    Transform gestureRegionContainer;

    [SerializeField]
    Sprite playGraphic;
    [SerializeField]
    Sprite pauseGraphic;


    bool isPlaying;

    private void Start()
    {
        isPlaying = false;
    }


    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if(simSlider.value == simSlider.maxValue || (GetHoveredRegion() != null && simSlider.value == GetHoveredRegion().endFrame))
            {
                SetPaused();
            } else
            {
                simSlider.value++;
            }
        }    
    }


    public void TogglePlayPause()
    {
        isPlaying = !isPlaying;
        if (isPlaying)
        {
            GetComponent<Image>().sprite = pauseGraphic;
        } else
        {
            GetComponent<Image>().sprite = playGraphic;
        }

        if(isPlaying && simSlider.value == simSlider.maxValue || (GetHoveredRegion() != null && simSlider.value == GetHoveredRegion().endFrame))
        {
            var region = GetHoveredRegion();
            if (region == null)
            {
                simSlider.value = 0;
            } else
            {
                simSlider.value = region.startFrame;
            }
            
            
        }
    }

    public void SetPaused()
    {
        isPlaying = false;
        GetComponent<Image>().sprite = playGraphic;
    }


    public GestureRegion GetHoveredRegion()
    {
        int currentFrame = (int)simSlider.value;
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
}
