using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunShootSound : MonoBehaviour
{
    public AudioSource audio;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Click() {
        audio.Play();
    }
}
