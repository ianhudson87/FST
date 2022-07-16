using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] SpawnPosition taggerSpawnPosition;
    [SerializeField] SpawnPosition runnerSpawnPosition;

    void Awake() {
        Instance = this;
    }

    public Transform GetTaggerSpawn() {
        return taggerSpawnPosition.transform;
    }

    public Transform GetRunnerSpawn(float radius) {
        Transform randomRunnerPosition = new GameObject().transform;
        randomRunnerPosition.position = runnerSpawnPosition.transform.position + new Vector3(Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius));
        randomRunnerPosition.rotation = runnerSpawnPosition.transform.rotation;
        return randomRunnerPosition;
    }
}
