using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using System.Text;
using System;

public class FileExport : MonoBehaviour
{
    [SerializeField]
    TMP_InputField nameInput;
    [SerializeField]
    FrameMarker startMarker;
    [SerializeField]
    FrameMarker endMarker;
    public HandTrackingData handData;
    [SerializeField]
    Simulator sim;


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
        handData = sim.GetHandTrackingData();
    }

    public void ExportFile()
    {
        string defaultfileName = "gestureData";
        string fileNameInput = nameInput.text == "" ? defaultfileName : nameInput.text;
        string path = Application.dataPath + "/SimulatorOutput/" + fileNameInput + ".csv";
        StreamWriter stream;

        if (File.Exists(path))
        {
            int offset = 1;
            do
            {
                path = Application.dataPath + "/SimulatorOutput/" + fileNameInput + " (" + offset + ")" + ".csv";
                offset++;
            } while (File.Exists(path));
        }

        stream = new StreamWriter(path, false);


        //Write the header
        string headerToWrite = "";
        headerToWrite += "Frame,";
        foreach(Simulator.TrackingPoint point in sim.trackingPoints)
        {
            headerToWrite += GetHeaderString(point.hand, point.finger, point.joint);
            headerToWrite += ",";
        }

        headerToWrite = headerToWrite.TrimEnd(',');

        stream.WriteLine(headerToWrite);



        stream.Close();

    }


    string GetHeaderString(HandTrackingData.Hand hand , HandTrackingData.Finger finger, HandTrackingData.Joint joint)
    {
        string ret = "";
        string jointLabel = (hand == HandTrackingData.Hand.Left ? "L_" : "R_") + (finger != HandTrackingData.Finger.None ? finger.ToString() : "Palm") + (finger != HandTrackingData.Finger.None ? "_" + joint.ToString() : "");
        Array featureValues = Enum.GetValues(typeof(Features));

        foreach (Features val in featureValues)
        {
            ret += jointLabel + "_" + Enum.GetName(typeof(Features), val);
            ret += ",";
        }
        ret = ret.TrimEnd(',');

        return ret;
    }
}
