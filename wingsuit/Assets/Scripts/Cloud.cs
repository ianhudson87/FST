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
        for(int i = 0; i < transform.childCount; i++) {
            GameObject cloud = transform.GetChild(i).gameObject;
            if(cloud.GetComponent<MeshCollider>() != null) {
                cloud.GetComponent<MeshCollider>().enabled = false;
            }
            if(cloud.GetComponent<MeshRenderer>() != null) {
                cloud.GetComponent<MeshRenderer>().material.color = new Color(1.0f, 1.0f, 1.0f, 0.3f);
            }
        }
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
