using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class FrameMarkerHandle : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    FrameMarker marker;

    void Start()
    {
        marker = transform.parent.GetComponent<FrameMarker>();
    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        marker.OnPointerDown(eventData);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        marker.OnDrag(eventData);
    }
}
