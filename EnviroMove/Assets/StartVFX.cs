using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartVFX : MonoBehaviour
{
    [SerializeField] private ParticleSystem vfx;
    // Start is called before the first frame update
    void Start()
    {
        vfx.Play();
    }
    
}
