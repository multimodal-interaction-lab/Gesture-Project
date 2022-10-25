using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegionButtonInteractivity : MonoBehaviour
{
    public GameObject overlay;
    public Transform gestureRegionContainer;
    public Slider frameSlider;
    public RectTransform sliderHandle;
    public GameObject gestureRegionObj;

    /*
    public Button addButton;
    public Button removeButton;
    public Button splitButton;
     */
    public Button clearButton;
  
    [SerializeField]
    public GameObject noRegionMenu;
    [SerializeField]
    public GameObject regionMenu;

    [SerializeField]
    public TMP_InputField startText;
    public TMP_InputField endText;

    GestureRegion selectedRegion;

    bool editingRegion;


    private void Start()
    {
        editingRegion = false;
    }
    private void Update()
    {
        UpdateInteractivity();
        if(!editingRegion && selectedRegion != null)
        {
            startText.text = selectedRegion.startFrame.ToString();
            endText.text = selectedRegion.endFrame.ToString();
        }
    }

    public void UpdateInteractivity()
    {
        if (overlay.activeInHierarchy || frameSlider.maxValue == 1f)
        {
            noRegionMenu.SetActive(false);
            regionMenu.SetActive(false);
            selectedRegion = null;
            clearButton.interactable = false;
  
            return;
        }
        clearButton.interactable = true;

        int currentFrame = (int)frameSlider.value;

        //Find the first gesture region
        GestureRegion existingRegion = gestureRegionContainer.GetComponentInChildren<GestureRegion>();
        if (existingRegion == null)
        {
            noRegionMenu.SetActive(true);
            regionMenu.SetActive(false);
            selectedRegion = null;
            return;
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
                noRegionMenu.SetActive(false);
                regionMenu.SetActive(true);
                if(selectedRegion != existingRegion)
                {
                    selectedRegion = existingRegion;
                    startText.text = selectedRegion.startFrame.ToString();
                    endText.text = selectedRegion.endFrame.ToString();
                }
               
                
                
                return;
            }


            existingRegion = existingRegion.nextRegion;
        } while (existingRegion != null);

        //No region hovered
        noRegionMenu.SetActive(true);
        regionMenu.SetActive(false);
        selectedRegion = null;

    }

    public void SelectedField()
    {
        editingRegion = true;
    }

    public void DeselectedField()
    {
        editingRegion = false;
    }

    public void UpdateStartFrame(string input)
    {
        if(selectedRegion == null)
        {
            return;
        }
        int newFrame = int.Parse(input);
        if (newFrame != selectedRegion.startFrame)
        {
            selectedRegion.UpdateStartFrame(newFrame);
            endText.text = selectedRegion.endFrame.ToString();
        }
        frameSlider.value = Mathf.Max(selectedRegion.startFrame, frameSlider.value);

    }

    public void UpdateEndFrame(string input)
    {
        if (selectedRegion == null)
        {
            return;
        }
        int newFrame = int.Parse(input);
        if (newFrame != selectedRegion.endFrame)
        {
            selectedRegion.UpdateEndFrame(newFrame);
            endText.text = selectedRegion.endFrame.ToString();
        }
        frameSlider.value = Mathf.Min(selectedRegion.endFrame, frameSlider.value);

    }

    /* Old version- using the Add, Delete, Split Buttons
    public void UpdateInteractivity()
    {
        if (overlay.activeInHierarchy || frameSlider.maxValue == 1f)
        {
            addButton.interactable = false;
            removeButton.interactable = false;
            splitButton.interactable = false;
            clearButton.interactable = false;
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
            clearButton.interactable = true;
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
                clearButton.interactable = true;
                if (currentFrame > existingRegion.startFrame && currentFrame < existingRegion.endFrame)
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
        clearButton.interactable = true;


    }
    */
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
        currentGestReg.UpdateEndFrame(currentFrame + 50);
    }

    public void DeleteRegion()
    {
        int currentFrame = (int)frameSlider.value;


        if(selectedRegion.prevRegion != null)
        {
            selectedRegion.prevRegion.nextRegion = selectedRegion.nextRegion;
        }

        if(selectedRegion.nextRegion != null)
        {
            selectedRegion.nextRegion.prevRegion = selectedRegion.prevRegion;
        }

        Destroy(selectedRegion.gameObject);
        selectedRegion = null;

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

    public void ClearRegions()
    {
        foreach(Transform t in gestureRegionContainer)
        {
            Destroy(t.gameObject);
        }
    }

}
