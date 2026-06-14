using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchableObject : MonoBehaviour
{
    
    public ParticleSystem ps;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.enabled = true;
    }
    public void OnTouched()
    {
        ps.Play();    
        meshRenderer.enabled = false;
    }
}
