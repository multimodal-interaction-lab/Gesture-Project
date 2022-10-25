using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class Simulator : MonoBehaviour
{
    public SimFrameSlider simSlider;
    public SimPlayPause pauseButton;

    public GameObject handCylinder;
    public GameObject handSphere;
    public float jointRadius;
    public float palmRadius;
    int currentFrame;
    public HandTrackingData data;
    bool dataLoaded;

    [SerializeField]
    Transform gestureRegionContainer;

    public List<KeyCode> pauseKeys;

    public List<KeyCode> forwardKeys;
    public List<KeyCode> backKeys;

    public List<KeyCode> mod5Keys;
    public List<KeyCode> mod10Keys;

    const float KEY_HOLD_DELAY = .5f;

    bool forwardPressed;
    bool backPressed;
    bool pausePressed;

    float timeForwardPressed;
    float timeBackwardPressed;

    enum Direction { Forward, Backward, None }

    Direction frameAdjustDir;
    int modLevel;


    public bool isPlaying;


    [System.Serializable]
    public struct TrackingPoint
    {
        public HandTrackingData.Hand hand;
        public HandTrackingData.Finger finger;
        public HandTrackingData.Joint joint;
        public Transform transform;

    }

    [SerializeField]
    public List<TrackingPoint> trackingPoints;

    // Start is called before the first frame update
    void Start()
    {
        currentFrame = 0;
        foreach(TrackingPoint point in trackingPoints)
        {
            GenerateCylinders(point);
            GenerateSphere(point);
        }
        GenerateNonTrackedCylinders();
        GenerateNonTrackedSpheres();
        dataLoaded = false;
        forwardPressed = false;
        backPressed = false;
        pausePressed = false;
    }

    private void Update()
    {
        if (dataLoaded)
        {
            HandleInputs();

           
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if (pausePressed)
        {
            pauseButton.TogglePlayPause();
            pausePressed = false;
        }

        if (isPlaying)
        {
            if (simSlider.slider.value == simSlider.slider.maxValue || (GetHoveredRegion() != null && simSlider.slider.value == GetHoveredRegion().endFrame))
            {
                pauseButton.SetPaused();
            }
            else
            {
                simSlider.slider.value++;
            }
        } else
        {
         
            if (!dataLoaded)
            {
                return;
            }
            switch (frameAdjustDir)
            {
                case Direction.Forward:
                    simSlider.slider.value = Mathf.Min(simSlider.slider.value + modLevel, (data.endFrame - data.startFrame));
                    break;
                case Direction.Backward:
                    simSlider.slider.value = Mathf.Max(simSlider.slider.value - modLevel, 0);
                    break;
                default:
                    break;
            }
          
        }
        forwardPressed = false;
        backPressed = false;

    }

    public void HandleInputs()
    {

        frameAdjustDir = Direction.None;
        foreach(KeyCode kc in forwardKeys)
        {

            if (Input.GetKeyDown(kc))
            {
                forwardPressed = true;
                timeForwardPressed = Time.time;
            } else if(Input.GetKey(kc) && Time.time - timeForwardPressed > KEY_HOLD_DELAY)
            {
                forwardPressed = true;
            }
        }

        foreach (KeyCode kc in backKeys)
        {

            if (Input.GetKeyDown(kc))
            {
                backPressed = true;
                timeBackwardPressed = Time.time;
            }
            else if (Input.GetKey(kc) && Time.time - timeBackwardPressed > KEY_HOLD_DELAY)
            {
                backPressed = true;
            }
        }

        if((forwardPressed && backPressed) || (!forwardPressed && !backPressed))
        {
            frameAdjustDir = Direction.None;
        } else if (forwardPressed)
        {
            frameAdjustDir = Direction.Forward;
        } else
        {
            frameAdjustDir = Direction.Backward;
        }

        modLevel = 1;

        foreach (KeyCode kc in mod5Keys)
        {
            if (Input.GetKey(kc))
            {
                modLevel = 5;
            }
        }


        foreach (KeyCode kc in mod10Keys)
        {
            if (Input.GetKey(kc))
            {
                modLevel = 10;
            }
        }

        foreach (KeyCode kc in pauseKeys)
        {
            if (Input.GetKeyDown(kc))
            {
                pausePressed = true;
            }
        }
    }

    public void RenderFrame(int frame)
    {
        simSlider.slider.value = frame;
        if(currentFrame != frame)
        {
            currentFrame = frame;
        }
        int frameToRender = frame % (data.endFrame - data.startFrame) + data.startFrame;
        foreach(TrackingPoint point in trackingPoints)
        {
            string key = HandTrackingData.EnumsToString(point.hand, point.finger, point.joint);
            point.transform.localPosition = (Vector3)data.positionData[key][frameToRender];
            point.transform.localRotation = (Quaternion)data.rotationData[key][frameToRender];
        }
    }
    

    void GenerateCylinders(TrackingPoint origin)
    {
        GameObject newCylinder;
        switch (origin.joint)
        {
            case HandTrackingData.Joint.Tip:
                newCylinder = Instantiate(handCylinder, origin.transform);
                newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, origin.finger, HandTrackingData.Joint.Distal).transform;

                break;
            case HandTrackingData.Joint.Distal:
                newCylinder = Instantiate(handCylinder, origin.transform);

                if (origin.finger == HandTrackingData.Finger.Thumb)
                {
                    newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, origin.finger, HandTrackingData.Joint.Proximal).transform;
                }
                else
                {
                    newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, origin.finger, HandTrackingData.Joint.Middle).transform;
                }

                break;
            case HandTrackingData.Joint.Middle:
                newCylinder = Instantiate(handCylinder, origin.transform);
                newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, origin.finger, HandTrackingData.Joint.Knuckle).transform;

                break;
            case HandTrackingData.Joint.Knuckle:
                switch (origin.finger)
                {
                    case HandTrackingData.Finger.Pinky:
                        newCylinder = Instantiate(handCylinder, origin.transform);
                        newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, HandTrackingData.Finger.Ring, origin.joint).transform;
                        switch (origin.hand)
                        {
                            case HandTrackingData.Hand.Right:
                                newCylinder = Instantiate(handCylinder, origin.transform);
                                newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Right/Palm/PalmPinky");
                                break;
                            case HandTrackingData.Hand.Left:
                                newCylinder = Instantiate(handCylinder, origin.transform);
                                newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Left/Palm/PalmPinky");
                                break;
                        }                
                        break;
                    case HandTrackingData.Finger.Ring:
                        newCylinder = Instantiate(handCylinder, origin.transform);
                        newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, HandTrackingData.Finger.Middle, origin.joint).transform;
                        break;
                    case HandTrackingData.Finger.Middle:
                        newCylinder = Instantiate(handCylinder, origin.transform);
                        newCylinder.GetComponent<HandCylinder>().attachedTo = GetTrackingPoint(origin.hand, HandTrackingData.Finger.Index, origin.joint).transform;
                        break;
                    case HandTrackingData.Finger.Index:
                        switch (origin.hand)
                        {
                            case HandTrackingData.Hand.Right:
                                newCylinder = Instantiate(handCylinder, origin.transform);
                                newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Right/Palm/PalmIndexThumb");
                                break;
                            case HandTrackingData.Hand.Left:
                                newCylinder = Instantiate(handCylinder, origin.transform);
                                newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Left/Palm/PalmIndexThumb");
                                break;
                        }
                        break;
                }
                break;
            case HandTrackingData.Joint.Proximal:
                switch (origin.hand)
                {
                    case HandTrackingData.Hand.Right:
                        newCylinder = Instantiate(handCylinder, origin.transform);
                        newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Right/Palm/PalmIndexThumb");
                        break;
                    case HandTrackingData.Hand.Left:
                        newCylinder = Instantiate(handCylinder, origin.transform);
                        newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Left/Palm/PalmIndexThumb");
                        break;
                }
                break;

        }

    }

    void GenerateNonTrackedCylinders()
    {
        GameObject newCylinder;
        newCylinder = Instantiate(handCylinder, transform.Find("Left/Palm/PalmPinky"));
        newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Left/Palm/PalmIndexThumb");
        newCylinder = Instantiate(handCylinder, transform.Find("Right/Palm/PalmPinky"));
        newCylinder.GetComponent<HandCylinder>().attachedTo = transform.Find("Right/Palm/PalmIndexThumb");

    }

    void GenerateNonTrackedSpheres()
    {
        GameObject newSphere;
        newSphere = Instantiate(handSphere, transform.Find("Left/Palm/PalmPinky"));
        newSphere.transform.localScale = new Vector3(jointRadius, jointRadius, jointRadius);
        newSphere = Instantiate(handSphere, transform.Find("Right/Palm/PalmPinky"));
        newSphere.transform.localScale = new Vector3(jointRadius, jointRadius, jointRadius);
        newSphere = Instantiate(handSphere, transform.Find("Left/Palm/PalmIndexThumb"));
        newSphere.transform.localScale = new Vector3(jointRadius, jointRadius, jointRadius);
        newSphere = Instantiate(handSphere, transform.Find("Right/Palm/PalmIndexThumb"));
        newSphere.transform.localScale = new Vector3(jointRadius, jointRadius, jointRadius);
    }

    void GenerateSphere(TrackingPoint origin)
    {
        var newSphere = Instantiate(handSphere, origin.transform);
        if(origin.finger == HandTrackingData.Finger.None)
        {
            newSphere.transform.localScale = new Vector3(palmRadius, palmRadius, palmRadius);
        } else
        {
            newSphere.transform.localScale = new Vector3(jointRadius, jointRadius, jointRadius);
        }
    }

    TrackingPoint GetTrackingPoint(HandTrackingData.Hand hand, HandTrackingData.Finger finger, HandTrackingData.Joint joint)
    {
        for(int i =0; i < trackingPoints.Count; i++)
        {
            if(hand == trackingPoints[i].hand && finger == trackingPoints[i].finger && joint == trackingPoints[i].joint)
            {
                return trackingPoints[i];
            }

        }
        Debug.LogError("Could not find the tracking point with hand = " + hand + ", finger = " + finger + ", joint = " + joint);
        return trackingPoints[0];
    }

    public void SetHandTrackingData(HandTrackingData data)
    {
        this.data = data;
        dataLoaded = true;
    }

    public HandTrackingData GetHandTrackingData()
    {
        return data;
    }

    public GestureRegion GetHoveredRegion()
    {
        int currentFrame = (int)simSlider.slider.value;
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
}
