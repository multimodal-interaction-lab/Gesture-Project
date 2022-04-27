using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRotation : MonoBehaviour
{
    [SerializeField]
    GameObject targetObj;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = targetObj.transform.rotation;
    }
}
