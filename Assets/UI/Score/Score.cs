using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    static Score instance;

    [Header("References")]
    [SerializeField] TMP_Text scoreNumber;
    [SerializeField] GameObject explanationTextPrefab;
    [SerializeField] Vector2 explanationTextOffset;

    [Header("Display")]
    [SerializeField] int goalScore = -1;
    [SerializeField] int numDigits = 4;
    [SerializeField] Color successColour;
    [SerializeField] Color failureColour;

    decimal score = 0;
    RectTransform canvas;

    //There should only be one Score per scene!
    //Otherwise it is unpredictable which one will become the actual score counter used by everyone.
    void Awake()
    { 
        instance = this;
        canvas = FindObjectOfType<Canvas>().GetComponent<RectTransform>(); 
        instance.scoreNumber.text = AsText();
    }

    public static float AsFloat() => (float)instance.score;
    public static int AsInt() => (int)instance.score;
    public static string AsText()
    {
        string t = AsInt().ToString($"D{instance.numDigits}");
        if (instance.goalScore <= 0) return t;

        t += " / " + ((int)instance.goalScore).ToString($"D{instance.numDigits}");
        return t;
    }

    public static bool EvaluateGoal()
    {
        bool result = instance.score >= instance.goalScore;
        instance.scoreNumber.color = result ? instance.successColour : instance.failureColour;
        return result;
    }


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
