using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
     public float speed = 5f;

    void Update()
    {
         float moveX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, 0, moveZ));
    }
}
