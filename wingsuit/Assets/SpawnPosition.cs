using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPosition : MonoBehaviour
{
    [SerializeField] public bool isTagger;
    [SerializeField] public GameObject model;

    void Awake() {
        model.SetActive(false);
    }
}