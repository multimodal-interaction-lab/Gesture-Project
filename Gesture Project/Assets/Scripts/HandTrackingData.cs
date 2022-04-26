using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandTrackingData
{
    public int startFrame;
    public int endFrame;

    public enum Hand
    {
        Right,
        Left
    }

    public enum Finger
    {
        Thumb,
        Index,
        Middle,
        Ring,
        Pinky,
        None //If not a finger, indicates palm
    }

    public enum Joint
    {
        Proximal,
        Distal,
        Tip,
        Knuckle,
        Middle,
        None //For palm
    }

    readonly string[] handStrings = { "R", "L" };
    readonly string[] fingerStrings = { "Thumb", "Index", "Middle", "Ring", "Pinky" };
    readonly string[] jointStrings = { "ProximalJoint", "DistalJoint", "Tip", "Knuckle", "MiddleJoint" };

    public Dictionary<string, List<SerializableVector3>> positionData;
    public Dictionary<string, List<SerializableQuaternion>> rotationData;




    public HandTrackingData()
    {
        startFrame = 0;
        endFrame = 0;
        positionData = new Dictionary<string, List<SerializableVector3>>();
        rotationData = new Dictionary<string, List<SerializableQuaternion>>();

    }

   
    public static string EnumsToString(Hand hand, Finger finger, Joint joint)
    {
        string ret = "";
        ret += hand.ToString();

        if(finger == Finger.None)
        {
            //Palm
            ret += "_Palm";
            return ret;
        }
        ret += "_" + finger.ToString();
        ret += "_" + joint.ToString();

        return ret;
    }

}
