using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimPlayPause : MonoBehaviour
{
    [SerializeField]
    Slider simSlider;

    [SerializeField]
    Simulator simulator;



    [SerializeField]
    Sprite playGraphic;
    [SerializeField]
    Sprite pauseGraphic;


    bool IsPlaying
    {
        get { return simulator.isPlaying; }
        set { simulator.isPlaying = value; }
    }

    private void Start()
    {
        IsPlaying = false;
    }


    private void FixedUpdate()
    {

    }


    public void TogglePlayPause()
    {
        IsPlaying = !IsPlaying;
        if (IsPlaying)
        {
            GetComponent<Image>().sprite = pauseGraphic;
        } else
        {
            GetComponent<Image>().sprite = playGraphic;
        }

        if(IsPlaying && simSlider.value == simSlider.maxValue || (simulator.GetHoveredRegion() != null && simSlider.value == simulator.GetHoveredRegion().endFrame))
        {
            var region = simulator.GetHoveredRegion();
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
        IsPlaying = false;
        GetComponent<Image>().sprite = playGraphic;
    }


   
}
