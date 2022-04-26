using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Recorder : MonoBehaviour
{
    [SerializeField]
    Button recordButton;
    [SerializeField]
    Sprite startImage;
    [SerializeField]
    Sprite stopImage;
    [SerializeField]
    TextMeshProUGUI timeText;
    [SerializeField]
    GameObject simulator;
    [SerializeField]
    GameObject leapRig;
    [SerializeField]
    GameObject recordMenu;
    [SerializeField]
    GameObject saveMenu;
    [SerializeField]
    TMP_InputField fileNameEntry;


    bool recordPressed;
    bool canStopRecord;
    bool isRecording;
    int currentFrameNum;
    HandTrackingData data;


    [System.Serializable]
    public struct TrackingPoint
    {
        public HandTrackingData.Hand hand;
        public HandTrackingData.Finger finger;
        public HandTrackingData.Joint joint;
        public Transform transform;

    }

    [SerializeField]
    List<TrackingPoint> trackingPoints;

    // Start is called before the first frame update
    void Start()
    {
        currentFrameNum = 0;
        recordPressed = false;
        canStopRecord = false;
        recordMenu.SetActive(true);
        saveMenu.SetActive(false);
    }

    void FixedUpdate()
    {
        if (isRecording)
        {
            foreach(TrackingPoint point in trackingPoints)
            {
                RecordTrackingPoint(point);
            }
            currentFrameNum++;
        }
    }

    void RecordTrackingPoint(TrackingPoint point)
    {
        string key = HandTrackingData.EnumsToString(point.hand, point.finger, point.joint);

        if (!data.positionData.ContainsKey(key))
        {
            data.positionData.Add(key, new List<SerializableVector3>());
            data.rotationData.Add(key, new List<SerializableQuaternion>());
        }

        data.positionData[key].Add(point.transform.localPosition);
        data.rotationData[key].Add(point.transform.localRotation);
    }

    public void StartPreRecording()
    {
        isRecording = true;
        data = new HandTrackingData();
        currentFrameNum = 0;
    }

    public void StartRecording()
    {
        data.startFrame = currentFrameNum;
    }


    public void EndRecording()
    {
        data.endFrame = currentFrameNum;
        isRecording = false;
        simulator.SetActive(true);
        leapRig.SetActive(false);
        var simCntrl = simulator.GetComponent<Simulator>();
        simCntrl.isPlaying = true;
        simCntrl.SetHandTrackingData(data);
        recordMenu.SetActive(false);
        saveMenu.SetActive(true);
    }

    public void RecordPressed()
    {
        if (!recordPressed)
        {
            StartCoroutine(RecordSequence());
        } else
        {
            StopAllCoroutines();
            if (canStopRecord)
            {
                EndRecording();
            }
            recordButton.image.sprite = startImage;
            recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Start Recording";
            isRecording = false;
            recordPressed = false;
            
        }
    }

    public IEnumerator RecordSequence()
    {
        recordPressed = true;
        int timer = 3;
        while(timer > 0)
        {
            timeText.text = timer.ToString();
            if(timer == 1)
            {
                StartPreRecording();
            }
            yield return new WaitForSeconds(1f);
            timer--;
        }

        timeText.text = "";
        StartRecording();
        recordButton.image.sprite = stopImage;
        recordButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop Recording";
        canStopRecord = true;

    }
    public void SavePressed()
    {
        SaveData();
    }

    public void SaveData()
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.dataPath + "/RecorderOutput/" + fileNameEntry.text + ".trackdat";
        FileStream stream;

        if (File.Exists(path))
        {
            int offset = 1;
            do
            {
                path = Application.dataPath + "/RecorderOutput/" + fileNameEntry.text + " (" + offset + ")" + ".trackdat";
                offset++;
            } while (File.Exists(path));
        }

        stream = new FileStream(path, FileMode.Create);


        formatter.Serialize(stream, data);
        stream.Close();
    }

}
