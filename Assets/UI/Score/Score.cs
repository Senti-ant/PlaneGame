using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    static Score instance;

    [SerializeField] TMP_Text scoreNumber;

    [SerializeField] GameObject explanationTextPrefab;
    [SerializeField] Vector2 explanationTextOffset;
    decimal score = 0;

    RectTransform canvas;

    //There should only be one Score per scene!
    //Otherwise it is unpredictable which one will become the actual score counter used by everyone.
    void Awake()
    { 
        instance = this;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>(); 
    }

    public static float AsFloat() => (float)instance.score;
    public static int AsInt() => (int)instance.score;
    public static string AsText() => AsInt().ToString("D4");

    public static void Add(decimal a) { instance.score += a; instance.scoreNumber.text = AsText(); }
    public static void Subtract(decimal s) {instance.score -= s; instance.scoreNumber.text = AsText(); }
    public static void Add(decimal a, string reason, Vector2 position)
    {
        Add(a);

        TMP_Text t = Instantiate(instance.explanationTextPrefab, new Vector2(10000, 10000), Quaternion.identity, instance.canvas)
                     .GetComponent<TMP_Text>();
        t.text = $"{reason} +{a}";
        t.GetComponent<FloatingUIThingy>().StartFloating(position + instance.explanationTextOffset);
    }
    public static void Subtract(decimal s, string reason, Vector2 position)
    {
        Subtract(s);

        TMP_Text t = Instantiate(instance.explanationTextPrefab, new Vector2(10000, 10000), Quaternion.identity, instance.canvas)
                     .GetComponent<TMP_Text>();
        t.text = $"{reason} -{s}";
        t.GetComponent<FloatingUIThingy>().StartFloating(position + instance.explanationTextOffset);
    }
}
