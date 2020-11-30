using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ScriptableObjectArchitecture;
using TMPro;

public class TextFromProperty : MonoBehaviour
{
    public IntReference property;

    void Update()
    {
        this.GetComponent<TextMeshProUGUI>().text = "" + property.Value;
    }
}
