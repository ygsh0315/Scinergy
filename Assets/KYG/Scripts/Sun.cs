using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sun : Star
{
    public Light SunShine;
    // Start is called before the first frame update
    void Start()
    {
        name = "Sun";
        starName = "Sun";
        ra = 13.35185f;
        dec = -09.56045f;
        generatedType = 1;
        TransformSet();
    }

    // Update is called once per frame
    void Update()
    {
        SunShine.transform.forward = GameManager.instance.CelestialSphere.transform.position - transform.position;
    }
}
