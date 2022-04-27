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

    bool isPlaying;

    private void Start()
    {
        isPlaying = false;
    }


    private void FixedUpdate()
    {
        if (isPlaying)
        {
            if(simSlider.value == simSlider.maxValue)
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

        if(isPlaying && simSlider.value == simSlider.maxValue)
        {
            simSlider.value = 0;
        }
    }

    public void SetPaused()
    {
        isPlaying = false;
        GetComponent<Image>().sprite = playGraphic;
    }
}
