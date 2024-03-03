using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;
using System.Collections;

public class ApprovalDialogue : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] Vector2 PositionOffset;
    [SerializeField] int CountdownSecs = 5;

    [Header("References")]
    [SerializeField] TMP_Text Text;
    [SerializeField] Button CheckButton;
    [SerializeField] Button CrossButton;

    public Airport airport { set; private get; }

    RectTransform rect;
    Camera cam;
    
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        cam = Camera.main;

        StartCoroutine(Countdown());
    }

    void Update()
    {
        if (airport == null)
            return;

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, airport.transform.position);
        rect.position = PositionOffset + screenPoint;
    }

    IEnumerator Countdown()
    {
        for (int i = CountdownSecs; i > 0; i--)
        {
            Text.text = $"Approve flight plan?\n{i}s";
            yield return new WaitForSeconds(1);
        }
        Cancel();
    }

    public void Approve()
    {
        airport.DepartNext();
        Destroy(gameObject);
    }

    public void Cancel()
    {
        airport.CancelNext();
        Destroy(gameObject);
    }
}
