using UnityEngine;
using TMPro;

public class Score : MonoBehaviour
{
    static Score instance;
    [SerializeField] TMP_Text text;
    decimal score = 0;

    //There should only be one Score per scene!
    //Otherwise it is unpredictable which one will become the actual score counter used by everyone.
    void Awake() => instance = this;

    public static float AsFloat() => (float)instance.score;
    public static int AsInt() => (int)instance.score;
    public static string AsText() => AsInt().ToString("D4");

    public static void Add(decimal a) { instance.score += a; instance.text.text = AsText(); }
    public static void Subtract(decimal s) {instance.score -= s; instance.text.text = AsText(); }
}
