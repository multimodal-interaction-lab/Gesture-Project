using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;
using System;
using UnityEngine.UI;

public class FileExport : MonoBehaviour
{
    [SerializeField]
    TMP_InputField nameInput;
    /*
    [SerializeField]
    FrameMarker startMarker;
    [SerializeField]
    FrameMarker endMarker;
    */
    [SerializeField]
    Transform gestureRegionContainer;
    [SerializeField]
    GameObject overlay;
    [SerializeField]
    Simulator sim;
    [SerializeField]
    Button exportButton;
    [SerializeField]
    GameObject canvasObj;
    [SerializeField]
    Slider simSlider;
    [SerializeField]
    Simulator.TrackingPoint leftPalm;
    [SerializeField]
    Simulator.TrackingPoint rightPalm;
    StreamWriter stream;


    enum Features
    {
        Position_X,
        Position_Y,
        Position_Z,
        Direction_X,
        Direction_Y,
        Direction_Z,
        Velocity,
        Acceleration
    }


    private void Start()
    {
    }

    public void ExportFile()
    {
        StartCoroutine(ExportFeatureData());

    }

    IEnumerator ExportFeatureData()
    {
        overlay.SetActive(true);
        var overlayText = overlay.GetComponentInChildren<TextMeshProUGUI>();
        overlayText.text = "";
        
        foreach (Selectable obj in canvasObj.GetComponentsInChildren<Selectable>())
        {
            obj.interactable = false;
        }
        string defaultfileName = "gestureData";
        string fileNameInput = nameInput.text == "" ? defaultfileName : nameInput.text;
        string path = Application.dataPath + "/SimulatorOutput/" + fileNameInput + ".csv";
        string altpath = Application.dataPath + "/SimulatorOutput/" + fileNameInput + "-0.csv";


        if (File.Exists(path) || File.Exists(altpath))
        {
            int offset = 1;
            do
            {
                path = Application.dataPath + "/SimulatorOutput/" + fileNameInput + " (" + offset + ")" + ".csv";
                offset++;
            } while (File.Exists(path));
        }

        int regionCount = 0;

        foreach (GestureRegion region in gestureRegionContainer.GetComponentsInChildren<GestureRegion>())
        {
            string modifiedPath = path;
            if (gestureRegionContainer.GetComponentsInChildren<GestureRegion>().Length > 1)
            {
                modifiedPath = path.Substring(0, path.IndexOf(".csv"));
                modifiedPath += "-" + regionCount + ".csv";
            }
            overlayText.text = "Exporting " + modifiedPath + "...";
            stream = new StreamWriter(modifiedPath, false);


            //Write the header
            string headerToWrite = "";
            headerToWrite += "Frame,";
            foreach (Simulator.TrackingPoint point in sim.trackingPoints)
            {
                headerToWrite += GetHeaderString(point.hand, point.finger, point.joint);
                headerToWrite += ",";
            }

            headerToWrite = headerToWrite.TrimEnd(',');

            stream.WriteLine(headerToWrite);


            int frame = 0;

            for (int i = region.startFrame; i <= (region.endFrame - region.startFrame) + region.startFrame; i++)
            {
                simSlider.value = i;
                stream.WriteLine(frame + "," + GetDataString(i));
                frame++;
                yield return new WaitForFixedUpdate();
            }
            stream.Close();
            regionCount++;
        }





        
        foreach (Selectable obj in canvasObj.GetComponentsInChildren<Selectable>())
        {
            obj.interactable = true;
        }
        overlayText.text = "";
        overlay.SetActive(false);

        //exportButton.interactable = true;
    }

    string GetHeaderString(HandTrackingData.Hand hand , HandTrackingData.Finger finger, HandTrackingData.Joint joint)
    {
        string ret = "";
        string jointLabel = HandTrackingData.EnumsToString(hand, finger, joint);
        Array featureValues = Enum.GetValues(typeof(Features));

        foreach (Features val in featureValues)
        {
            ret += jointLabel + "_" + Enum.GetName(typeof(Features), val);
            ret += ",";
        }
        ret = ret.TrimEnd(',');

        return ret;
    }

    string GetDataString(int frame)
    {
        string ret = "";
        Array featureValues = Enum.GetValues(typeof(Features));

        foreach (Simulator.TrackingPoint point in sim.trackingPoints)
        {
            foreach (Features val in featureValues)
            {
                ret += GetFeatureVal(point, val, frame);
                ret += ",";
            }
        }


        
        ret = ret.TrimEnd(',');

        return ret;
    }

    string GetFeatureVal(Simulator.TrackingPoint point, Features feature, int frame)
    {
        int internalFrame = frame % (sim.data.endFrame - sim.data.startFrame) + sim.data.startFrame;
        string ret = "";
        string key = HandTrackingData.EnumsToString(point.hand, point.finger, point.joint);
        switch (feature)
        {
            case Features.Position_X:
                //ret += sim.data.positionData[key][internalFrame].x;
                if(point.finger == HandTrackingData.Finger.None)
                {
                    ret += point.transform.position.x;
                } else
                {
                    if(point.hand == HandTrackingData.Hand.Left)
                    {
                        ret += point.transform.position.x - leftPalm.transform.position.x;
                    } else
                    {
                        ret += point.transform.position.x - rightPalm.transform.position.x;
                    }
                }
                
                break;
            case Features.Position_Y:
                //ret += sim.data.positionData[key][internalFrame].y;
                if (point.finger == HandTrackingData.Finger.None)
                {
                    ret += point.transform.position.y;
                }
                else
                {
                    if (point.hand == HandTrackingData.Hand.Left)
                    {
                        ret += point.transform.position.y - leftPalm.transform.position.y;
                    }
                    else
                    {
                        ret += point.transform.position.y - rightPalm.transform.position.y;
                    }
                }
                break;
            case Features.Position_Z:
                //ret += sim.data.positionData[key][internalFrame].z;
                if (point.finger == HandTrackingData.Finger.None)
                {
                    ret += point.transform.position.z;
                }
                else
                {
                    if (point.hand == HandTrackingData.Hand.Left)
                    {
                        ret += point.transform.position.z - leftPalm.transform.position.z;
                    }
                    else
                    {
                        ret += point.transform.position.z - rightPalm.transform.position.z;
                    }
                }
                break;
            case Features.Direction_X:
                /*
                if (internalFrame < 1)
                {
                    ret += "?";
                }
                else
                {
                    ret += ((Vector3)sim.data.positionData[key][internalFrame] - (Vector3)sim.data.positionData[key][internalFrame - 1]).normalized.x;
                }
                */
                ret += point.transform.forward.x;
                break;
            case Features.Direction_Y:
                /*
                if (internalFrame < 1)
                {
                    ret += "?";
                }
                else
                {
                    ret += ((Vector3)sim.data.positionData[key][internalFrame] - (Vector3)sim.data.positionData[key][internalFrame - 1]).normalized.y;
                }
                */
                ret += point.transform.forward.y;
                break;
            case Features.Direction_Z:
                /*
                if (internalFrame < 1)
                {
                    ret += "?";
                }
                else
                {
                    ret += ((Vector3)sim.data.positionData[key][internalFrame] - (Vector3)sim.data.positionData[key][internalFrame - 1]).normalized.z;
                }
                */
                ret += point.transform.forward.z;
                break;
            case Features.Velocity:
                /*
                if(internalFrame < 1)
                {
                    ret += "?";
                } else
                {
                    ret += ((Vector3)sim.data.positionData[key][internalFrame] - (Vector3)sim.data.positionData[key][internalFrame - 1]).magnitude / Time.fixedDeltaTime;
                }
                */
                if (internalFrame < 1)
                {
                    ret += "?";
                }
                else
                {
                    var orig = point.transform.position;
                    point.transform.localPosition = (Vector3)sim.data.positionData[key][internalFrame - 1];
                    var p1 = point.transform.position;
                    ret += (orig - p1).magnitude / Time.fixedDeltaTime;
                    point.transform.position = orig;
                }
                break;
            case Features.Acceleration:
                /*
                if (internalFrame < 2)
                {
                    ret += "?";
                }
                else
                {
                    var v1 = ((Vector3)sim.data.positionData[key][internalFrame] - (Vector3)sim.data.positionData[key][internalFrame - 1]) / Time.fixedDeltaTime;
                    var v2 = ((Vector3)sim.data.positionData[key][internalFrame - 1] - (Vector3)sim.data.positionData[key][internalFrame - 2]) / Time.fixedDeltaTime;
                    ret += (v1 - v2).magnitude / Time.fixedDeltaTime;
                }
                */
                if (internalFrame < 2)
                {
                    ret += "?";
                }
                else
                {
                    var orig = point.transform.position;
                    point.transform.localPosition = (Vector3)sim.data.positionData[key][internalFrame - 1];
                    var p1 = point.transform.position;
                    point.transform.localPosition = (Vector3)sim.data.positionData[key][internalFrame - 2];
                    var p2 = point.transform.position;
                    ret += (((orig - p1) / Time.fixedDeltaTime) - ((p1 - p2) / Time.fixedDeltaTime)).magnitude / Time.fixedDeltaTime;
                    point.transform.position = orig;
                }
                break;
        }
        return ret;
    }

}
