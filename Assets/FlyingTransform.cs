using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingTransform : MonoBehaviour
{
    public IEnumerator Animate(PathCurve path, AnimationCurve animationCurve, float rotation, float animationLength) {
        RectTransform rect = GetComponent<RectTransform>();
        float startTime = Time.time;
        while (Time.time < startTime + animationLength) {
            yield return new WaitForFixedUpdate();
            float t = animationCurve.Evaluate((Time.time - startTime) / animationLength);
            rect.position = path.Lerp(t);
            rect.rotation = Quaternion.AngleAxis(rotation * t, Vector3.forward);
        }
    }
}
