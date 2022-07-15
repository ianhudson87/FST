using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;
    [SerializeField] float displayTime;

    void Awake() {
        Destroy(this.gameObject, displayTime);
    }

    public void SetText(string text) {
        textField.text = text;
    }
}
