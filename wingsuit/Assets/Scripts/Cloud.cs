using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    // float speed;
    Vector3 velocity;
    Vector3 basePosition;
    Vector3 offset;
    float timestep;
    void Start()
    {
        basePosition = transform.position;
        velocity = new Vector3(Random.Range(5f, 5f), 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        timestep += Time.deltaTime;

        offset = new Vector3(0, Mathf.Sin(timestep), 0);
        basePosition += velocity * Time.deltaTime;

        transform.position = basePosition + offset;
    }
}
