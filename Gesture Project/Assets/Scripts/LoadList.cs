using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LoadList : MonoBehaviour
{
    string loadPath;
    TMP_Dropdown loadDropdown;
    [SerializeField]
    Simulator simulator;
    [SerializeField]
    Slider frameSlider;
    [SerializeField]
    RectTransform sliderHandle;
    [SerializeField]
    Button playButton;
    [SerializeField]
    GameObject frameOptionsMenu;
    [SerializeField]
    GameObject exportMenu;
    [SerializeField]
    FrameMarker startMarker;
    [SerializeField]
    FrameMarker endMarker;

    [SerializeField]
    float translateThreshold;
    [SerializeField]
    float rotationThreshold;
    [SerializeField]
    int frameCountThreshold;
    [SerializeField]
    GameObject gestureRegionContainer;
    [SerializeField]
    GameObject gestureRegionObject;
    // Start is called before the first frame update
    void Start()
    {
        loadPath = Application.dataPath + "/RecorderOutput/";
        loadDropdown = GetComponent<TMP_Dropdown>();
        //Populate dropdown
        loadDropdown.ClearOptions();
        var options = new List<string>();
        foreach (string file in Directory.GetFiles(loadPath, "*.trackdat"))
        {
            options.Add(Path.GetFileName(file));
        }
        loadDropdown.AddOptions(options);
    }




    public void LoadFile()
    {
        int fileIndex = loadDropdown.value;
        string fileName = loadDropdown.options[fileIndex].text;

        if (File.Exists(loadPath + fileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(loadPath + fileName, FileMode.Open);

            HandTrackingData data = formatter.Deserialize(stream) as HandTrackingData;

            

            stream.Close();

            simulator.SetHandTrackingData(data);
            simulator.RenderFrame(0);
            SetupSlider(data);
            //SetupGestureRegions();
            frameSlider.value = 0;

            playButton.GetComponent<SimPlayPause>().SetPaused();

            frameSlider.interactable = true;
            playButton.interactable = true;
            foreach(Button btn in frameOptionsMenu.GetComponentsInChildren<Button>())
            {
                btn.interactable = true;
            }
            foreach (Button btn in exportMenu.GetComponentsInChildren<Button>())
            {
                btn.interactable = true;
            }
            //startMarker.GetComponent<Image>().enabled = false;
            //startMarker.frame = 0;
            //endMarker.GetComponent<Image>().enabled = false;
            //endMarker.frame = (data.endFrame - data.startFrame) - 1;
            
        }
        else
        {
            Debug.LogError("Error: Save file not found in " + loadPath + fileName);
        }

    }

    public void SetupSlider(HandTrackingData dat)
    {
        var simSlider = frameSlider.gameObject.GetComponent<SimFrameSlider>();
        simSlider.NewSliderBounds(dat.startFrame, dat.endFrame);
    }

    void SetupGestureRegions()
    {
        foreach(Transform obj in gestureRegionContainer.transform)
        {
            Destroy(obj.gameObject);
        }

        GestureRegion currentGestReg = null;
        GestureRegion prevGestReg = null;
        Queue<bool> frameHistory = new Queue<bool>();
        for(int i = 0; i < frameCountThreshold; i++)
        {
            frameHistory.Enqueue(false);
        }

        for(int i = 0; i <= frameSlider.maxValue; i++)
        {
            frameHistory.Enqueue(IsFrameMoving(i));
            frameHistory.Dequeue();

            bool allTrue = !frameHistory.Contains(false);

            bool allFalse = !frameHistory.Contains(true);

            if (allTrue)
            {
                if(currentGestReg == null)
                {
                    var newObj = Instantiate(gestureRegionObject, gestureRegionContainer.transform);
                    currentGestReg = newObj.GetComponent<GestureRegion>();
                    currentGestReg.prevRegion = prevGestReg;
                    if(prevGestReg != null)
                    {
                        prevGestReg.nextRegion = currentGestReg;
                    }
                    
                    currentGestReg.frameSlider = frameSlider;
                    foreach(FrameMarker marker in currentGestReg.GetComponentsInChildren<FrameMarker>())
                    {
                        marker.frameSlider = frameSlider;
                        marker.sliderRect = sliderHandle;
                        marker.rectTrans = marker.GetComponent<RectTransform>();
                    }
                    currentGestReg.SetStartFrame(i - frameCountThreshold);
                }
            } else if(allFalse)
            {
                if(currentGestReg != null)
                {
                    currentGestReg.SetEndFrame(i - frameCountThreshold);
                    prevGestReg = currentGestReg;
                    currentGestReg = null;
                }
            }
        }

        if(currentGestReg != null)
        {
            currentGestReg.SetEndFrame((int)frameSlider.maxValue);
        }
    }

    bool IsFrameMoving(int frame)
    {
        int internalFrame = frame % (simulator.data.endFrame - simulator.data.startFrame) + simulator.data.startFrame;
        foreach (Simulator.TrackingPoint point in simulator.trackingPoints)
        {
            string key = HandTrackingData.EnumsToString(point.hand, point.finger, point.joint);

            var velocity = ((Vector3)simulator.data.positionData[key][internalFrame] - (Vector3)simulator.data.positionData[key][internalFrame - 1]).magnitude / Time.fixedDeltaTime;

            if(velocity > translateThreshold)
            {
                return true;
            }

            var rotAngle = Quaternion.Angle((Quaternion)simulator.data.rotationData[key][internalFrame], (Quaternion)simulator.data.rotationData[key][internalFrame]);

            if(rotAngle > rotationThreshold)
            {
                return true;
            }

        }

        return false;
    }

}
