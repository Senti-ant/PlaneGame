using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class FloatingUIThingy : MonoBehaviour
{
    [SerializeField] Vector2 DistanceFloated;
    [SerializeField] float FloatingTime;

    Vector2 initialPosition;
    Camera cam;
    RectTransform rect;
    TMP_Text text;

    public void StartFloating(Vector2 initialPosition)
    {
        cam = Camera.main;
        rect = GetComponent<RectTransform>();
        this.initialPosition = initialPosition;
        text = GetComponent<TMP_Text>();

        StartCoroutine(FloatAway);
        Destroy(gameObject, FloatingTime);
    }


    void SetText(float t)
    {
        Vector2 worldPosition = Vector2.Lerp(initialPosition, initialPosition + DistanceFloated, t);
        rect.position = RectTransformUtility.WorldToScreenPoint(cam, worldPosition);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1f - t);
    }

    IEnumerator FloatAway => Tween.Routine(
        Tween.Linear,
        SetText,
        FloatingTime
    );
}
