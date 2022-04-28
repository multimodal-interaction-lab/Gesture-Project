using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimPlayPause : MonoBehaviour
{
    [SerializeField]
    Slider simSlider;

    [SerializeField]
    Sprite playGraphic;
    [SerializeField]
    Sprite pauseGraphic;

    [SerializeField]
    FrameMarker startMarker;
    [SerializeField]
    FrameMarker endMarker;

    bool isPlaying;

    private void Start()
    {
        isPlaying = false;
    }


    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if(simSlider.value == simSlider.maxValue || (endMarker.GetComponent<Image>().enabled && (simSlider.value == endMarker.frame)))
            {
                TogglePlayPause();
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

        if(isPlaying && simSlider.value == simSlider.maxValue || (endMarker.GetComponent<Image>().enabled && (simSlider.value == endMarker.frame)))
        {
            if (!startMarker.GetComponent<Image>().enabled)
            {
                simSlider.value = 0;
            } else
            {
                simSlider.value = startMarker.frame;
            }
            
        }
    }

    public void SetPaused()
    {
        isPlaying = false;
        GetComponent<Image>().sprite = playGraphic;
    }
}
