using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GradientTest : MonoBehaviour
{
    public Gradient grad;
    public List<Color> colors;
    // Start is called before the first frame update
    void Start()
    {
        colors = new List<Color>();
        for(int i = 0; i < 16; i++)
        {
            colors.Add(grad.Evaluate((1f / 16f) * i));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
