using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    [SerializeField] GameObject mapPrefab;
    [SerializeField] Vector3 mapSize;
    [SerializeField] int iters;

    void Awake() {
        for(int i = -iters; i <= iters; i++) {
            for(int j = -iters; j <= iters; j++) {
                for(int k = -iters; k <= iters; k++) {
                    GameObject mapCopy = Instantiate(mapPrefab, transform.position + new Vector3(i*mapSize.x, j*mapSize.y, k*mapSize.z), Quaternion.identity);
                    mapCopy.transform.SetParent(this.transform);
                }
            }
        }
        // Instantiate(mapPrefab, transform.position, Quaternion.identity);
        // Destroy(this);
    }
}
