using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Focus : MonoBehaviour, IPointerClickHandler {
    private RectTransform focusTransform;
    private Image image;
    private UnityAction call;

    void Start() {
        focusTransform = this.transform as RectTransform;
        image = GetComponent<Image>();
        //button = GetComponent<Button>();
    }

    public void EnableClick(UnityAction call) {
        image.raycastTarget = true;
        this.call = call;
    }

    public void DisableClick() {
        image.raycastTarget = false;
        call = null;
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (eventData.pointerCurrentRaycast.gameObject == this.gameObject && call != null) {
            call.Invoke();
        }
    }

    public void DisableFocus() {
        this.gameObject.SetActive(false);
    }

    public void FocusOn(RectTransform target) {
        this.gameObject.SetActive(true);

        Transform p = focusTransform.parent;
        focusTransform.parent = target;
        focusTransform.anchoredPosition = new Vector2(0, 0);
        focusTransform.sizeDelta = new Vector2(0, 0);
        focusTransform.parent = p;
    }

    public IEnumerator DelayedFocusOn(RectTransform target, float delay) {
        this.gameObject.SetActive(false);
        yield return new WaitForSeconds(delay);
        FocusOn(target);
    }
}
