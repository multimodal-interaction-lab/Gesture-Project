using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCylinder : MonoBehaviour
{
    public Transform attachedTo;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(attachedTo);
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, (attachedTo.position - transform.position).magnitude);
    }
}
