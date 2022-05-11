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
    Button playButton;
    [SerializeField]
    GameObject frameOptionsMenu;
    [SerializeField]
    GameObject exportMenu;
    [SerializeField]
    FrameMarker startMarker;
    [SerializeField]
    FrameMarker endMarker;
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
            startMarker.GetComponent<Image>().enabled = false;
            startMarker.frame = 0;
            endMarker.GetComponent<Image>().enabled = false;
            endMarker.frame = (data.endFrame - data.startFrame) - 1;
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
}
