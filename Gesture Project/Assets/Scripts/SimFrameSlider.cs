using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimFrameSlider : MonoBehaviour
{
    int startFrameIndex;
    int endFrameIndex;
    Slider slider;
    [SerializeField]
    Simulator sim;

    private void Start()
    {
        slider = GetComponent<Slider>();
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

    }
}
