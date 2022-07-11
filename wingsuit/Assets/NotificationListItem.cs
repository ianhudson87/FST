using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NotificationListItem : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textField;

    public void SetText(string text) {
        textField.text = text;
    }
}
