using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deplacement_cam : MonoBehaviour
{
    public Transform target;
    
    public Vector3 offset;

    public GameObject player;
    
    void Start()
    {
        offset = target.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        transform.position = target.position - offset;
    } 
}
