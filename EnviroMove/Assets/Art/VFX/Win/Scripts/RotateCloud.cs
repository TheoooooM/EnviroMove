using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCloud : MonoBehaviour
{
    public Vector3 speed;
    public float Delay;
    public AnimationCurve Curve;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time = time + Time.deltaTime;

        if (time >= Delay)
        {
            transform.Rotate(new Vector3 (0, 0, Curve.Evaluate(time) * Time.deltaTime));
        }
        
    }
}
