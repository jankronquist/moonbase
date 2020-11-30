using System.Collections;
using System.Collections.Generic;
using ScriptableObjectArchitecture;
using UnityEngine;

public class ShowBasedOnProperty : MonoBehaviour {
    public BoolVariable property;
    public BoolReference target;

    void Start() {
        property.AddListener(PropertyChanged);
        PropertyChanged(property.Value);
    }

    private void OnDestroy() {
        property.RemoveListener(PropertyChanged);
    }

    public void PropertyChanged(bool value) {
        this.gameObject.SetActive(value == target.Value);
    }

}
