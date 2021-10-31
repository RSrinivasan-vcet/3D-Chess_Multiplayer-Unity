using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObject : MonoBehaviour
{
    Vector3 target;
    float speed;

    void Update()
    {
        float distance = Vector3.Distance(target, transform.position);

        // will only move while the distance is bigger than 1.0 units
        if (distance > 1.0f)
        {
            Vector3 dir = target - transform.position;
            dir.Normalize();                                    // normalization is obligatory
            transform.position =Vector3.Lerp(transform.position,target,3* Time.deltaTime); // using deltaTime and speed is obligatory
        }
    }
}