using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Clickable : MonoBehaviour
{
    public UnityEvent ClickEvent;
    bool clickBegan;


    private void OnMouseDown()
    {
        clickBegan = true;
    }

    private void OnMouseUp()
    {
        if (clickBegan)
        {
            ClickEvent.Invoke();
        }
    }

    private void OnMouseExit()
    {
        clickBegan = false;
    }
}
