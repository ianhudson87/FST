using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinCode : MonoBehaviour
{
    private GameObject[] skins;
    // Start is called before the first frame update
    void Start()
    {
        skins = new GameObject[transform.childCount];
        
        for(int i = 0; i < transform.childCount; i++) {
            skins[i] = transform.GetChild(i).gameObject;
            skins[i].SetActive(false);
        }

        if (skins[1]) {
            skins[1].SetActive(true);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
